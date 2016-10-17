using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
namespace ExpRuntimeApp.Modules
{
    class ChannelValue : INotifyPropertyChanged
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

        private float _value;

        public float Value
        {
            get { return _value; }
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
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

    }

    class ExpPointValue : INotifyPropertyChanged
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

        /// <summary>
        /// 获取所属的实验段,为分组、过滤等提供支持
        /// </summary>
        public string ExpArea { get { return ExpPoint.LabGroup.ToString(); } }
        
        // 测试点数据
        private float _value;
        /// <summary>
        /// 获取/设备测试点数据
        /// </summary>
        public float Value
        {
            get { return _value; }
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
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
