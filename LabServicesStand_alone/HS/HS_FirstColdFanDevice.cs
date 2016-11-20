using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.Logic.Execute;
namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 环散系统风机变频器控制器
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
        }

        #region Properties

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
        private void HS_FirstColdFanDevice_UpdateFedback(IDataFedback sender)
        {
            
        }
        
        // 使用 控制风机设备。
        private void HS_FirstColdFanDevice_ExecuteChanged(object sender, double executedVal)
        {
            
        }

        // 需要实时地采集获取风机的运行状态。
        private void UpdateFanStatus()
        {

        }
    }
}
