using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.BaseService.ExperimentDataExchange;
namespace LabMCESystem.DataContract
{
    [Serializable]
    public abstract class ExpValueConverterBase : IExpValueConvert
    {
        public Type SourceType
        {
            get;
            set;
        }

        public Type TargetType
        {
            get;
            set;
        }

        public string SourceUnit
        {
            get;
            set;
        }

        public string TargetUnit
        {
            get;
            set;
        }

        public abstract object Convert(object value);

        public static IExpValueConvert Defualt { set; get; } = null;
    }
}
