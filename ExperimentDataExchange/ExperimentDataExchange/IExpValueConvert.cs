using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService.ExperimentDataExchange
{
    public interface IExpValueConvert
    {
        object Convert(object value);

        Type SourceType { get; set; }

        Type TargetType { get; set; }

        string SourceUnit { get; set; }

        string TargetUnit { get; set; }
    }
}
