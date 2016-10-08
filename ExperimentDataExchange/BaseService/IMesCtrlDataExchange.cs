using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.BaseService.ExperimentDataExchange;
namespace LabMCESystem.BaseService
{
    public interface IMesCtrlDataExchange : IExpDataExchangeLoad, IExpDataPush, IExpDataRead
    {

    }
}
