using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 定义通道异常触发器的基类。
    /// </summary>
    public class ChannelTrigger : ExcepTrigger
    {
        public ChannelTrigger()
        {

        }

        public ChannelTrigger(Channel ch)
        {
            ActionChannel = ch;
        }

        /// <summary>
        /// 确定当前触发器是否处于激活状态。
        /// </summary>
        public override bool IsAction
        {
            get
            {
                if (ActionChannel != null)
                {
                    return ActionValue == ActionChannel.Value;
                }
                else
                {
                    return false;
                }
            }
        }

        private Channel _actionChannel;
        /// <summary>
        /// 获取/设置激活触发器的通道。
        /// </summary>
        public Channel ActionChannel
        {
            get { return _actionChannel; }
            set
            {
                if (_actionChannel != null)
                {
                    value.NotifyValueUpdated -= ActionChannel_NotifyValueUpdated;
                }
                if (value != null)
                {
                    value.NotifyValueUpdated += ActionChannel_NotifyValueUpdated;
                }
                _actionChannel = value;
            }
        }

        private void ActionChannel_NotifyValueUpdated(object sender, object e)
        {
            OnTrigger(ActionChannel);
        }

        /// <summary>
        /// 获取/设置激活值。
        /// </summary>
        public object ActionValue { get; set; }
    }
}
