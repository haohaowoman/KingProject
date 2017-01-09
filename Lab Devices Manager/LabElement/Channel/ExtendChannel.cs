using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 可以由户定义的除预定义通道以外的通道。
    /// </summary>
    [Serializable]
    public abstract class ExtendChannel : Channel
    {
        public ExtendChannel(LabDevice owner, string label, ExperimentStyle style) : base(owner, label, style)
        {

        }
    }
}