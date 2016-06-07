using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService
{
    public class RegistElementResultArgs
    {
        public bool IsSucceed { get; private set; } = false;

        public bool IsRevised { get; private set; } = false;

        public int NewCode { get; private set; } = -1;

        public int RegistedCount { get; private set; } = -1;

        public RegistElementResultArgs(bool bSucceed, bool bRevised, int code, int regCount)
        {
            IsSucceed = bSucceed;
            IsRevised = bRevised;
            NewCode = code;
            RegistedCount = regCount;
        }

        public RegistElementResultArgs()
        {
            
        }

        public RegistElementResultArgs(bool bRevised, int code, int regCount):this(true, bRevised, code, regCount)
        {

        }

        public RegistElementResultArgs(int code, int regCount):this(true, false, code, regCount)
        {
           
        }
    }
}
