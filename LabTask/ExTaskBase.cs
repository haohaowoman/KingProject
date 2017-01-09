using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.ETask
{
    /// <summary>
    /// 任务执行状态
    /// </summary>
    [Serializable]
    public enum TaskState
    {
        // 在执行
        Executing,
        // 等待
        Waited,
        // 被中断
        Interrupted,
        // 挂起
        Suspended,
        // 成功完结
        Succeed
    }

    /// <summary>
    /// 可执行任务的基类
    /// </summary>
    [Serializable]
    public class ExTaskBase
    {
        private List<TaskSetter> _setters;

        /// <summary>
        /// 获取/设置任务的设定项列表
        /// </summary>
        public List<TaskSetter> Setters
        {
            get { return _setters; }
            set { _setters = value; }
        }

        /// <summary>
        /// 获取任务的创建时间
        /// </summary>
        public DateTime BuildTime { get; private set; } = DateTime.Now;

        // 任务编号
        // 任务编号生产方式：长时间+线程ID+Index
        private string _number;
        /// <summary>
        /// 获取任务编号
        /// </summary>
        public string Number
        {
            get { return _number; }
            private set { _number = value; }
        }


        private TaskState _state;
        /// <summary>
        /// 获取/设置任务当前的执行状态及阶段
        /// </summary>
        public TaskState State
        {
            get { return _state; }
            set { _state = value; }
        }

    }
}
