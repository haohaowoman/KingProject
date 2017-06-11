using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 定义通道的抽象类。
    /// </summary>
    [Serializable]
    public abstract class Channel : ChildElement, IComparable<Channel>
    {
        /// <summary>
        /// 创建通道时需要为其指定所属的设备对角，以及通道试验方式。
        /// </summary>
        /// <param name="owner">通道所属的设备对象。</param>
        /// <param name="style">通道试验方式。</param>
        /// <exception cref="ArgumentNullException">通道不接受空的拥有者设备对象。</exception>
        internal Channel(LabDevice owner, string label, ExperimentStyle style) : base(owner, label)
        {
            if (owner == null)
            {
                throw new ArgumentNullException(nameof(OwnerDevice), "通道不接受空的拥有者设备对象。");
            }
            Style = style;
        }
        
        #region Override

        public override string ToString()
        {
            return $"{OwnerDevice}#{Label}";
        }

        public int CompareTo(Channel other)
        {
            // 首要比较为Style。
            if (this.Style != other.Style)
            {
                return Style > other.Style ? 1 : -1;
            }
            else
            {
                // 次要比较为Label属性。
                return Label.CompareTo(other.Label);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 获取/设置通道的数据。
        /// </summary>
        public abstract object Value { set; get; }

        /// <summary>
        /// 获取/设置通道所属的设备。
        /// 是 Group属性的转换封装。
        /// </summary>         
        public LabDevice OwnerDevice { get { return Group as LabDevice; } }

        /// <summary>
        /// 获取通道的试验方式。
        /// </summary>
        public ExperimentStyle Style { get; private set; }

        private string _prompt;
        private LabDevice owner;
        private ExperimentStyle experimentStyle;

        /// <summary>
        /// 获取/设置通道在设备上的快速提示信息。
        /// </summary>
        [XmlAnyAttribute]
        public string Prompt
        {
            get { return _prompt; }
            set { _prompt = value; }
        }

        /// <summary>
        /// 当通道的数据值发生改变时发生。
        /// </summary>
        public event EventHandler<object> NotifyValueUpdated;
        #endregion

        #region Operators
        
        /// <summary>
        /// 派生类调用此方法以触发NotifyValueUpdated事件。
        /// </summary>
        protected void NotifyValueUpdate()
        {
            NotifyValueUpdated?.Invoke(this, Value);
        }
        
        #endregion
    }
}
