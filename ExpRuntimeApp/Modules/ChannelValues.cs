using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem;
using LabMCESystem.LabElement;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;

namespace ExpRuntimeApp.Modules
{
    /// <summary>
    /// UI通道模块，实现通道信息的界面更新。
    /// </summary>
    public class MdChannel : INotifyPropertyChanged,
        IAnalogueMeasure,
        IAnalogueOutput,
        IStatusExpress,
        IStatusController
    {
        public MdChannel(Channel channel)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("Channel", "通道数据不接受为null的通道对象。");
            }
            Channel = channel;
        }

        #region Properties

        public virtual event PropertyChangedEventHandler PropertyChanged;

        public event Action<object, MesureCollectorChangedEventArgs> CollectorChanged;
        public event Action<IAnalogueOutput> AOValueChanged;
        public event Action<IController> SettedControlValue;
        public event ControllerExecuteEventHandler Execute;
        public event Action<IStatusController> SettedNextStatus;

        private Channel _channel;

        public Channel Channel
        {
            get { return _channel; }
            set
            {
                if (_channel != value)
                {
                    Channel old = _channel;

                    // 移除通道事件。
                    if (old != null)
                    {
                        old.NotifyElementLabelChanged -= NotifyElementLabelChanged;
                        old.NotifyValueUpdated -= NotifyValueUpdated;
                    }

                    _channel = value;

                    // 增加通道事件
                    if (_channel != null)
                    {
                        _channel.NotifyElementLabelChanged += NotifyElementLabelChanged;
                        _channel.NotifyValueUpdated += NotifyValueUpdated;
                    }

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Channel"));
                }
            }
        }

        public IAnalogueMeasure AsAIChannel
        {
            get { return _channel as IAnalogueMeasure; }
        }

        public IAnalogueOutput AsAOChannel
        {
            get { return _channel as IAnalogueOutput; }
        }

        public IStatusExpress AsStatusChannel
        {
            get { return _channel as IStatusExpress; }
        }

        public IController AsController
        {
            get { return _channel as IController; }
        }

        public IStatusController AsStatusController
        {
            get { return _channel as IStatusController; }
        }


        #region 实现与Channel相同的属性代码。

        public virtual string Label
        {
            set
            {
                Channel.Label = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            }
            get
            {
                return Channel.Label;
            }
        }

        public string Summary
        {
            set
            {
                Channel.Summary = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Summary"));
            }
            get
            {
                return Channel.Summary;
            }
        }

        public ExperimentStyle Style
        {
            get { return _channel.Style; }
        }

        public LabDevice OwnerDevice
        {
            get { return _channel.OwnerDevice; }
        }

        public object Value
        {
            get { return _channel?.Value; }
        }

        public string Prompt
        {
            set
            {
                _channel.Prompt = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Prompt"));
            }
            get
            {
                return _channel.Prompt;
            }
        }
        #endregion

        public int Frequence
        {
            get
            {
                return AsAIChannel?.Frequence ?? 1;
            }

            set
            {
                AsAIChannel.Frequence = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Frequence"));
            }
        }

        public double MeasureValue
        {
            get
            {
                return AsAIChannel?.MeasureValue ?? 0;
            }

            set
            {
                AsAIChannel.MeasureValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MeasureValue"));
            }
        }

        public object Collector
        {
            get
            {
                return AsAIChannel?.Collector;
            }

            set
            {
                AsAIChannel.Collector = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Collector"));
            }
        }
        
        public string Unit
        {
            get
            {
                return AsAIChannel?.Unit;
            }

            set
            {
                AsAIChannel.Unit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Unit"));
            }
        }

        public QRange Range
        {
            get
            {
                return AsAIChannel?.Range ?? new QRange();
            }

            set
            {
                AsAIChannel.Range = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Range"));
            }
        }

        public double RangeLow
        {
            get
            {
                return AsAIChannel?.Range.Low ?? 0;
            }
            set
            {
                AsAIChannel.Range = new QRange(value, AsAIChannel.Range.Height);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Range"));
            }
        }
        
        public double RangeHigh
        {
            get
            {
                return AsAIChannel?.Range.Height ?? 0;
            }
            set
            {
                AsAIChannel.Range = new QRange(AsAIChannel.Range.Low, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Range"));
            }
        }
        
        public double AOValue
        {
            get
            {
                return AsAOChannel?.AOValue ?? 0;
            }

            set
            {
                AsAOChannel.AOValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AOValue"));
            }
        }

        public object Controller
        {
            get
            {
                return AsAOChannel?.Controller;
            }

            set
            {
                AsAOChannel.Controller = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Controller"));
            }
        }

        public object ControlValue
        {
            get
            {
                return AsAOChannel?.ControlValue;
            }

            set
            {
                AsAOChannel.ControlValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ControlValue"));
            }
        }

        public bool Status
        {
            get
            {
                return AsStatusChannel?.Status ?? false;
            }

            set
            {
                AsStatusChannel.Status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
        }

        public bool NextStatus
        {
            get
            {
                return AsStatusController?.NextStatus ?? false;
            }

            set
            {
                AsStatusController.NextStatus = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NextStatus"));
            }
        }

        #endregion

        #region Operators

        public override string ToString()
        {
            return Channel.Label;
        }

        public void ControllerExecute()
        {
            AsController?.ControllerExecute();
        }

        private void NotifyValueUpdated(object obj, object e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            if (_channel as IStatusExpress != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            }
            else if (_channel as IAnalogueMeasure != null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MeasureValue"));
            }
        }

        private void NotifyElementLabelChanged(LabElement sender, NotifyElementLabelChangedEventArgs arg)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
        }

        /// <summary>
        /// 通知所有UI更新所有属性。
        /// </summary>
        public virtual void UpdataProperties()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Channel"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Label"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Summary"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Style"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Prompt"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MeasureValue"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Frequence"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Collector"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AOValue"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Status"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NextStatus"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ControlValue"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Controller"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Unit"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Range"));

        }

        protected void NotifyPorpertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("propertyName"));
        }
        #endregion
    }

    public class MdExperPoint : MdChannel
    {
        public MdExperPoint(ExperimentalPoint point) : base(point.PairedChannel)
        {
            if (point == null)
            {
                throw new ArgumentNullException("ExperPoint", "测试点数据不接受为null的测试点对象。");
            }
            ExperPoint = point;
        }

        private ExperimentalPoint _exeprPoint;
        /// <summary>
        /// 获取/设备相关联的测试点
        /// </summary>
        public ExperimentalPoint ExperPoint
        {
            get { return _exeprPoint; }
            set
            {
                ExperimentalPoint old = _exeprPoint;
                if (old != null)
                {
                    _exeprPoint.NotifyElementLabelChanged -= NotifyElementLabelChanged;
                    _exeprPoint.PairedChannelChanged -= PairedChannelChanged;
                }
                _exeprPoint = value;

                if (_exeprPoint != null)
                {
                    _exeprPoint.NotifyElementLabelChanged += NotifyElementLabelChanged;
                    _exeprPoint.PairedChannelChanged += PairedChannelChanged;
                }
                NotifyPorpertyChanged("ExperPoint");
            }
        }

        #region 实现与ExperimentalPoint相同的属性代码。

        public override string Label
        {
            set
            {
                ExperPoint.Label = value;
                NotifyPorpertyChanged("Label");
            }
            get
            {
                return ExperPoint.Label;
            }
        }

        public new string Summary
        {
            set
            {
                ExperPoint.Summary = value;
                NotifyPorpertyChanged("Summary");
            }
            get
            {
                return ExperPoint.Summary;
            }
        }

        public ExperimentalArea Area
        {
            get { return ExperPoint.Area; }
        }


        public new string Unit
        {
            get
            {
                return ExperPoint.Unit;
            }

            set
            {
                ExperPoint.Unit = value;
                NotifyPorpertyChanged("Unit");
            }
        }

        public new QRange Range
        {
            get
            {
                return ExperPoint.Range;
            }

            set
            {
                ExperPoint.Range = value;
                NotifyPorpertyChanged("Range");
            }
        }


        public new double RangeLow
        {
            get
            {
                return _exeprPoint?.Range.Low ?? 0;
            }
            set
            {
                _exeprPoint.Range = new QRange(value, _exeprPoint.Range.Height);
                NotifyPorpertyChanged("Range");
            }
        }

        public new double RangeHigh
        {
            get
            {
                return _exeprPoint?.Range.Height ?? 0;
            }
            set
            {
                _exeprPoint.Range = new QRange(_exeprPoint.Range.Low, value);
                NotifyPorpertyChanged("Range");
            }
        }


        public Channel PairedChannel
        {
            set
            {
                _exeprPoint.PairedChannel = value;
                NotifyPorpertyChanged("PairedChannel");
            }
            get
            {
                return _exeprPoint.PairedChannel;
            }
        }

        #endregion

        #region Operators

        private void PairedChannelChanged(object arg1, NotifyPairedChannelChangedEventArgs arg2)
        {
            Channel = arg2.NewChannel;
            NotifyPorpertyChanged("Channel");
        }

        private void NotifyElementLabelChanged(LabElement sender, NotifyElementLabelChangedEventArgs arg)
        {
            NotifyPorpertyChanged("Label");
        }

        public override void UpdataProperties()
        {
            NotifyPorpertyChanged("ExperPoint");
            NotifyPorpertyChanged("Area");
            base.UpdataProperties();
        }

        #endregion
    }

    /// <summary>
    /// 提供了通道Label属性索引元素的功能。
    /// </summary>
    public class MdChannelsCollection : 
        IList,
        IList<MdChannel>, 
        ICollection, 
        ICollection<MdChannel>,
        INotifyCollectionChanged
    {

        public MdChannelsCollection()
        {
            _mdChannels = new ObservableCollection<MdChannel>();
            _mdChannels.CollectionChanged += _mdChannels_CollectionChanged;
        }

        private void _mdChannels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private ObservableCollection<MdChannel> _mdChannels;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count
        {
            get
            {
                return ((IList<MdChannel>)_mdChannels).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<MdChannel>)_mdChannels).IsReadOnly;
            }
        }

        public object SyncRoot
        {
            get
            {
                return ((ICollection)_mdChannels).SyncRoot;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)_mdChannels).IsSynchronized;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return ((IList)_mdChannels).IsFixedSize;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return ((IList)_mdChannels)[index];
            }

            set
            {
                ((IList)_mdChannels)[index] = value;
            }
        }

        public MdChannel this[int index]
        {
            get
            {
                return ((IList<MdChannel>)_mdChannels)[index];
            }

            set
            {
                ((IList<MdChannel>)_mdChannels)[index] = value;
            }
        }

        public MdChannel this[string label]
        {
            get
            {
                MdChannel ch = null;
                foreach (var item in _mdChannels)
                {
                    if (item.Label == label)
                    {
                        ch = item;
                        break;
                    }
                }
                return ch;
            }
        }

        public int IndexOf(MdChannel item)
        {
            return ((IList<MdChannel>)_mdChannels).IndexOf(item);
        }

        public void Insert(int index, MdChannel item)
        {
            ((IList<MdChannel>)_mdChannels).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<MdChannel>)_mdChannels).RemoveAt(index);
        }

        public void Add(MdChannel item)
        {
            ((IList<MdChannel>)_mdChannels).Add(item);
        }

        public void Clear()
        {
            ((IList<MdChannel>)_mdChannels).Clear();
        }

        public bool Contains(MdChannel item)
        {
            return ((IList<MdChannel>)_mdChannels).Contains(item);
        }

        public void CopyTo(MdChannel[] array, int arrayIndex)
        {
            ((IList<MdChannel>)_mdChannels).CopyTo(array, arrayIndex);
        }

        public bool Remove(MdChannel item)
        {
            return ((IList<MdChannel>)_mdChannels).Remove(item);
        }

        public IEnumerator<MdChannel> GetEnumerator()
        {
            return ((IList<MdChannel>)_mdChannels).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<MdChannel>)_mdChannels).GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_mdChannels).CopyTo(array, index);
        }

        public int Add(object value)
        {
            return ((IList)_mdChannels).Add(value);
        }

        public bool Contains(object value)
        {
            return ((IList)_mdChannels).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList)_mdChannels).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            ((IList)_mdChannels).Insert(index, value);
        }

        public void Remove(object value)
        {
            ((IList)_mdChannels).Remove(value);
        }
    }
}
