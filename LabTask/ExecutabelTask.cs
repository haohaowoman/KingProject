using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Task
{
    /// <summary>
    /// 可执行的任务类，包含基本任务信息，其它试验任务的基类
    /// </summary>
    public class ExecutabelTask
    {
        #region Properties

        /// <summary>
        /// 获取/设置任务设定项集合
        /// </summary>
        public List<TaskSetter> Setters { get; set; }

        /// <summary>
        /// 获取/设备任务结果项集合
        /// </summary>
        public List<TaskResult> Results { get; set; }


        /// <summary>
        /// 获取任务单号
        /// </summary>
        public string TaskNumber { get; private set; }


        #endregion


    }
}
