using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Execute;
using mcLogic;
using LabMCESystem.LabElement;
using System.Diagnostics;

namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 环散系统风机变频器控制器
    /// 风机可以通过选择进行PLC 模拟量控制变频面频率，也可以通过rs485通讯方式控制频率
    /// 对一冷实现段入口流量的控制是通过风机进行变频控制
    /// 根据风机和变频器的滞后性可以设置PID参数
    /// Ts = 2s Kp = 0.8 Ti = 3s Td = 0
    /// 变频器到风机转速的转换关系：。。。
    /// 在此进行风机的状态，数据进行操作 风机的数据采集。
    /// </summary>
    class HS_FirstColdFanDevice : PredicatePositionPID
    {
        public HS_FirstColdFanDevice(string designMark) : base(0, new SafeRange(0, 3000), new PIDParam() { Ts = 2000, Kp = 0.8, Td = 0, Ti = 3000 })
        {
            DesignMark = designMark;

            ExecuteChanged += HS_FirstColdFanDevice_ExecuteChanged;

            UpdateFedback += HS_FirstColdFanDevice_UpdateFedback;

            ExecuteOvered += HS_FirstColdFanDevice_ExecuteOvered;
        }

        #region Properties

        /// <summary>
        /// 获取/设置风机PCL频率设定通道通道。
        /// </summary>
        public FeedbackChannel FanFrequencePlcChannel { get; set; }
        /// <summary>
        /// 获取/设置风机启动通道。
        /// </summary>
        public StatusOutputChannel FanStartChannel { get; set; }
        /// <summary>
        /// 获取/设置风机停止通道。
        /// </summary>
        public StatusOutputChannel FanStopChannel { get; set; }

        /// <summary>
        /// 获取/设置风机的远程连接通道。
        /// </summary>
        public StatusChannel FanConnectionChannel { set; get; }
        /// <summary>
        /// 风机已准备好状态通道。
        /// </summary>
        public StatusChannel FanReadyChannel { get; set; }

        /// <summary>
        /// 获取/设置风机是否通道COM 485通讯进行频率控制，否则通道PLC模拟量控制变频器频率。
        /// </summary>
        public bool IsCOMControlMode { get; set; } = true;

        /// <summary>
        /// 获取风机的转速。
        /// </summary>
        public double RevolvingSpeed { get; private set; }

        private string _comport;
        /// <summary>
        /// 获取/设置风机控制器的串行端口名称。
        /// </summary>
        public string COMPort
        {
            get { return _comport; }
            set { _comport = value; }
        }
        
        #endregion

        // 在此更新风机的转速状态。
        private void HS_FirstColdFanDevice_UpdateFedback(IDataFeedback sender)
        {
            sender.FedbackData = ExecuteVal;
        }
        
        // 使用 控制风机设备。
        private void HS_FirstColdFanDevice_ExecuteChanged(object sender, double executedVal)
        {
            // 设置风机频率。
            if (IsCOMControlMode)
            {

            }
            else
            {
                Debug.Assert(FanFrequencePlcChannel != null);
                FanFrequencePlcChannel.AOValue = executedVal;
                (FanFrequencePlcChannel.Controller as Executer)?.ExecuteBegin();
            }
            //
            Debug.Assert(FanStartChannel != null);
            var exe = FanStartChannel.Controller as SimplePulseExecuter;
            Debug.Assert(exe != null);
            exe.ExecuteBegin();
        }

        private void HS_FirstColdFanDevice_ExecuteOvered(object obj)
        {
            Debug.Assert(FanStopChannel != null);
            var exe = FanStopChannel.Controller as SimplePulseExecuter;
            Debug.Assert(exe != null);
            exe.ExecuteBegin();
            // 重置风机频率。
            if (IsCOMControlMode)
            {

            }
            else
            {
                Debug.Assert(FanFrequencePlcChannel != null);
                
                (FanFrequencePlcChannel.Controller as Executer)?.ExecuteOver();
            }
        }

        // 需要实时地采集获取风机的运行状态。
        private void UpdateFanStatus()
        {

        }
    }
}
