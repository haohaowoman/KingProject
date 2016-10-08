using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    [Serializable]
    public enum SensorSignalType
    {
        Analogue, 
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
