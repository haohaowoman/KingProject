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
    /// 通知LabElement的Label属性已改变的事件委托
    /// </summary>
    /// <param name="sender">LabElement 对象</param>
    /// <param name="arg">改变事件参数</param>
    public delegate void NotifyElementLabelChangedEventHandler(LabElement sender, NotifyElementLabelChangedEventArgs arg);

    /// <summary>
    /// 通知LabElement的Label改变之前的事件委托
    /// </summary>
    /// <param name="sender">LabElement 对象</param>
    /// <param name="newLabel">新设置的Label参数</param>
    /// <returns>是否满足更改要求</returns>
    public delegate bool NotifyElementLabelChangingEventHandler(LabElement sender, string newLabel);

    /// <summary>
    /// 实验室系统元素基类
    /// 代表系统中的实物与抽象信息
    /// </summary>
    [Serializable]
    public abstract class LabElement
    {
        private string _label;
        /// <summary>
        /// 获取/设置LabElement的标签。
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
                
        private string _summary;
        /// <summary>
        /// 获取/设置Element摘要说明。
        /// </summary>
        public string Summary
        {
            get { return _summary; }
            set { _summary = value; }
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
        /// LabElement标签Label已改变时发生。
        /// </summary>
        public event NotifyElementLabelChangedEventHandler NotifyElementLabelChanged;

        /// <summary>
        /// LabElement标签Label改变之前发生。
        /// </summary>
        public event NotifyElementLabelChangingEventHandler ElementLabelChanging;

        #endregion
    }

}
