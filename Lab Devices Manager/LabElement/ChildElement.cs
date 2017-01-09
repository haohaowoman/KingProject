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
    /// 只能由组对象创建的元素抽象基类。
    /// 子元素类必须从此类继承。
    /// 并且在组中不能出现Label属性相同的元素。
    /// </summary>
    [Serializable]
    public abstract class ChildElement : LabElement, IBelongtoGroup, IComparable<ChildElement>
    {
        #region Builds
        /// <summary>
        /// 必须通过指定元素的组来创建子元素对象。
        /// </summary>
        /// <param name="group">元素的组。</param>
        /// <exception cref="ArgumentNullException">通道不接受空的拥有者设备对象。</exception>
        public ChildElement(LabElement group, string label) : base(label)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group), "子元素对象不接受空的组对象。");
            }

            Group = group;

            ElementLabelChanging += Element_ElementLabelChanging;
        }

        #endregion

        #region Properties
        [NonSerialized]
        private LabElement _labGroup;
        /// <summary>
        /// 获取/设置子元素的组
        /// </summary>
        public LabElement Group
        {
            get { return _labGroup; }
            protected set
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
        public int CompareTo(ChildElement other)
        {
            return Label.CompareTo(other.Label);
        }


        #endregion

        #region Event

        /// <summary>
        /// This group object changed event.
        /// </summary>
        public event NotifyGroupObjectChangedEventHandler GroupChanged;


        private bool Element_ElementLabelChanging(LabElement sender, string newLabel)
        {
            bool canChange = true;
            GroupElement<ChildElement> g = Group as GroupElement<ChildElement>;

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
            if (Group != null)
            {
                return $"{Group.ToString()}#{Label}";
            }
            return base.ToString();
        }
                
        #endregion
    }
}
