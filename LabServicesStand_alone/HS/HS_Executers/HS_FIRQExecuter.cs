using mcLogic;
using mcLogic.Execute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 实验入口流量PID控制调节。
    /// 根据阀门调节与组合调节的滞后性设定PID参数。
    /// Ts = 4s Kp = 0.78 Ti = 6s Td = 0
    /// 安全范围在0 - 5000kg / h
    /// 公差范围在+-50
    /// 根据电炉需求的最低流量做控制限制。
    /// 不自动停步。
    /// 通过递推算法计算流量对应的组合阀开度。
    /// </summary>
    class HS_FIRQExecuter : PredicatePositionPID
    {
        public HS_FIRQExecuter(string designMark, HS_MeasCtrlDevice dev, double minQ) :
            base(200, new SafeRange(0, 5000), new PIDParam()
            {
                Ts = 4000,
                Kp = 0.78,
                Ti = 6000,
                Td = 0
            })
        {
            AllowTolerance = new Tolerance(50);
            AutoFinish = false;

            MinRequirQ = minQ;

            ExecuteChanged += HS_FIRQExecuter_ExecuteChanged;
        }

        /// <summary>
        /// 获取/设置最低要求流量。
        /// </summary>
        public double MinRequirQ { get; set; }

        private HS_MultipelEOVExecuter _mEOVExe;
        /// <summary>
        /// 获取/设置组合控制阀组合。
        /// </summary>
        public HS_MultipelEOVExecuter MulEOVExecuter
        {
            get { return _mEOVExe; }
            set { _mEOVExe = value; }
        }

        /// <summary>
        /// 获取当前流量对应开度的比例系数。
        /// </summary>
        public double QKParam { get; private set; } = 1;

        // 执行输出。
        private void HS_FIRQExecuter_ExecuteChanged(object sender, double executedVal)
        {
            if (_mEOVExe != null)
            {
                executedVal = Math.Max(MinRequirQ, ExecuteVal);
                double Keov = executedVal / QKParam;
                if (_mEOVExe.FedbackData != 0)
                {
                    QKParam = FedbackData / _mEOVExe.FedbackData;
                }
                if (_mEOVExe.SafeRange.IsSafeIn(Keov))
                {
                    _mEOVExe.TargetVal = Keov;
                }

                _mEOVExe.ExecuteBegin();
            }            
        }

    }
}
