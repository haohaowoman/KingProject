using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
using mcLogic;
using mcLogic.Execute;

namespace LabMCESystem.Servers.HS.HS_Executers
{
    class FirstColdFIRQExecuter : PredicatePositionPID
    {
        public FirstColdFIRQExecuter(double targetVal, SafeRange srange) : base(targetVal, srange, new PIDParam()
        {
            Ts = 5 * 60 * 1000,
            Kp = 0.88,
            Ti = 20 * 60 * 1000
        }
        )
        {
            UpdateFedback += FirstColdFIRQExecuter_UpdateFedback;
            ExecuteChanged += FirstColdFIRQExecuter_ExecuteChanged;
            ExecuteOvered += FirstColdFIRQExecuter_ExecuteOvered;
        }

        /// <summary>
        /// 风机控制通道。
        /// </summary>
        public FeedbackChannel FanDevChannel { get; set; }
        /// <summary>
        /// 流量反馈通道。
        /// </summary>
        public FeedbackChannel FIRQChannel { get; set; }

        #region Operators

        private void FirstColdFIRQExecuter_ExecuteOvered(object obj)
        {
            Debug.Assert(FanDevChannel != null);
            FanDevChannel.AOValue = 0;
            FanDevChannel.StopControllerExecute();
        }

        private void FirstColdFIRQExecuter_ExecuteChanged(object sender, double executedVal)
        {
            Debug.Assert(FanDevChannel != null);
            // 风机输出流量对应的频率。
            FanDevChannel.AOValue = Math.Min(executedVal * 2500 / 20200.0, FanDevChannel.Range.Height);

            FanDevChannel.ControllerExecute();
        }

        private void FirstColdFIRQExecuter_UpdateFedback(IDataFeedback sender)
        {
            Debug.Assert(FIRQChannel != null);
            sender.FedbackData = FIRQChannel.MeasureValue;
        }

        #endregion

    }
}
