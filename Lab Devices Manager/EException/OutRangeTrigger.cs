using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
namespace LabMCESystem.EException
{
    /// <summary>
    /// 定义可检测超出范围的触发器。
    /// </summary>
    public sealed class OutRangeTrigger : ChannelTrigger
    {
        public OutRangeTrigger()
        {

        }

        /// <summary>
        /// 指定实现了IAnalogueMeasure的Channel创建。
        /// </summary>
        /// <param name="amc">实现了IAnalogueMeasure的通道对象。</param>
        /// <exception cref="ArgumentException">需要实现IAnalogueMeasure的Channel对象。</exception>
        public OutRangeTrigger(IAnalogueMeasure amc)
        {
            _inRange = amc.Range;
            ActionChannel = amc as Channel;
            if (ActionChannel == null)
            {
                throw new ArgumentException("需要实现IAnalogueMeasure的Channel对象。", nameof(amc));
            }
        }

        private QRange _inRange;
        /// <summary>
        /// 获取/设置不触发的范围。
        /// </summary>
        public QRange InRange
        {
            get { return _inRange; }
            set { _inRange = value; }
        }

        public override bool IsAction
        {
            get
            {
                IAnalogueMeasure am = ActionChannel as IAnalogueMeasure;
                if (am != null)
                {
                    return !_inRange.SureInRange(am.MeasureValue);
                }
                else
                {
                    return false;
                }                
            }
        }
    }
}
