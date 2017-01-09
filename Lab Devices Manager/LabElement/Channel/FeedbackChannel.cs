using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 表示带反馈的模拟量可输出通道，实现IAnalogeMeasure与IAnalogueOutput接口。
    /// </summary>
    [Serializable]
    public class FeedbackChannel : Channel, IFeedback
    {
        /// <summary>
        /// 创建带反馈的模拟量输出控制通道。
        /// </summary>
        /// <param name="owner">通道所属的设备对象。</param>
        /// <param name="label">通道的Label属性。</param>
        /// <exception cref="ArgumentNullException">通道不接受空的拥有者设备对象。</exception>
        internal FeedbackChannel(LabDevice owner, string label) : base(owner, label, ExperimentStyle.Feedback)
        {

        }

        #region Override

        /// <summary>
        /// 重写，获取/设置通道数据，为MeasureValue的简单封装。
        /// </summary>
        public override object Value
        {
            get
            {
                return MeasureValue;
            }

            set
            {
                MeasureValue = (double)value;
            }
        }

        #endregion

        #region IAnalogueMeasure，IAnalogueOutput

        public event Action<object, MesureCollectorChangedEventArgs> CollectorChanged;
        /// <summary>
        /// 模拟量输数值发生改变时发生。
        /// </summary>
        public event Action<IAnalogueOutput> AOValueChanged;
        /// <summary>
        /// 设置了控制值时发生。
        /// </summary>
        public event Action<IController> SettedControlValue;
        /// <summary>
        /// 当ControllerExecute时发生，通知控制器执行控制。
        /// </summary>
        public event ControllerExecuteEventHandler Execute;

        private object _collector;
        /// <summary>
        /// 获取/设置采集器对象。
        /// </summary>
        public object Collector
        {
            get
            {
                return _collector;
            }

            set
            {
                object old = _collector;
                _collector = value;
                CollectorChanged?.Invoke(this, new MesureCollectorChangedEventArgs(_collector, old));
            }
        }

        /// <summary>
        /// 获取/设置采集的采样率。
        /// </summary>
        public int Frequence
        {
            get;
            set;
        }

        private double _measureValue;
        /// <summary>
        /// 获取/设置采集数据值。
        /// </summary>
        public double MeasureValue
        {
            get { return _measureValue; }
            set
            {
                _measureValue = value;
                NotifyValueUpdate();
            }
        }

        /// <summary>
        /// 获取/设置量纲单位。
        /// </summary>
        public QRange Range
        {
            get;
            set;
        }

        /// <summary>
        /// 获取/设置量纲范围。
        /// </summary>
        public string Unit
        {
            get;
            set;
        }

        private double _aoValue;
        /// <summary>
        /// 获取/设置模拟量输出的数值。
        /// </summary>
        public double AOValue
        {
            get
            {
                return _aoValue;
            }

            set
            {
                double old = _aoValue;
                if (old != value)
                {
                    _aoValue = value;
                    AOValueChanged?.Invoke(this);

                    SettedControlValue?.Invoke(this);
                }
            }
        }

        /// <summary>
        /// 获取/设置控制器对象。
        /// </summary>
        public object Controller
        {
            get;
            set;
        }

        /// <summary>
        /// 获取/设置控制值。
        /// </summary>
        public object ControlValue
        {
            get
            {
                return _aoValue;
            }

            set
            {
                AOValue = (double)value;
            }
        }

        /// <summary>
        /// 虚函数，通知控制器开始执行控制操作。
        /// </summary>
        public virtual void ControllerExecute()
        {
            Execute?.Invoke(this, new ControllerEventArgs(ControlValue));
        }

        #endregion

    }

}
