using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 定义状态通道触发器。
    /// </summary>
    public sealed class StatusChannelTrigger : ChannelTrigger
    {
        public StatusChannelTrigger()
        {

        }

        public StatusChannelTrigger(StatusChannel ch) : base(ch)
        {
        }

        public IStatusExpress StatusActionChannel { get { return ActionChannel as IStatusExpress; } }

        /// <summary>
        /// 获取/设置激活状态。
        /// </summary>
        public bool ActionStatus { get; set; } = true;

        public override bool IsAction
        {
            get
            {
                if (StatusActionChannel != null)
                {
                    return StatusActionChannel.Status == ActionStatus;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
