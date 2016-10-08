using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.DataContract
{
    [Serializable]
    public abstract class CustomConverter : ExpValueConverterBase
    {
        public abstract void ReadXml(string fileName);

        public abstract void WriteXml(string fileName);
    }
}
