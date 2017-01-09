using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 表示可控制的状态通道。
    /// </summary>
    [Serializable]
    public class StatusOutputChannel : ControlChannel, IStatusController, IStatusExpress
    {
        internal StatusOutputChannel(LabDevice owner, string label) : base(owner, label, ExperimentStyle.StatusControl)
        {
            // 当设置基类属性ControlValue属性时，同步改变_newStatus值。
            SettedControlValue += (c) =>
            {
                _newStatus = (bool)c.ControlValue;
                SettedNextStatus?.Invoke(this);
            };
        }

        private bool _newStatus;
        /// <summary>
        /// 获取/设置通道的新状态值。
        /// </summary>
        public bool NextStatus
        {
            get
            {
                return _newStatus;
            }

            set
            {
                _newStatus = value;
                SettedNextStatus?.Invoke(this);

                // 需要重新设置Control属性值。
                ControlValue = value;
            }
        }

        private bool _status;
        /// <summary>
        /// 获取/设置通道的实际反馈状态。
        /// </summary>
        public bool Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
                NotifyValueUpdate();
            }
        }

        /// <summary>
        /// 获取/设置通道的实际反馈状态。
        /// </summary>
        public override object Value
        {
            get
            {
                return _status;
            }

            set
            {
                _status = (bool)value;
                NotifyValueUpdate();
            }
        }

        public event Action<IStatusController> SettedNextStatus;
    }
}