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
        /// 当前加热器的设置温度值。
        /// </summary>
        private double _currentSetTemp = 0;

        /// <summary>
        /// 电炉通讯控制。
        /// </summary>
        private FP23Ctrl _heaterCtrl;

        /// <summary>
        /// 低温加热时最低要求流量为。
        /// </summary>
        static public double HeaterLowTempReMinFlow = 200;
        /// <summary>
        /// 高温加热时最低要求流量。
        /// </summary>
        static public double HeaterHighTempReMinFlow = 300;
        /// <summary>
        /// 标识加热器超过此温度时为高温加热。
        /// </summary>
        static public double WhereIsHeaterHighTemp = 250;
        /// <summary>
        /// 加热器的最小设置温度。
        /// </summary>
        static public double HeaterMinSetTemp = 100;

        private int _offComTick = 0;

        /// <summary>
        /// 每个电炉的真实最小要求流量，如果低于此值应该关闭加热器。
        /// 在低温加热时最低要求流量默认为200，高温加热时最低要求流量默认为300。
        /// </summary>
        public double HeaterTrueRequirMinFlow
        {
            get
            {
                return _currentSetTemp > WhereIsHeaterHighTemp ? HeaterHighTempReMinFlow : HeaterLowTempReMinFlow;
            }
        }

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
            lock (_heaterLocker)
            {
                bt = _heaterCtrl.InitCtrl(Addr);
                bt &= HeaterIsRemote = _heaterCtrl.SetComStyle(true);
                System.Threading.Thread.Sleep(10);
                bt &= _heaterCtrl.SetFixStyle(true);
                bt &= _heaterCtrl.SetRun(true);
            }
            HeaterConnection = bt;

            if (HeaterConnectionChannel != null)
            {
                HeaterConnectionChannel.Status = bt;
            }

            HeaterStartChannel.Execute += HeaterStartStopChannel_Execute;
            HeaterStopChannel.Execute += HeaterStartStopChannel_Execute;

            Stop();

            return true;
        }

        /// <summary>
        /// 设置电炉需要控制的温度，如果电炉是远程控制则启动。
        /// </summary>
        /// <param name="tTemp">需要设定的控制温度。</param>
        /// <returns></returns>
        public bool SetTemperature(double tTemp)
        {
            tTemp = Math.Max(tTemp, HeaterMinSetTemp);

            bool bt = true;
            // 电炉准备好输出。
            if (HeaterReadyChannel?.Status == true)
            {
                double rTemp = 0;
                bt = GetCtrlTemperature(out rTemp);

                if (bt)
                {
                    // 由于数据类型的精度误差 将误差在0.1内的视为相等。
                    double ct = rTemp - tTemp;
                    if (ct <= 0.1 && ct >= -0.1)
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
                    else
                    {
                        // 重新设置温度。
                        lock (_heaterLocker)
                        {
                            bt &= _heaterCtrl.SetTemp((float)tTemp);
                        }

                        if (bt)
                        {
                            _currentSetTemp = tTemp;

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
                }

                if (!bt)
                {
                    // set or get fault,then reset .
                    lock (_heaterLocker)
                    {
                        bt = _heaterCtrl.SetComStyle(true);
                        bt &= _heaterCtrl.SetRun(true);
                    }

                }
                UpdateHeaterConnection(bt);
            }
            else
            {
                bt = false;
            }

            return bt;
        }

        /// <summary>
        /// 更新电炉链接状态。
        /// </summary>
        /// <param name="bt"></param>
        private void UpdateHeaterConnection(bool bt)
        {
            HeaterIsRemote = bt;
            HeaterConnection = bt;
            if (HeaterConnectionChannel != null)
            {
                HeaterConnectionChannel.Status = bt;
            }
            // 连续出现20次通讯中断 报故障 停止。            
            if (!bt)
            {
                _offComTick++;
                if (_offComTick >= 30)
                {
                    Stop();
                    HeaterFualtChannel.Status = true;
                    _offComTick = 0;
                }
            }
            else
            {
                // 成功重新计数。
                _offComTick = 0;
            }
        }

        private object _heaterLocker = new object();
        /// <summary>
        /// 获取电炉的当前设定温度。
        /// </summary>
        /// <param name="cTemp"></param>
        /// <returns></returns>
        public bool GetCtrlTemperature(out double cTemp)
        {
            cTemp = 0;
            float sv = 0;
            bool bt = false;

            lock (_heaterLocker)
            {
                bt = _heaterCtrl.GetTemp(out sv);
            }

            if (bt)
            {
                cTemp = sv;

                if (HeaterChannel != null)
                {
                    HeaterChannel.MeasureValue = sv;
                }
            }
            UpdateHeaterConnection(bt);
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

            bool bt = true;
            lock (_heaterLocker)
            {
                bt = _heaterCtrl.GetPVData(out pv);
            }
            if (bt)
            {
                cTemp = pv;
            }
            UpdateHeaterConnection(bt);
            return bt;
        }

        /// <summary>
        /// 停止加热器加热工作。
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            bool bs = false;
            System.Diagnostics.Debug.Assert(HeaterRunStatusChannel != null);

            if (HeaterReadyChannel?.Status == true && HeaterRunStatusChannel.Status)
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
            lock (_heaterLocker)
            {
                _heaterCtrl.ReleaseCtrl();
            }
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
