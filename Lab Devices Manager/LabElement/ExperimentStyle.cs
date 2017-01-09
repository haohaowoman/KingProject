using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 定义通道、试验点的工作方式。
    /// </summary>
    [Flags]
    [Serializable]
    public enum ExperimentStyle
    {
        /// <summary>
        /// 测量类型，其数据类型为double。
        /// </summary>
        Measure = 1,
        /// <summary>
        /// 控制类型，将指定为输出。
        /// </summary>
        Control,
        /// <summary>
        /// 反馈类型，输出后对其进行测量。
        /// </summary>
        Feedback,
        /// <summary>
        /// 状态类型，其数据类型为bool。
        /// </summary>
        Status,
        /// <summary>
        /// 状态控制类型，基数据类型为bool。
        /// </summary>
        StatusControl = ExperimentStyle.Control | ExperimentStyle.Status,
    }
}
