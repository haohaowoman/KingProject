using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Execute
{
    /// <summary>
    /// 简单的无中间逻辑的执行器。
    /// </summary>
    public class NoLogicExecuter : Executer
    {        
        public NoLogicExecuter(double targetVal, SafeRange srange) : base(targetVal, srange)
        {

        }

        public NoLogicExecuter(SafeRange srange) :this(0, srange)
        {

        }

        protected override bool OnExecute(ref double eVal)
        {
            eVal = TargetVal;
            return true;
        }
    }
}
