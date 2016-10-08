using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// Contains channel's key code changed event arguments.
    /// </summary>
    [Serializable]
    public class ChannelKeyCodeChangedEventArgs : EventArgs
    {
        public int OldKeyCode { get; private set; }
        public int NewKeyCode { get; private set; }

        public ChannelKeyCodeChangedEventArgs(int newCode, int oldCode)
        {
            NewKeyCode = newCode;
            OldKeyCode = oldCode;
        }
    }

    [Serializable]
    public class ChannelConnectSensorChangedEventArgs : EventArgs
    {
        public MeasureSensor OldSensor { get; set; }
        public MeasureSensor NewSensor { get; set; }

        public ChannelConnectSensorChangedEventArgs(MeasureSensor newSensor, MeasureSensor oldSensor)
        {
            NewSensor = newSensor;
            OldSensor = oldSensor;
        }
        public ChannelConnectSensorChangedEventArgs(MeasureSensor newSensor) : this(newSensor, null)
        {

        }
    }

    /// <summary>
    /// Class of Device's channel.
    /// </summary>
    [Serializable]
    public class LabChannel : LabGroupSubElement
    {
        #region Properties
        // channel refresh frequence.
        private int _frequence;
        /// <summary>
        /// Set/Get channel's workable frequence.
        /// </summary>
        public int Frequence
        {
            get { return _frequence; }
            set { _frequence = value; }
        }

        /// <summary>
        /// Set/Get channel's workable style.
        /// </summary>
        public ExperimentWorkStyle WorkStyle { get; set; }

        public override int IndexInGroup
        {
            get
            {
                LabDevice dev = LabGroup as LabDevice;
                if (dev != null)
                {
                    return dev.ElementIndexOf(this);
                }
                return -1;
            }
        }

        // This channel's only code.
        // We can use it to find this channel.
        private int _keyCode;

        /// <summary>
        /// Get channel's key code that is only param .
        /// </summary>
        public int KeyCode
        {
            get
            {
                if (_keyCode == 0)
                {
                    OperChannelKeyCode();
                }
                return _keyCode;
            }
        }

        // If this channel is a measurement channel, it should connects a sensor.
        private MeasureSensor _connectSensor;

        /// <summary>
        /// Get/set channel connects a measurement sensor.
        /// </summary>
        public MeasureSensor ConnectSensor
        {
            get { return _connectSensor; }
            set { _connectSensor = value; }
        }

        #endregion

        #region Event

        /// <summary>
        /// This Channel's KeyCode changed event.
        /// </summary>
        public event Action<LabChannel, ChannelKeyCodeChangedEventArgs> ChannelKeyCodeChanged;

        /// <summary>
        /// Invoke this event when channel's connect sensor changed.
        /// </summary>
        public event Action<LabChannel, ChannelConnectSensorChangedEventArgs> ChannelConnectSensorChanged;
        
        #endregion
        #region Build

        public LabChannel(LabChannel ownedev, string label, ExperimentWorkStyle workStyle) : this()
        {
            LabGroup = ownedev;

            Label = label;

            WorkStyle = workStyle;
        }

        public LabChannel(string label, ExperimentWorkStyle workStyle) : this(null, label, workStyle)
        {

        }

        public LabChannel()
        {
            // the event while refresh this key code.

            GroupChanged += LabChannel_GroupChanged;

            NotifyElementLabelChanged += LabChannel_NotifyElementLabelChanged;
        }

        #endregion

        #region Operations

        /// <summary>
        /// Native group chenged event. Then this key code will change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabChannel_GroupChanged(LabElement sender, NotifyGroupObjectChangedEventArgs e)
        {
            if (sender is LabDevice)
            {
                // refresh this key code.
                OperChannelKeyCode();
            }
        }

        /// <summary>
        /// This key code will refresh when this label changed.
        /// </summary>
        /// <param name="sneder"></param>
        /// <param name="e"></param>
        private void LabChannel_NotifyElementLabelChanged(LabElement sender, NotifyElementLabelChangedEventArgs e)
        {
            if (object.ReferenceEquals(this, sender) && e.IsChanged)
            {
                // refresh this key code.
                OperChannelKeyCode();
            }
        }

        /// <summary>
        /// Operate the key code.
        /// </summary>
        /// <returns>channel key code.</returns>
        private int OperChannelKeyCode()
        {
            int oldCode = _keyCode;
            _keyCode = ToString().GetHashCode();

            if (oldCode != _keyCode)
            {
                ChannelKeyCodeChanged?.Invoke(this, new ChannelKeyCodeChangedEventArgs(_keyCode, oldCode));
            }
            return _keyCode;
        }

        #endregion


    }
}
