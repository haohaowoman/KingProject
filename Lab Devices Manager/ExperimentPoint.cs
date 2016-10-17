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

        public NotifyPairedChannelChangedEventArgs(LabChannel newChannel):this(newChannel, null)
        {

        }
    }

    /// <summary>
    /// Class of lab's experiment point. 
    /// </summary>
    [Serializable]
    public class ExperimentPoint : LabGroupSubElement
    {
        #region Fields Properties

        /// <summary>
        /// 获取/设置试验点类型
        /// </summary>
        public ExperimentWorkStyle WorkType { get; set; }

        private string _units;
        /// <summary>
        /// 获取/设置试验点的工程量单位
        /// </summary>
        public string Units
        {
            get { return _units; }
            set { _units = value; }
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
                    value.ChannelKeyCodeChanged += Value_ChannelKeyCodeChanged;
                }

                // remove ChannelKeyCodeChanged event of old paired channel.
                if (temp != null)
                {
                    temp.ChannelKeyCodeChanged -= Value_ChannelKeyCodeChanged;                    
                }
            }
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
            WorkType = ExperimentWorkStyle.Measure;
        }

        public ExperimentPoint(string label, ExperimentWorkStyle workStyle)
        {
            Label = label;
            WorkType = workStyle;
        }

        public ExperimentPoint(ExperimentArea owner, ExperimentWorkStyle workType = ExperimentWorkStyle.Measure)
        {
            LabGroup = owner;
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
