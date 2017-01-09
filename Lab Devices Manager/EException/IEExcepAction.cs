using System;
using System.Collections.Generic;

namespace LabMCESystem.EException
{
    public interface IEExcepAction
    {
        int AppearCount { get; }
        IReadOnlyList<IEExceptionAppear> Appears { get; }
        EExcepDealStyle DealStyle { get; }
        DateTime DealTime { get; }
        DateTime FirstAppearTime { get; }
        bool IsActive { get; }
        IEExceptionAppear LastAppearInfo { get; }
        EExcepState State { get; }
        EExcepStatus Status { get; }
    }
}