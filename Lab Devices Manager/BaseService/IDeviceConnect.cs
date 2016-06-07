using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService
{
    public interface IDeviceConnect
    {
        IDeviceOperator GetDeviceOperator();
    }
}
