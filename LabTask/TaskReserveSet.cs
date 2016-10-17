using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Task
{
    /// <summary>
    /// 任务备选集
    /// </summary>
    public class TaskReserveSet
    {
        /// <summary>
        /// 获取/设置任务说明
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 获取/设置任务列表
        /// </summary>
        public List<ExecutabelTask> Tasks { get; set; }

        /// <summary>
        /// 获取该任务备选集中的所有任务设定项列表
        /// </summary>
        public List<Setter> Setters
        {
            get
            {
                List<Setter> tempSetters = new List<Setter>();
                foreach (var t in Tasks)
                {
                    tempSetters.AddRange(t.Setters);
                }
                return tempSetters;
            }
        }


    }
}
