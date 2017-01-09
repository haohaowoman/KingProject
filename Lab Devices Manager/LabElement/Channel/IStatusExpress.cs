using System;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 可表示状态的接口。
    /// </summary>
    public interface IStatusExpress
    {
        bool Status { get; set; }
    }

    /// <summary>
    /// 可进行状态控制的接口。
    /// </summary>
    public interface IStatusController : IController
    {
        bool NextStatus { get; set; }

        event Action<IStatusController> SettedNextStatus;
    }
}