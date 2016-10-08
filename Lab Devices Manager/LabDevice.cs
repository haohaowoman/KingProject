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
    public class LabDevice : LabGroupElement<LabChannel>, IRegisted, IComparable<LabDevice>
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

        // events.
        /// <summary>
        /// Invoke this event when device state has been changed.
        /// </summary>
        public event Action<object, DeviceStateChangedEventArgs> DeviceStateChanged;       
        
        #endregion

        #region Override

        public override string ToString()
        {
            return $"{Label} # NO.{RegistID:X8}";
        }

        #endregion

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
        /// Look up LabChannel with key code.
        /// </summary>
        /// <param name="keyCode">Channel's key code.</param>
        /// <returns>Channel</returns>
        public LabChannel FindChannelAsKeyCode(int keyCode)
        {
            return _subElements.Find(o => o.KeyCode == keyCode);
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

            foreach (var item in dev.SubElements)
            {
                sb.Append("\t\t");
                sb.AppendLine(item.Label);
            }

            sb.Append("\t");
            sb.AppendLine("}");

            return dev.RegistID = sb.ToString().GetHashCode();
        }

    }
}
