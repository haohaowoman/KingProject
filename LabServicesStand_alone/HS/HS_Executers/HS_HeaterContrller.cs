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
        }

        #region Properties

        /// <summary>
        /// 电炉通讯控制。
        /// </summary>
        private FP23Ctrl _heaterCtrl;

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
            bt = _heaterCtrl.InitCtrl();
            HeaterIsRemote = _heaterCtrl.SetComStyle(true);
            HeaterConnection = bt;

            if (HeaterConnectionChannel != null)
            {
                HeaterConnectionChannel.Status = bt;
            }            

            return true;
        }

        /// <summary>
        /// 设置电炉需要控制的温度，如果电炉是远程控制则启动。
        /// </summary>
        /// <param name="tTemp">需要设定的控制温度。</param>
        /// <returns></returns>
        public bool SetTemperature(double tTemp)
        {
            bool bt = _heaterCtrl.SetTemp((float)tTemp);

            if (HeaterReadyChannel?.Status == true)
            {
                // 首先停止电炉的输出。
                HeaterStopChannel.NextStatus = true;
                HeaterStopChannel.ControllerExecute();
                System.Threading.Thread.Sleep(5);
            }
            if (HeaterStartChannel != null)
            {
                HeaterStartChannel.NextStatus = true;
                HeaterStartChannel.ControllerExecute();
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

                if (HeaterChannel != null)
                {
                    HeaterChannel.MeasureValue = pv;
                }
            }
            return bt;
        }

        /// <summary>
        /// 释放加热器的控制资源。
        /// </summary>
        public void Dispose()
        {
            if (HeaterReadyChannel.Status)
            {
                // 首先停止电炉的输出。
                HeaterStopChannel.NextStatus = true;
                HeaterStopChannel.ControllerExecute();
                System.Threading.Thread.Sleep(5);
            }
            _heaterCtrl.ReleaseCtrl();
        }

        #endregion
    }
}
