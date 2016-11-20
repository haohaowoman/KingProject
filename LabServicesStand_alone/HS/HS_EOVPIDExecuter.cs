using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.Logic.Execute;
namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 环散电动调节阀PID执行器
    /// </summary>
    class HS_EOVPIDExecuter : PredicatePositionPID
    {
        /// <summary>
        /// 指定电动调节阀的设计标记创建执行器
        /// 使用默认的安全范围0~100，PID 参数设置为 Kp = 0.6, Ti = 0.5, Td = 0
        /// 执行周期设置为T = 0.5s
        /// 自动完成默认为True
        /// 如果有的需要 可以为阀门添加执行条件
        /// 将阀的控制公差设置为+-1%
        /// </summary>
        /// <param name="designMark"></param>
        /// <param name="targetVal"></param>
        public HS_EOVPIDExecuter(string designMark, double targetVal = 50) : base(targetVal, new SafeRange(0, 100), new PIDParam() { Kp = 0.6, Ti = 0.5, Td = 0, Ts = 500.0 })
        {
            DesignMark = designMark;
            AllowTolerance = new Tolerance(1);
            AutoFinish = true;
        }

        /// <summary>
        /// 获取/设置阀门的管道直径。
        /// 单位毫米。
        /// </summary>
        public double PipeDiameter { get; set; } = 100;

    }
}
