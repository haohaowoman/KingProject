using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// Class of lab device.
    /// </summary>
    [Serializable]
    public class LabDevice : LabGroupElement<LabChannel>, IRegisted, IComparable<LabDevice>
    {
        #region Builds

        public LabDevice()
        {
            _isOnline = false;
        }

        public LabDevice(int regID) : this(null, regID)
        {
            
        }

        public LabDevice(string label, int regID):this()
        {
            Label = label;
            _registID = regID;
        }
        #endregion

        #region Properties

        [NonSerialized]
        private bool _isOnline;
        /// <summary>
        /// Get/set device online statues.
        /// </summary>
        public bool IsOnline
        {
            get { return _isOnline; }
            set { _isOnline = value; }
        }

        //
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
