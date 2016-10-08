using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.DataContract
{
    [Serializable]
    public class MultipleLinearCablibrater : SensorCalibraterBase
    {
        public float MRangeMin { get; set; } = 0;

        public float MRangeMax { get; set; } = 1f;

        public List<RangeLinearCalibrater> Linears { get; set; }

        public override float Calibrate(float srcValue)
        {
            if (srcValue < MRangeMin || srcValue > MRangeMax)
            {
                throw new ArgumentOutOfRangeException();
            }

            float temp = 0f;

            foreach (var lc in Linears)
            {
                try
                {
                    temp = lc.Calibrate(srcValue);
                }
                catch (ArgumentOutOfRangeException oex)
                {
                    Console.WriteLine(oex.Message);
                }
            }

            return temp;
        }
    }
}
