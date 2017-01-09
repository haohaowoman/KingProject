using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 表示可模拟量输出控制的控制通道类。
    /// </summary>
    [Serializable]
    public class AnalogueOutputChannel : ControlChannel, IAnalogueOutput
    {
        /// <summary>
        /// 创建模拟量输出控制通道。
        /// </summary>
        /// <param name="owner">通道所属的设备对象。</param>
        /// <param name="label">通道的Label属性。</param>
        /// <exception cref="ArgumentNullException">通道不接受空的拥有者设备对象。</exception>
        internal AnalogueOutputChannel(LabDevice owner, string label) : base(owner, label, ExperimentStyle.Control)
        {

        }

        private double _aoValue;
        /// <summary>
        /// 获取/设置模拟量输出数值。
        /// </summary>
        public double AOValue
        {
            get
            {
                return _aoValue;
            }

            set
            {
                if (value != _aoValue)
                {
                    AOValueChanged?.Invoke(this);
                }
                _aoValue = value;
            }
        }

        /// <summary>
        /// 重写属性虚函数，
        /// 获取/设置控制值对象，当value不为null时并将同步对AOValue属性进行更新。
        /// </summary>
        public override object ControlValue
        {
            get
            {
                return AOValue;
            }

            set
            {
                base.ControlValue = value;
                if (value != null)
                {
                    AOValue = (double)value;
                }                
            }
        }

        /// <summary>
        /// 重写数值属性，
        /// 获取/设置通道数据。与ControlValue相同。
        /// </summary>
        public override object Value
        {
            get
            {
                return ControlValue;
            }

            set
            {
                ControlValue = value;
                NotifyValueUpdate();
            }
        }

        /// <summary>
        /// 当AOValue属性发生改变时发生。
        /// </summary>
        public event Action<IAnalogueOutput> AOValueChanged;
    }
}
