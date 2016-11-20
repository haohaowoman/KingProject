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

    /// <summary>
    /// 通道连接传感器的事件改变参数。
    /// </summary>
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
    /// 表示实验室设备中的特定通道对象。
    /// </summary>
    [Serializable]
    public class LabChannel : LabGroupSubElement, IUnitRange
    {
        #region Properties
        // channel refresh frequence.
        private int _frequence = 1;
        /// <summary>
        /// 获取/设置通道的采样或输出频率。
        /// </summary>
        public int Frequence
        {
            get { return _frequence; }
            set { _frequence = value; }
        }

        /// <summary>
        /// 获取/设置通道的工作方式。
        /// </summary>
        public ExperimentWorkStyle WorkStyle { get; set; }

        // This channel's only code.
        // We can use it to find this channel.
        private int _keyCode;

        /// <summary>
        /// 获取通道的关键码。
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
        /// 获取/设置通道的连接传感器。
        /// </summary>
        public MeasureSensor ConnectSensor
        {
            get { return _connectSensor; }
            set { _connectSensor = value; }
        }

        private SignalType _connectSignalType;
        /// <summary>
        /// 获取/设置通道连接信号的类型。
        /// 可能会影响到通道的ConnectSensor属性和Range属性。
        /// </summary>
        public SignalType ConnectSignalType
        {
            get { return _connectSignalType; }
            set
            {
                _connectSignalType = value;
                // 如果将信号类型设置为离散量信号 则同时修改连接传感器的各个属性。
                if (_connectSignalType == SignalType.Digital)
                {
                    Range = new QRange(0, 1);
                }
            }
        }

        private string _unit;
        /// <summary>
        /// 获取/设置能通道数据的工程量纲。
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        private string _prompt;
        /// <summary>
        /// 获取/设置通道在设备上的快速提示信息。
        /// </summary>
        public string Prompt
        {
            get { return _prompt; }
            set { _prompt = value; }
        }

        /// <summary>
        /// 获取/设置通道的测量或输出范围。
        /// </summary>
        public QRange Range
        {
            get;
            set;
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

        public LabChannel(string label, ExperimentWorkStyle workStyle = ExperimentWorkStyle.Measure) : this()
        {
            Label = label;

            WorkStyle = workStyle;
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
