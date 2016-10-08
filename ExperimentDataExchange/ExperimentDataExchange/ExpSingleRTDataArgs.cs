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

        public float RTValue { get; set; }

        public ExpSingleRTDataArgs()
        {
            RefreshTime = DateTime.Now;
        }

        public ExpSingleRTDataArgs(int chKeyCode)
        {
            ChKeyCode = chKeyCode;
        }

        public ExpSingleRTDataArgs(int chKeyCode, float value)
        {
            RefreshTime = DateTime.Now;
            ChKeyCode = chKeyCode;
            RTValue = value;
        }
    }

    public class ExpMulRTDataArgs : ExpSingleRTDataArgs
    {
        public float[] RTValues { set; get; }

        public ExpMulRTDataArgs()
        {

        }

        public ExpMulRTDataArgs(int chKeyCode) : base(chKeyCode)
        {

        }
    }
}
