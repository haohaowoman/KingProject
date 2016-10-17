using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Logic.Execute
{
    /// <summary>
    /// 拥有条件的执行控制器
    /// </summary>
    public class PredicateExcuter : Executer, IPredicateExecute
    {
        /// <summary>
        /// 指定条件委托创建
        /// </summary>
        /// <param name="targetVal">目标值</param>
        /// <param name="srange">安全范围</param>
        /// <param name="p">执行条件委托</param>
        public PredicateExcuter(float targetVal, SafeRange srange, ExectuePredicateEventHandler p) : base(targetVal, srange)
        {
            ExecutePredicate = p;
        }


        public ExectuePredicateEventHandler ExecutePredicate
        {
            get;
            set;
        }

        #region Override

        protected override bool OnExecute(ref float eVal)
        {
            return ExecutePredicate.Invoke(this, ref eVal);
        }

        #endregion
    }
}
