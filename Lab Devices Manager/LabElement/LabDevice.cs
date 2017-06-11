using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// enum device state.
    /// </summary>
    [Serializable]
    [Flags]
    public enum DeviceState
    {
        Created,

        Registed,

        Connected,

        Running,

        Stopped,

        Closed,
    }

    /// <summary>
    /// Deivce state has been changed event arguments.
    /// </summary>
    [Serializable]
    public class DeviceStateChangedEventArgs : EventArgs
    {
        public DeviceState NewState { get; private set; } = DeviceState.Registed;
        public DeviceState OldState { get; private set; } = DeviceState.Created;

        public DeviceStateChangedEventArgs(DeviceState newState, DeviceState oldState)
        {
            NewState = newState;
            OldState = oldState;
        }
    }
    /// <summary>
    /// Class of lab device.
    /// </summary>
    [Serializable]
    public class LabDevice : GroupElement<Channel>, IRegisted, IComparable<LabDevice>
    {
        #region Builds

        public LabDevice()
        {
            State = DeviceState.Created;
        }

        public LabDevice(int regID) : this(null, regID)
        {

        }

        public LabDevice(string label, int regID) : this()
        {
            Label = label;
            _registID = regID;
        }
        #endregion

        #region Properties

        private DeviceState _state;
        /// <summary>
        /// Get/set the device current state.
        /// </summary>
        public DeviceState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    DeviceStateChangedEventArgs args = new DeviceStateChangedEventArgs(value, _state);

                    _state = value;

                    DeviceStateChanged?.Invoke(this, args);
                }
            }
        }

        // The only id in all of lab devices.
        private int _registID;

        /// <summary>
        /// Get device registed id.
        /// </summary>
        public int RegistID
        {
            get
            {
                return _registID;
            }
            set
            {
                _registID = value;
            }
        }

        /// <summary>
        /// 获取设备所拥有的所有AnalogueMeasureChannel模拟量采集通道列表集合。
        /// </summary>
        public List<AnalogueMeasureChannel> AIChannels
        {
            get
            {
                return new List<AnalogueMeasureChannel>
                    (
                    from ch in _children where (ch as AnalogueMeasureChannel) != null select ch as AnalogueMeasureChannel
                    );
            }
        }

        /// <summary>
        /// 获取设备所拥有的所有StatusChannels状态表示通道列表集合。
        /// </summary>
        public List<StatusChannel> StatusChannels
        {
            get
            {
                return new List<StatusChannel>
                    (
                    from ch in _children where (ch as StatusChannel) != null select ch as StatusChannel
                    );
            }
        }

        /// <summary>
        /// 获取设备所拥有的所有AnalogueOutputChannel模拟量输出通道列表集合。
        /// </summary>
        public List<AnalogueOutputChannel> AOChannels
        {
            get
            {
                return new List<AnalogueOutputChannel>
                    (
                    from ch in _children where (ch as AnalogueOutputChannel) != null select ch as AnalogueOutputChannel
                    );
            }
        }

        /// <summary>
        /// 获取设备所拥有的所有FeedbackChannel可反馈模拟量通道列表集合。
        /// </summary>
        public List<FeedbackChannel> FeedbackChannels
        {
            get
            {
                return new List<FeedbackChannel>
                    (
                    from ch in _children where (ch as FeedbackChannel) != null select ch as FeedbackChannel
                    );
            }
        }

        /// <summary>
        /// 获取设备所拥有的所有StatusOutputChannel可控制的状态通道列表集合。
        /// </summary>
        public List<StatusOutputChannel> StatusOutputChannels
        {
            get
            {
                return new List<StatusOutputChannel>
                    (
                    from ch in _children where (ch as StatusOutputChannel) != null select ch as StatusOutputChannel
                    );
            }
        }

        /// <summary>
        /// 获取设备所拥有的可控制通道集合。
        /// </summary>
        public List<IController> ControlChannels
        {
            get
            {
                return new List<IController>
                    (
                    from ch in _children where (ch as IController) != null select ch as IController
                    );
            }
        }

        /// <summary>
        /// 获取设备所包含的通道只读列表集合，是Children的简单封装。
        /// </summary>
        public IReadOnlyList<Channel> Channels
        {
            get
            {
                return Children;
            }
        }

        // events.
        /// <summary>
        /// Invoke this event when device state has been changed.
        /// </summary>
        public event Action<object, DeviceStateChangedEventArgs> DeviceStateChanged;

        #endregion

        #region Override

        public override string ToString()
        {
            return $"{Label} & NO.{RegistID:X8}";
        }

        #endregion

        #region Operators

        // LabDevice default compare is by regist id.
        public int CompareTo(LabDevice other)
        {
            if (_registID < other.RegistID)
            {
                return -1;
            }
            else if (_registID == other.RegistID)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// Rebuild the dev RegistID.
        /// </summary>
        /// <param name="dev">LabDevice</param>
        /// <returns>A new ID.</returns>
        public static int ReBuildDeviceID(LabDevice dev)
        {
            StringBuilder sb = new StringBuilder(dev.Label);
            sb.AppendLine();

            sb.Append("\t");
            sb.AppendLine("{");

            foreach (var item in dev.Children)
            {
                sb.Append("\t\t");
                sb.AppendLine(item.Label);
            }

            sb.Append("\t");
            sb.AppendLine("}");

            return dev.RegistID = sb.ToString().GetHashCode();
        }

        /// <summary>
        /// 重写，创建一个AnalogueMeasureChannel通道对象。
        /// </summary>
        /// <param name="label">为子元素指定Label属性。</param>
        /// <returns>创建成功返回子元素对象，否则返回null。</returns>
        /// <remarks>此方法只进行对象的创建，并不将其添加进行组，如要如此做请再使用group.AddElement函数。</remarks>
        public override Channel CreateChild(string label)
        {
            return new AnalogueMeasureChannel(this, label);
        }

        /// <summary>
        /// 于设备中创建一个AI通道。
        /// </summary>
        /// <param name="label">指定通道的Label属性。</param>
        /// <returns>成功返回通道对象，失败返回null。</returns>
        public AnalogueMeasureChannel CreateAIChannelIn(string label)
        {
            return (AnalogueMeasureChannel)LabDevice.CreateChannelInto(this, label, ExperimentStyle.Measure);
        }

        /// <summary>
        /// 于设备中创建一个AO通道。
        /// </summary>
        /// <param name="label">指定通道的Label属性。</param>
        /// <returns>成功返回通道对象，失败返回null。</returns>
        public AnalogueOutputChannel CreateAOChannelIn(string label)
        {
            return (AnalogueOutputChannel)LabDevice.CreateChannelInto(this, label, ExperimentStyle.Control);
        }

        /// <summary>
        /// 于设备中创建一个具有反馈的模拟量通道。
        /// </summary>
        /// <param name="label">指定通道的Label属性。</param>
        /// <returns>成功返回通道对象，失败返回null。</returns>
        public FeedbackChannel CreateFeedbackChannelIn(string label)
        {
            return (FeedbackChannel)LabDevice.CreateChannelInto(this, label, ExperimentStyle.Feedback);
        }

        /// <summary>
        /// 于设备中创建一个状态通道。
        /// </summary>
        /// <param name="label">指定通道的Label属性。</param>
        /// <returns>成功返回通道对象，失败返回null。</returns>
        public StatusChannel CreateStatusChannelIn(string label)
        {
            return (StatusChannel)LabDevice.CreateChannelInto(this, label, ExperimentStyle.Status);
        }

        /// <summary>
        /// 于设备中创建一个可控制的状态通道。
        /// </summary>
        /// <param name="label">指定通道的Label属性。</param>
        /// <returns>成功返回通道对象，失败返回null。</returns>
        public StatusOutputChannel CreateStatusOutputChannelIn(string label)
        {
            return (StatusOutputChannel)LabDevice.CreateChannelInto(this, label, ExperimentStyle.StatusControl);
        }

        /// <summary>
        /// 根据chStyel，工厂创建通道类型，并添加至gDev设备。
        /// </summary>
        /// <param name="gDev">设备对象。</param>
        /// <param name="label">通道的Label属性。</param>
        /// <param name="chStyel">通道的Sytle属性。</param>
        /// <returns>如果创建和添加成功返回通道对象，否则返回null。</returns>
        public static Channel CreateChannelInto(LabDevice gDev, string label, ExperimentStyle chStyel)
        {
            Channel ch = null;

            switch (chStyel)
            {
                case ExperimentStyle.Measure:
                    ch = new AnalogueMeasureChannel(gDev, label);
                    break;
                case ExperimentStyle.Control:
                    ch = new AnalogueOutputChannel(gDev, label);
                    break;
                case ExperimentStyle.Feedback:
                    ch = new FeedbackChannel(gDev, label);
                    break;
                case ExperimentStyle.Status:
                    ch = new StatusChannel(gDev, label);
                    break;
                case ExperimentStyle.StatusControl:
                    ch = new StatusOutputChannel(gDev, label);
                    break;
                default:

                    break;
            }

            bool ba = gDev.AddElement(ch);

            return ba ? ch : null;
        }

        /// <summary>
        /// 根据chStyel，工厂创建通道类型，但不将通道添加至gDev设备。
        /// </summary>
        /// <param name="gDev">设备对象。</param>
        /// <param name="label">通道的Label属性。</param>
        /// <param name="chStyel">通道的Sytle属性。</param>
        /// <returns>返回创建的对象。</returns>
        public static Channel CreateChannel(LabDevice gDev, string label, ExperimentStyle chStyel)
        {
            Channel ch = null;

            switch (chStyel)
            {
                case ExperimentStyle.Measure:
                    ch = new AnalogueMeasureChannel(gDev, label);
                    break;
                case ExperimentStyle.Control:
                    ch = new AnalogueOutputChannel(gDev, label);
                    break;
                case ExperimentStyle.Feedback:
                    ch = new FeedbackChannel(gDev, label);
                    break;
                case ExperimentStyle.Status:
                    ch = new StatusChannel(gDev, label);
                    break;
                case ExperimentStyle.StatusControl:
                    ch = new StatusOutputChannel(gDev, label);
                    break;
                default:

                    break;
            }
            return  ch;
        }

        #endregion
    }
}
