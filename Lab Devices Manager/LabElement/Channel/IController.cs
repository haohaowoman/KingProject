using System;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 控制器执行控制操作事件参数。
    /// </summary>
    public class ControllerEventArgs : EventArgs
    {
        public object ExecuteValue { get; private set; }

        public ControllerEventArgs()
        {

        }

        public ControllerEventArgs(object eval)
        {
            ExecuteValue = eval;
        }
    }

    /// <summary>
    /// 控制器执行控制操作事件委托。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">事件参数。</param>
    public delegate void ControllerExecuteEventHandler(object sender, ControllerEventArgs e);

    /// <summary>
    /// 表示包含控制器的接口。
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// 获取/设置控制器对象。
        /// </summary>
        object Controller { get; set; }

        /// <summary>
        /// 获取/设置控制值。
        /// </summary>
        object ControlValue { get; set; }

        /// <summary>
        /// 设置了控制值时发生。
        /// </summary>
        event Action<IController> SettedControlValue;

        /// <summary>
        /// 通知控制器开始执行控制操作。
        /// </summary>
        void ControllerExecute();

        /// <summary>
        /// 调用ControllerExecute后发生。
        /// </summary>
        event ControllerExecuteEventHandler Execute;
    }
}