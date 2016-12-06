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
        public LabChannel OldChannel { get; private set; }

        public LabChannel NewChannel { get; private set; }

        public NotifyPairedChannelChangedEventArgs(LabChannel newChannel, LabChannel oldChannel)
        {
            NewChannel = newChannel;

            OldChannel = oldChannel;
        }

        public NotifyPairedChannelChangedEventArgs(LabChannel newChannel) : this(newChannel, null)
        {

        }
    }

    /// <summary>
    /// Class of lab's experiment point. 
    /// </summary>
    [Serializable]
    public class ExperimentPoint : LabGroupSubElement, IUnitRange
    {
        #region Fields Properties

        private string _unit;
        /// <summary>
        /// 获取/设置试验点的工程量单位
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        // The keyCode of the pairred channel.
        private int _pairedChannelKeyCode;
        /// <summary>
        /// Get this paired channel's key code.
        /// </summary>
        public int PairedChannelKeyCode
        {
            get { return _pairedChannelKeyCode; }
        }


        [NonSerialized]
        private LabChannel _pairedChannel;
        /// <summary>
        /// Get/set this paired channel.
        /// </summary>
        public LabChannel PairedChannel
        {
            get { return _pairedChannel; }
            set
            {
                LabChannel temp = _pairedChannel;

                if (value == null)
                {

                    _pairedChannelKeyCode = 0;
                }
                else
                {
                    _pairedChannelKeyCode = value.KeyCode;
                }

                _pairedChannel = value;
                PairedChannelChanged?.Invoke(this, new NotifyPairedChannelChangedEventArgs(value, temp));

                // regist ChannelKeyCodeChanged event of new paired channel.
                if (value != null)
                {
                    // 如果测试点单位为空 则从配对通道处更新。
                    if (Unit == null)
                    {
                        Unit = value.Unit;
                    }
                    // 如果范围无效 则从配对通道处更新。
                    if (Range.IsInvalid)
                    {
                        Range = value.Range;
                    }

                    value.ChannelKeyCodeChanged += Value_ChannelKeyCodeChanged;
                }

                // remove ChannelKeyCodeChanged event of old paired channel.
                if (temp != null)
                {
                    temp.ChannelKeyCodeChanged -= Value_ChannelKeyCodeChanged;
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

        public event Action<object, NotifyPairedChannelChangedEventArgs> PairedChannelChanged;

        // paired channel key code changed event callback.
        private void Value_ChannelKeyCodeChanged(LabChannel sender, ChannelKeyCodeChangedEventArgs e)
        {
            //event sourcess must be same instance with paired channel.
            if (object.ReferenceEquals(sender, _pairedChannel))
            {
                _pairedChannelKeyCode = e.NewKeyCode;
            }
        }

        #endregion

        #region Operators

        public ExperimentPoint()
        {
        }

        public ExperimentPoint(string label)
        {
            Label = label;

        }

        #endregion

        #region Override

        public override string ToString()
        {
            return $"{LabGroup}-{Label}";
        }

        #endregion

    }

}
