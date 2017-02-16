using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Execute;
using mcLogic;
using LabMCESystem.LabElement;

namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 环散电动调节阀执行器
    /// </summary>
    class HS_EOVPIDExecuter : ClosedLoopExecuter/*PredicatePositionPID*/
    {
        public HS_EOVPIDExecuter(string designMark, double targetVal = 50, IAnalogueMeasure efChannel = null) : base(targetVal, new SafeRange(0, 100))
        {
            DesignMark = designMark;
            AllowTolerance = new Tolerance(2);
            UpdateFedback += HS_EOVPIDExecuter_UpdateFedback;
            EovFeedbackChannel = efChannel;
        }

        public LabElement.IAnalogueMeasure EovFeedbackChannel { get; set; }

        ///// <summary>
        ///// 指定电动调节阀的设计标记创建执行器
        ///// 使用默认的安全范围0~100，PID 参数设置为 Kp = 0.6, Ti = 0.5, Td = 0
        ///// 执行周期设置为T = 2s
        ///// 自动完成默认为True
        ///// 如果有的需要 可以为阀门添加执行条件
        ///// 将阀的控制公差设置为+-1%
        ///// </summary>
        ///// <param name="designMark"></param>
        ///// <param name="targetVal"></param>
        //public HS_EOVPIDExecuter(string designMark, double targetVal = 50) : base(targetVal, new SafeRange(0, 100), new PIDParam() { Kp = 0.8, Ti = 0, Td = 0, Ts = 2000 })
        //{
        //    DesignMark = designMark;
        //    AllowTolerance = new Tolerance(2);
        //    AutoFinish = true;
        //}

        /// <summary>
        /// 获取/设置阀门的管道直径。
        /// 单位毫米。
        /// </summary>
        public double PipeDiameter { get; set; } = 100;

        protected override bool OnExecute(ref double eVal)
        {
            base.OnExecute(ref eVal);
            eVal = TargetVal;
            return true;
        }

        private void HS_EOVPIDExecuter_UpdateFedback(IDataFeedback sender)
        {
            if (EovFeedbackChannel != null)
            {
                sender.FedbackData = EovFeedbackChannel.MeasureValue;
            }
        }
    }
}
