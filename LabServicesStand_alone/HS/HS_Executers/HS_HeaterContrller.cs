using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Execute;
using LabMCESystem.LabElement;
using FP23;
namespace LabMCESystem.Servers.HS.HS_Executers
{
    class HS_HeaterContrller : IDisposable
    {
        /// <summary>
        /// 指定电炉的485通讯串口号与地址创建电炉控制对象。
        /// </summary>
        /// <param name="ComName">串口编号。</param>
        /// <param name="addr">电炉485通讯地址。</param>
        public HS_HeaterContrller(string caption, string ComName, int addr)
        {
            Caption = caption;
            _heaterCtrl = new FP23Ctrl(ComName);
            Addr = addr;
        }

        #region Properties

        /// <summary>
        /// 电炉通讯控制。
        /// </summary>
        private FP23Ctrl _heaterCtrl;

        /// <summary>
        /// 每个电炉的真实最小要求流量，如果低于此值应该关闭加热器。
        /// </summary>
        static public double HeaterTrueRequirMinFlow = 300;
        /// <summary>
        /// 在电炉控制时的基础流量，在控制流量下对使用电炉数量计量的基本参数。
        /// </summary>
        static public double HeaterControlBaseFlow = 500;

        /// <summary>
        /// 获取/设置电炉是否准备好的状态通道。
        /// </summary>
        public StatusChannel HeaterReadyChannel { get; set; }
        /// <summary>
        /// 获取/设置电炉运行状态通道。
        /// </summary>
        public StatusChannel HeaterRunStatusChannel { get; set; }
        /// <summary>
        /// 获取/设置电炉启动通道。
        /// </summary>
        public StatusOutputChannel HeaterStartChannel { get; set; }
        /// <summary>
        /// 获取/设置电炉停止通道。
        /// </summary>
        public StatusOutputChannel HeaterStopChannel { get; set; }
        /// <summary>
        /// 获取/设置电炉故障状态通道。
        /// </summary>
        public StatusChannel HeaterFualtChannel { get; set; }
        /// <summary>
        /// 获取/设置加热的远程在线状态通道。
        /// </summary>
        public StatusChannel HeaterConnectionChannel { get; set; }
        /// <summary>
        /// 获取/设置加热器的远程、本地控制状态通道。
        /// </summary>
        public StatusChannel HeaterRemoteControlChannle { get; set; }

        /// <summary>
        /// 获取/设置电炉的反馈通道。
        /// </summary>
        public FeedbackChannel HeaterChannel { get; set; }

        /// <summary>
        /// 获取电炉的连接状态。
        /// </summary>
        public bool HeaterConnection { get; private set; }
        /// <summary>
        /// 获取电炉的运程控制状态。
        /// </summary>
        public bool HeaterIsRemote { get; private set; }
        /// <summary>
        /// 电炉是否处于远程控制状态。
        /// </summary>
        public bool HeaterIsAuto
        {
            get
            {
                return HeaterRemoteControlChannle?.Status ?? false;
            }
        }
        /// <summary>
        /// 电炉是否处于运行状态。
        /// </summary>
        public bool HeaterIsRun
        {
            get
            {
                return HeaterRunStatusChannel?.Status ?? false;
            }
        }
        /// <summary>
        /// 获取电炉控制器名称。
        /// </summary>
        public string Caption { get; private set; }
        /// <summary>
        /// 获取电炉控制的通讯地址。
        /// </summary>
        public int Addr { get; private set; }
        #endregion

        #region Operators

        /// <summary>
        /// 在进行电炉控制之前进行对设备进行初始化。
        /// </summary>
        /// <returns>初始化成功返回True。</returns>
        public bool InitialHeater()
        {
            bool bt;
            bt = _heaterCtrl.InitCtrl(Addr);
            bt &= HeaterIsRemote = _heaterCtrl.SetComStyle(true);
            System.Threading.Thread.Sleep(10);
            bt &= _heaterCtrl.SetFixStyle(true);
            bt &= _heaterCtrl.SetRun(true);

            HeaterConnection = bt;

            if (HeaterConnectionChannel != null)
            {
                HeaterConnectionChannel.Status = bt;
            }

            HeaterStartChannel.Execute += HeaterStartStopChannel_Execute;
            HeaterStopChannel.Execute += HeaterStartStopChannel_Execute;

            return true;
        }

        /// <summary>
        /// 设置电炉需要控制的温度，如果电炉是远程控制则启动。
        /// </summary>
        /// <param name="tTemp">需要设定的控制温度。</param>
        /// <returns></returns>
        public bool SetTemperature(double tTemp)
        {
            bool bt = true;
            // 电炉准备好输出。
            if (HeaterReadyChannel?.Status == true)
            {
                double rTemp = 0;
                bt = GetCtrlTemperature(out rTemp);

                if (bt && rTemp == tTemp)
                {
                    //温度为需要设置的温度则不重新设置，进行启动。
                    if (HeaterRemoteControlChannle?.Status == true && 
                        HeaterRunStatusChannel?.Status != true && 
                        HeaterStartChannel != null)
                    {
                        HeaterStartChannel.NextStatus = true;
                        HeaterStartChannel.ControllerExecute();
                        bt &= true;
                    }
                }
                else if (bt)
                {
                    // 重新设置温度。
                    bt &= _heaterCtrl.SetTemp((float)tTemp);
                    if (!bt)
                    {
                        bt = _heaterCtrl.SetComStyle(true);
                        bt &= _heaterCtrl.SetRun(true);
                        HeaterIsRemote = bt;
                        HeaterConnection = bt;
                        if (HeaterConnectionChannel != null)
                        {
                            HeaterConnectionChannel.Status = bt;
                        }
                    }
                    if (HeaterRemoteControlChannle?.Status == true && 
                        HeaterRunStatusChannel?.Status != true && 
                        HeaterStartChannel != null)
                    {
                        HeaterStartChannel.NextStatus = true;
                        HeaterStartChannel.ControllerExecute();
                        bt &= true;
                    }
                }

            }
            else
            {
                bt = false;
            }

            return bt;
        }
        /// <summary>
        /// 获取电炉的当前设定温度。
        /// </summary>
        /// <param name="cTemp"></param>
        /// <returns></returns>
        public bool GetCtrlTemperature(out double cTemp)
        {
            cTemp = 0;
            float sv = 0;
            bool bt = _heaterCtrl.GetTemp(out sv);
            if (bt)
            {
                cTemp = sv;

                if (HeaterChannel != null)
                {
                    HeaterChannel.MeasureValue = sv;
                }
            }
            return bt;
        }

        /// <summary>
        /// 获取电炉的当前温度。
        /// </summary>
        /// <param name="cTemp"></param>
        /// <returns></returns>
        public bool GetCurrentTemperature(out double cTemp)
        {
            cTemp = 0;
            float pv = 0;
            bool bt = _heaterCtrl.GetPVData(out pv);
            if (bt)
            {
                cTemp = pv;

            }
            return bt;
        }

        /// <summary>
        /// 停止加热器加热工作。
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            bool bs = false;
            if (HeaterReadyChannel?.Status == true)
            {
                // 电炉准备好输出。
                if (HeaterRemoteControlChannle?.Status == true && 
                    HeaterRunStatusChannel?.Status == true && 
                    HeaterStopChannel != null)
                {
                    // 首先停止电炉的输出。
                    HeaterStopChannel.NextStatus = true;
                    HeaterStopChannel.ControllerExecute();

                    bs = true;
                }
            }
            return bs;
        }

        /// <summary>
        /// 释放加热器的控制资源。
        /// </summary>
        public void Dispose()
        {
            if (HeaterReadyChannel.Status)
            {
                // 首先停止电炉的输出。
                if (HeaterRemoteControlChannle?.Status == true && 
                    HeaterRunStatusChannel?.Status == true && 
                    HeaterStopChannel != null)
                {
                    HeaterStopChannel.NextStatus = true;
                    HeaterStopChannel.ControllerExecute();
                }
                System.Threading.Thread.Sleep(5);
            }
            _heaterCtrl.ReleaseCtrl();
        }

        private void HeaterStartStopChannel_Execute(object sender, ControllerEventArgs e)
        {
            var ssCh = sender as LabElement.StatusOutputChannel;
            if (ssCh != null)
            {
                var exe = ssCh.Controller as SimplePulseExecuter;
                if (exe != null)
                {
                    exe.ExecuteBegin();
                }
            }
        }

        #endregion
    }
}
