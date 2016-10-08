using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.DataContract
{
    [Serializable]
    public class SingleLinearCalibrater : SensorCalibraterBase
    {
        public float Scale { get; set; } = 1f;

        public float Offset { get; set; } = 0;

        public SingleLinearCalibrater()
        {
            
        }

        public SingleLinearCalibrater(float scale, float offset)
        {
            Scale = scale;
            Offset = offset;
        }
                
        public override float Calibrate(float srcValue)
        {
            return srcValue * Scale + Offset;
        }
    }
}
