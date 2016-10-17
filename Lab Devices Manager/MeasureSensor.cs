using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 传感器信息类型
    /// </summary>
    [Serializable]
    public enum SensorSignalType
    {
        // 模拟量信号
        Analogue,
        // 离散量信号
        Digital
    }

    /// <summary>
    /// Measure sensor should connect with a channel that measurement.
    /// </summary>
    [Serializable]
    public class MeasureSensor : LabElement
    {
        public string SensorNumber { get; set; }

        public string Remarks { get; set; }

        public SensorSignalType SignalType { get; set; }

        public override string ToString()
        {
            return $"{Label}/{SensorNumber}";
        }
    }
}
