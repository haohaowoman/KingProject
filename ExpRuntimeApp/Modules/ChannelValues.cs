using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
namespace ExpRuntimeApp.Modules
{
    public class ChannelValue : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private LabChannel _channel;

        public LabChannel Channel
        {
            get { return _channel; }
            set
            {
                _channel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Channel"));
            }
        }

        private double _value;

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }

        private double _valueSetter;
        /// <summary>
        /// 获取/设置控制设定值
        /// </summary>
        public double ValueSetter
        {
            get { return _valueSetter; }
            set
            {
                _valueSetter = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ValueSetter"));
            }
        }

        public ChannelValue(LabChannel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("Channel", "通道数据不接受为null的通道对象。");
            }
            Channel = channel;
            channel.NotifyElementLabelChanged += (lab, args) => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Channel")); };
        }

        public override string ToString()
        {
            return Channel.Label;
        }
    }

    public class ExpPointValue : INotifyPropertyChanged
    {
        private ExperimentPoint _expPoint;

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 获取/设备相关联的测试点
        /// </summary>
        public ExperimentPoint ExpPoint
        {
            get { return _expPoint; }
            set
            {
                _expPoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpPoint"));
            }
        }

        private ChannelValue _pairedChannelValue;

        public ChannelValue PairedChannelValue
        {
            get {
                return _pairedChannelValue;
            }
            set
            {
                if (value.Channel == ExpPoint.PairedChannel)
                {
                    _pairedChannelValue = value;

                    if (_pairedChannelValue != null)
                    {
                        _pairedChannelValue.PropertyChanged += _pairedChannelValue_PropertyChanged;
                    }
                }
                else
                {
                    _pairedChannelValue = null;
                }
                
            }
        }

        private void _pairedChannelValue_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                Value = _pairedChannelValue.Value;
            }
        }

        /// <summary>
        /// 获取所属的实验段,为分组、过滤等提供支持
        /// </summary>
        public string ExpArea { get { return ExpPoint.LabGroup.ToString(); } }

        // 测试点数据
        private double _value;
        /// <summary>
        /// 获取/设备测试点数据
        /// </summary>
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }

        private double _valueSetter;
        /// <summary>
        /// 获取/设置控制设定值
        /// </summary>
        public double ValueSetter
        {
            get { return _valueSetter; }
            set
            {
                _valueSetter = value;
                if (_pairedChannelValue != null)
                {
                    _pairedChannelValue.ValueSetter = value;
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ValueSetter"));
            }
        }

        public ExpPointValue(ExperimentPoint point)
        {
            if (point == null)
            {
                throw new ArgumentNullException("ExpPoint", "测试点数据不接受为null的测试点对象。");
            }
            ExpPoint = point;
            point.NotifyElementLabelChanged += (l, args) => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpPoint")); };
        }
    }
}
