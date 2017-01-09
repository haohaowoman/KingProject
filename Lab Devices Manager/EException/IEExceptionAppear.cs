using System;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 封装试验异常出现信息的方法。
    /// </summary>
    public interface IEExceptionAppear
    {
        DateTime AppearTime { get; }
        object OriginalSource { get; }
        object Position { get; }
        object Source { get; }
    }
}