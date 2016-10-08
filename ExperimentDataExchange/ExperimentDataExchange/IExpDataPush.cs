using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService.ExperimentDataExchange
{
    public interface IExpDataPush
    {
        void Push(ExpSingleRTDataArgs e);

        void Push(ExpSingleRTDataArgs[] es);

        void MultiplePush(ExpMulRTDataArgs em);

        void MultiplePush(ExpMulRTDataArgs[] ems);
    }
}
