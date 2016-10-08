using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Task
{
    /// <summary>
    /// 任务设置项
    /// 通过通道关键码确定所需要通道，并执行通道任务目标值
    /// </summary>
    public class TaskSetter
    {
        /// <summary>
        /// 获取/设置任务相关的通道关键码
        /// </summary>
        public int ChannelKeyCode { get; set; }

        /// <summary>
        /// 任务项通道设定目标
        /// </summary>
        public float TargetValue { get; set; }


    }

    /// <summary>
    /// 任务结果集
    /// </summary>
    public class TaskResult
    {
        /// <summary>
        /// 获取/设置任务相关的通道关键码
        /// </summary>
        public int ChannelKeyCode { get; set; }

        /// <summary>
        /// 任务项通道设定目标
        /// </summary>
        public float Value { get; set; }
    }
}
