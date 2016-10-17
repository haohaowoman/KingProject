using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Runtime.Serialization;
namespace LabMCESystem.LabElement
{
    /// <summary>
    /// Abstract class of should be grouped sub elemnet.
    /// </summary>
    [Serializable]
    public abstract class LabGroupSubElement : LabElement, IBeGrouped, IComparable<LabGroupSubElement>
    {
        #region Builds

        public LabGroupSubElement()
        {
            ElementLabelChanging += LabGroupSubElement_ElementLabelChanging;
        }
        
        #endregion

        #region Properties
        [NonSerialized]
        private LabElement _labGroup;
        /// <summary>
        /// 获取/设置子元素的组
        /// </summary>
        public LabElement LabGroup
        {
            get { return _labGroup; }
            set
            {
                if (!object.ReferenceEquals(_labGroup, value))
                {
                    LabElement temp = _labGroup;
                    _labGroup = value;
                    GroupChanged?.Invoke(this, new NotifyGroupObjectChangedEventArgs(value, temp));
                }
                
            }
        }

        #endregion

        #region Operations

        // IComparable interface.
        public int CompareTo(LabGroupSubElement other)
        {
            return Label.CompareTo(other.Label);
        }

        
        #endregion

        #region Event

        /// <summary>
        /// This group object changed event.
        /// </summary>
        public event NotifyGroupObjectChangedEventHandler GroupChanged;


        private bool LabGroupSubElement_ElementLabelChanging(LabElement sender, string newLabel)
        {
            bool canChange = true;
            LabGroupElement<LabGroupSubElement> g = LabGroup as LabGroupElement<LabGroupSubElement>;

            if (g != null && g.GetElementAsLabel(newLabel) != null)
            {
                // if this is in group.
                // if group cantains a element with newLabel.
                canChange = false;                
            }
            return canChange;
        }

        #endregion

        #region Override

        public override string ToString()
        {
            if (LabGroup != null)
            {
                return $"{LabGroup.ToString()}-{Label}";
            }
            return base.ToString();
        }
        #endregion
    }
}
