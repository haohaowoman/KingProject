using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.BaseService.ExperimentDataExchange;

namespace LabMCESystem.DataContract
{
    public abstract class SensorCalibraterBase : ISensorCalibrate
    {
        public static ISensorCalibrate Defualt { protected set; get; }

        public abstract float Calibrate(float srcValue);
    }

    internal class DefualtSensorCalibrater : SensorCalibraterBase
    {
        public override float Calibrate(float srcValue)
        {
            return srcValue;
        }

        static DefualtSensorCalibrater()
        {
            Defualt = new DefualtSensorCalibrater();
        }
    }
}
