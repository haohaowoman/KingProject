using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.DataContract
{
    public class DiscreteSignalConverter : ExpValueConverterBase
    {
        public float Critical { get; set; }

        public override object Convert(object value)
        {
            float temp = (float)value;

            return temp < Critical;
        }
    }
}
