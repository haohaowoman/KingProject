using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService.ExperimentDataExchange
{
    public class ExpSingleRTDataArgs
    {
        public int ChKeyCode { get; set; }

        public DateTime RefreshTime { get; set; }

        public double RTValue { get; set; }

        public ExpSingleRTDataArgs()
        {
            RefreshTime = DateTime.Now;
        }

        public ExpSingleRTDataArgs(int chKeyCode)
        {
            ChKeyCode = chKeyCode;
        }

        public ExpSingleRTDataArgs(int chKeyCode, double value)
        {
            RefreshTime = DateTime.Now;

            ChKeyCode = chKeyCode;
            RTValue = value;
        }
    }

    public class ChannelRTData : ExpSingleRTDataArgs
    {
        public Channel Channel { get; private set; }

        public ChannelRTData(Channel ch) : base(1/*ch.KeyCode*/)
        {
            Channel = ch;
        }
    }

    public class ExpMulRTDataArgs : ExpSingleRTDataArgs
    {
        public double[] RTValues { set; get; }

        public ExpMulRTDataArgs()
        {

        }

        public ExpMulRTDataArgs(int chKeyCode) : base(chKeyCode)
        {

        }
    }
}
