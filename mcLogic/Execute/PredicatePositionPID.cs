using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Execute
{
    /// <summary>
    /// 条件先行的位置PID算法执行器
    /// 实现IPredicateExecute
    /// </summary>
    public class PredicatePositionPID : PositionPIDExecuter, IPredicateExecute
    {
        public PredicatePositionPID(double targetVal, SafeRange srange) : base(targetVal, srange)
        {
        }

        public PredicatePositionPID(double targetVal, SafeRange srange, PIDParam param) : base(targetVal, srange, param)
        {

        }

        public ExecutePredicateEventHandler ExecutePredicate
        {
            get;
            set;            
        }

        protected override bool OnExecute(ref double eVal)
        {
            bool br = true;

            if (ExecutePredicate != null)
            {
                br = ExecutePredicate(this, ref eVal);
                // 如果条件未通过，则对周期的计数-1，让条件先行但不影响在一个PID周期内的周期运行次数。
                if (!br)
                {
                    double periodTime = PeriodCount * PeriodInterval;
                    //如果达到控制执行周期，对周期计数-1。
                    if (periodTime >= Param.Ts)
                    {
                        PeriodCount--;
                    }
                }
            }

            return br && base.OnExecute(ref eVal);
        }
    }
}
