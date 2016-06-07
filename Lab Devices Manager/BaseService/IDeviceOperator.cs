using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;

namespace LabMCESystem.BaseService
{
    public interface IDeviceOperator
    {
        bool RegistNewDevice(LabDevice nDev);

        bool DischargeDevice(string devLabel);

        bool DischargeDevice(LabDevice dev);

        LabDevice LoginDevice(int devRegID);

        bool ExitLogDevice(int devRegID);

        bool ExitLogDevice(LabDevice dev);
    }
}
