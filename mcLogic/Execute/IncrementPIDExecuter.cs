using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Execute
{
    /// <summary>
    /// PID增量式算法执行器
    /// 是增量是输出PID，适合有步进关系的输出控制，如步进电机
    /// 由于增量式算法与位置式算法的区别，进行优化控制
    /// </summary>
    public class IncrementPIDExecuter : PIDExecuterBase
    {
        public IncrementPIDExecuter(double targetVal, SafeRange srange) : base(targetVal, srange)
        {
        }

        public IncrementPIDExecuter(double targetVal, SafeRange srange, PIDParam param) : base(targetVal, srange, param)
        {
        }

        #region Override

        protected override bool OnExecute(ref double eVal)
        {
            // 对倒数第二次控制误差进行记录
            if (ExecuteTCount >= 2)
            {
                ELastTwo = ELastOne;
            }
            return base.OnExecute(ref eVal); ;
        }

        protected override void OnPIDExecute()
        {

        }

        protected override bool OnPIDMath(ref double eVal)
        {
            double q0, q1, q2;
            Param.GetIncrementPIDParam(out q0, out q1, out q2);
            eVal = q0 * ECurrent + q1 * ELastOne + q2 * ELastTwo;
            return true;
        }

        public override void Reset()
        {
            ELastTwo = 0;
            base.Reset();
        }
        #endregion

        /// <summary>
        /// 获取倒数第二次执行控制误差
        /// </summary>
        public double ELastTwo { get; private set; }

    }
}
