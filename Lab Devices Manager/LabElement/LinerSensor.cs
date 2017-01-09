using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 表示模拟量或离散量信号类型 
    /// </summary>
    [Serializable]
    public enum SignalType
    {        
        /// <summary>
        /// 模拟量信号，传感器输出电流或电压值。
        /// </summary>
        Analogue,
        // 数字量信号
        /// <summary>
        /// 变送器类传感器，使用数字量通讯输出。
        /// </summary>
        Digital
    }

    /// <summary>
    /// 表示测量系统中的线性传感器对象。
    /// 传感器的输出与工程量成线性关系。
    /// </summary>
    [Serializable]
    public class LinerSensor : LabElement, IUnitRange
    {
        /// <summary>
        /// 指定传感器的电信号输出范围与工程量量纲测量范围。
        /// </summary>
        /// <param name="elecOutRange"></param>
        /// <param name="mQRange"></param>
        public LinerSensor(QRange elecOutRange, QRange mQRange)
        {
            ElectricSignalRange = elecOutRange;
            Range = mQRange;
        }

        private QRange _range;
        /// <summary>
        /// 获取/设置传感器的工程量测量表示范围。
        /// </summary>
        public QRange Range
        {
            get
            {
                return _range;
            }
            set
            {
                _range = value;
            }
        }

        /// <summary>
        /// 获取/设置传感器的电信号输出范围。
        /// </summary>
        public QRange ElectricSignalRange { get; set; }
        /// <summary>
        /// 获取/设置传感器的特定编号。
        /// </summary>
        public string SensorNumber { get; set; }
        
        /// <summary>
        /// 获取/设置传感器采集所表示的工程量量纲。
        /// </summary>
        public string Unit
        {
            get;
            set;
        }
        
        /// <summary>
        /// 将返回传感器的 标签/编号。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Label}/{SensorNumber}";
        }
    }
}
