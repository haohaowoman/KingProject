using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 定义试验点的类型
    /// </summary>
    [Serializable]
    public enum ExperimentWorkStyle
    {
        Measure, //输入（采集）
        Control,//输出（控制）
        Both//双向
    }
}
