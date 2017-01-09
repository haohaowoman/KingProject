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
            }

            return br && base.OnExecute(ref eVal);
        }
    }
}
