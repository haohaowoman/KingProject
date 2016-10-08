using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService.ExperimentDataExchange
{
    public struct ExpDataExchangeLoadArgs
    {
        public int ChKeyCode { get; set; }

        public DateTime FisrtUpdateTime { get; set; }

        public DateTime CurRefreshTime { get; set; }

        public int ReceiveCount { get; set; }

        public int RefreshCount { get; set; }
    }

    public interface IExpDataExchangeLoad
    {
        List<ExpDataExchangeLoadArgs> CurrentLoad { get; }

        ExpDataExchangeLoadArgs GetOneLoad(int chKeyCode);
    }
}
