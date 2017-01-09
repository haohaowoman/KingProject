using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 定义模拟量比较的触发器。
    /// </summary>
    public sealed class AnalogueChannelTrigger : ChannelTrigger
    {
        public AnalogueChannelTrigger()
        {

        }

        public AnalogueChannelTrigger(AnalogueMeasureChannel ach) : base(ach)
        {

        }

        public AnalogueChannelTrigger(FeedbackChannel fch) : base(fch)
        {

        }

        public double ActionAnalogue { get; set; }

        public IAnalogueMeasure ActionAnaChannle { get { return ActionChannel as IAnalogueMeasure; } }

        public override bool IsAction
        {
            get
            {
                if (ActionAnaChannle != null)
                {
                    return ActionAnalogue == ActionAnaChannle.MeasureValue;
                }
                else
                {
                    return false;
                }                
            }
        }
    }
}
