using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// Class of element label changed event arguments.
    /// </summary>
    public class NotifyElementLabelChangedEventArgs : EventArgs
    {
        public NotifyElementLabelChangedEventArgs(bool bChanged, string newLabel, string oldLabel)
        {
            IsChanged = bChanged;
            NewLabel = newLabel;
            OldLabel = oldLabel;
        }

        public NotifyElementLabelChangedEventArgs(bool bChanged, string newLabel):this(bChanged, newLabel, null)
        {

        }

        public NotifyElementLabelChangedEventArgs(bool bChanged):this(bChanged, null)
        {

        }

        public NotifyElementLabelChangedEventArgs():this(false)
        {

        }

        /// <summary>
        /// Is it label changed?
        /// </summary>
        public bool IsChanged { get; private set; }

        /// <summary>
        /// Get element's old label.
        /// </summary>
        public string OldLabel { get; private set; }

        /// <summary>
        /// Get element's new label.
        /// </summary>
        public string NewLabel { get; private set; }

        
    }

    /// <summary>
    /// Class of lab base element.
    /// </summary>
    [Serializable]
    public abstract class LabElement
    {
        private string _label;
        /// <summary>
        /// Get/set this label.
        /// </summary>
        [XmlAttribute(AttributeName = "Name")]
        public string Label
        {
            get { return _label; }
            set
            {
                NotifyElementLabelChangedEventArgs e;
                if (value != _label)
                {
                    bool? bc = ElementLabelChanging?.Invoke(this, value);

                    if (bc == true || bc == null)
                    {
                        e = new NotifyElementLabelChangedEventArgs(true, value, _label);
                        _label = value;
                    }
                    else
                    {
                        e = new NotifyElementLabelChangedEventArgs(false, value);
                    }
                }
                else
                {
                    e = new NotifyElementLabelChangedEventArgs();
                }

                NotifyElementLabelChanged?.Invoke(this, e);
            }
        }
        #region Build

        public LabElement()
        {

        }

        public LabElement(string label)
        {
            Label = label;
        }

        #endregion

        #region Override

        /// <summary>
        /// Override ToString.
        /// </summary>
        /// <returns>This label.</returns>
        public override string ToString()
        {
            return _label;
        }

        public override int GetHashCode()
        {
            if (_label == null)
            {
                return base.GetHashCode();
            }
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            LabElement tempObj = obj as LabElement;

            if (object.ReferenceEquals(tempObj, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this, tempObj))
            {
                return true;
            }
            else
            {
                // 
                return GetHashCode() == obj.GetHashCode();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Label have being changed event.
        /// </summary>
        public event Action<LabElement, NotifyElementLabelChangedEventArgs> NotifyElementLabelChanged;

        /// <summary>
        /// Label is channging.If you don't want to change it return false.
        /// </summary>
        public event Func<LabElement, string, bool> ElementLabelChanging;

        #endregion
    }

}
