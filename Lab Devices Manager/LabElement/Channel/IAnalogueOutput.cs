using System;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 表示可模拟量输出的通道。
    /// </summary>
    public interface IAnalogueOutput : IController
    {
        /// <summary>
        /// 获取/设置模拟量输出的数值。
        /// </summary>
        double AOValue { get; set; }

        /// <summary>
        /// 模拟量输数值发生改变时发生。
        /// </summary>
        event Action<IAnalogueOutput> AOValueChanged;
    }
}