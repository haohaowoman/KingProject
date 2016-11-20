using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Logic.Execute
{
    /// <summary>
    /// 定义可附加条件执行控制的方法
    /// 只有在满足所给条件时执行器才能输出数据
    /// </summary>
    public interface IPredicateExecute
    {
        /// <summary>
        /// 获取/设置执行器的执行条件的委托
        /// </summary>
        ExecutePredicateEventHandler ExecutePredicate { set; get; }
    }

    /// <summary>
    /// 有条件执行器的条件委托
    /// </summary>
    /// <param name="excuter">执行器</param>
    /// <param name="val">将要执行的输出值</param>
    /// <returns>是否满足条件</returns>
    public delegate bool ExecutePredicateEventHandler(object excuter, ref double val);
}
