using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 异常触发器触发事件参数。
    /// </summary>
    public class ExceptTriggeredEventArgs : EventArgs
    {
        public ExceptTriggeredEventArgs(ExcepTrigger et, object src = null)
        {
            Trigger = et;
            Source = src;
        }

        /// <summary>
        /// 获取产生事件的触发器。
        /// </summary>
        public ExcepTrigger Trigger { get; private set; }

        /// <summary>
        /// 获取/设置事件的触发源。
        /// </summary>
        public object Source { get; set; }

    }
    
    /// <summary>
    /// 定义异常触发器的基类。
    /// </summary>
    public abstract class ExcepTrigger
    {
        /// <summary>
        /// 确定当前触发器是否处于激活状态。
        /// </summary>
        public abstract bool IsAction { get; }

        /// <summary>
        /// 触发，当满足触发条件时发生。
        /// </summary>
        public event EventHandler<ExceptTriggeredEventArgs> Triggered;

        /// <summary>
        /// 尝试触发。
        /// </summary>
        protected virtual void OnTrigger(object src = null)
        {
            if (IsAction)
            {
                Triggered?.Invoke(this, new ExceptTriggeredEventArgs(this, src));
            }
        }
    }
}
