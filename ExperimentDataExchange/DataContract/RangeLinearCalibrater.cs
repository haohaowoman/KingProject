using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.DataContract
{
    [Serializable]
    public class RangeLinearCalibrater : SingleLinearCalibrater
    {
        public float RangeMin { get; set; } = 0;

        public float RangeMax { get; set; } = 1;

        public override float Calibrate(float srcValue)
        {
            if (srcValue < RangeMin || srcValue > RangeMax)
            {
                throw new ArgumentOutOfRangeException();
            }
            return base.Calibrate(srcValue);
        }

        public RangeLinearCalibrater()
        {

        }

        public RangeLinearCalibrater(float rMin, float rMax, float scale, float offset):base(scale, offset)
        {
            RangeMin = rMin;
            RangeMax = rMax;
        }
    }
}
