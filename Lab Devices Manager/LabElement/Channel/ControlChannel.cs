using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 表示可控制通道的抽象类。
    /// </summary>
    [Serializable]
    public abstract class ControlChannel : Channel, IController
    {
        /// <summary>
        /// 创建对象时自动加上ExperimentStyle.Control方式，使其成为可控制对象。
        /// </summary>
        /// <param name="owner">通道所属的设备对象。</param>
        /// <param name="style">通道试验方式。</param>
        /// <exception cref="ArgumentNullException">通道不接受空的拥有者设备对象。</exception>        
        internal ControlChannel(LabDevice owner, string label, ExperimentStyle style) : base(owner, label, style | ExperimentStyle.Control)
        {

        }

        #region Properties

        private object _controller;

        /// <summary>
        /// 获取/设置可控制通道的控制器。
        /// </summary>
        public object Controller
        {
            get { return _controller; }
            set { _controller = value; }
        }

        private object _controlValue;
        /// <summary>
        /// 获取/设置控制值。
        /// 如果派生类的控制值是其它数值类型需要重写该属性封装器。
        /// </summary>
        public virtual object ControlValue
        {
            get
            {
                return _controlValue;
            }
            set
            {
                _controlValue = value;
                SettedControlValue?.Invoke(this);
            }
        }

        /// <summary>
        /// 设置有效控制值后发生。
        /// </summary>
        public event Action<IController> SettedControlValue;

        /// <summary>
        /// 调用ControllerExecute后发生。
        /// </summary>
        public event ControllerExecuteEventHandler Execute;
        /// <summary>
        /// 调用StopControllerExecute后发生。
        /// </summary>
        public event ControllerExecuteEventHandler StopExecute;

        #endregion

        #region Operators
        /// <summary>
        /// 虚函数实现通道控制器执行控制操作。
        /// 派生类重载此函数需要最后调用基类ControllerExecute函数。
        /// </summary>
        public virtual void ControllerExecute()
        {
            Execute?.Invoke(this, new ControllerEventArgs(ControlValue));
        }
        /// <summary>
        /// 通知控制器停止控制操作。
        /// </summary>
        public void StopControllerExecute()
        {
            StopExecute?.Invoke(this, new ControllerEventArgs(ControlValue));
        }

        #endregion
    }
}
