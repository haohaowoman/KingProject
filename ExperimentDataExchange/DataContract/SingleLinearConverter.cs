using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.DataContract
{
    [Serializable]
    public class SingleLinearConverter : ExpValueConverterBase
    {
        public float Scale { get; set; }

        public float Offset { get; set; }

        public SingleLinearConverter()
        {

        }

        public SingleLinearConverter(float scale, float offset)
        {
            Scale = scale;
            Offset = offset;
        }

        public override object Convert(object value)
        {
            float temp = (float)value;

            temp = temp * Scale + Offset;

            return temp;
        }
    }
}
