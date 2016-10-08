using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService.ExperimentDataExchange
{
    public interface ISingleExpRTDataRead
    {
        void Read(ExpSingleRTDataArgs e);

        void Read(ExpSingleRTDataArgs[] es);
    }
}
