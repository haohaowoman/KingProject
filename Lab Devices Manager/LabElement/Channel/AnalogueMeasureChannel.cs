using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// Contains channel's key code changed event arguments.
    /// </summary>
    [Serializable]
    public class ChannelKeyCodeChangedEventArgs : EventArgs
    {
        public int OldKeyCode { get; private set; }
        public int NewKeyCode { get; private set; }

        public ChannelKeyCodeChangedEventArgs(int newCode, int oldCode)
        {
            NewKeyCode = newCode;
            OldKeyCode = oldCode;
        }
    }

    /// <summary>
    /// 采集通道的采集器改变事件参数。
    /// </summary>
    [Serializable]
    public class MesureCollectorChangedEventArgs : EventArgs
    {
        public object OldCollector { get; private set; }
        public object NewCollector { get; private set; }

        public MesureCollectorChangedEventArgs(object newSensor, object oldSensor)
        {
            NewCollector = newSensor;
            OldCollector = oldSensor;
        }
        public MesureCollectorChangedEventArgs(object newSensor) : this(newSensor, null)
        {

        }
    }

    /// <summary>
    /// 表示实验室设备中的特定通道对象。
    /// </summary>
    [Serializable]
    public class AnalogueMeasureChannel : Channel, IUnitRange, IAnalogueMeasure
    {

        #region Build

        internal AnalogueMeasureChannel(LabDevice owner, string label) : base(owner, label, ExperimentStyle.Measure)
        {

        }

        #endregion

        #region Properties
        // channel refresh frequence.
        private int _frequence = 1;
        /// <summary>
        /// 获取/设置通道的采样频率。
        /// </summary>
        public int Frequence
        {
            get { return _frequence; }
            set { _frequence = value; }
        }

        /// <summary>
        /// 获取/设置通道的工作方式。
        /// </summary>
        public ExperimentStyle WorkStyle { get; set; }
        
        private string _unit;
        /// <summary>
        /// 获取/设置能通道数据的工程量纲。
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        /// <summary>
        /// 获取/设置通道的测量或输出范围。
        /// </summary>
        public QRange Range
        {
            get;
            set;
        }

        private double _measureValue;

        public double MeasureValue
        {
            get
            {
                return _measureValue;
            }

            set
            {
                _measureValue = value;
                NotifyValueUpdate();
            }
        }

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

        private object _collector;

        public object Collector
        {
            get { return _collector; }
            set
            {
                object oldSensor = _collector;
                _collector = value;
                CollectorChanged?.Invoke(this, new MesureCollectorChangedEventArgs(value, oldSensor));
            }
            
        }

        #endregion

        #region Event

        /// <summary>
        /// Invoke this event when channel's connect sensor changed.
        /// </summary>
        public event Action<object, MesureCollectorChangedEventArgs> CollectorChanged;

        #endregion

        #region Operations

        #endregion


    }
}
