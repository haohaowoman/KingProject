using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// Class of ExperimentPoint object paired channel changed event argument.
    /// </summary>
    [Serializable]
    public class NotifyPairedChannelChangedEventArgs : EventArgs
    {
        public Channel OldChannel { get; private set; }

        public Channel NewChannel { get; private set; }

        public NotifyPairedChannelChangedEventArgs(Channel newChannel, Channel oldChannel)
        {
            NewChannel = newChannel;

            OldChannel = oldChannel;
        }

        public NotifyPairedChannelChangedEventArgs(Channel newChannel) : this(newChannel, null)
        {

        }
    }

    /// <summary>
    /// Class of lab's experiment point. 
    /// </summary>
    [Serializable]
    public class ExperimentalPoint : ChildElement, IUnitRange
    {

        internal ExperimentalPoint(ExperimentalArea area, string label) : base(area, label)
        {

        }

        #region Fields Properties

        public ExperimentalArea Area { get { return Group as ExperimentalArea; } }

        private string _unit;
        /// <summary>
        /// 获取/设置试验点的工程量单位
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
        
        [NonSerialized]
        private Channel _pairedChannel;

        /// <summary>
        /// Get/set this paired channel.
        /// </summary>
        public Channel PairedChannel
        {
            get { return _pairedChannel; }
            set
            {
                Channel temp = _pairedChannel;

                _pairedChannel = value;

                PairedChannelChanged?.Invoke(this, new NotifyPairedChannelChangedEventArgs(value, temp));

                if (Unit == null || Unit == "")
                {
                    // 如果没有设置试验点的单位则根据所匹配通道的单位来实现。
                    IUnitRange ur = _pairedChannel as IUnitRange;
                    if (ur != null)
                    {
                        Unit = ur.Unit;
                    }
                }

                if (Range.IsInvalid)
                {
                    // 如果没有设置试验点的工程范围则根据所匹配通道的范围来实现。
                    IUnitRange ur = _pairedChannel as IUnitRange;
                    if (ur != null)
                    {
                        Range = ur.Range;
                    }
                }
            }
        }

        /// <summary>
        /// 获取/设置测试点的测量或输出范围。
        /// </summary>
        public QRange Range
        {
            get;
            set;
        }

        #endregion

        #region Events
        
        public event EventHandler<NotifyPairedChannelChangedEventArgs> PairedChannelChanged;

        #endregion

        #region Operators

        #endregion

    }

}
