using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService
{
    /// <summary>
    /// 提供设备登录的接口。
    /// </summary>
    public interface IDeviceLogin
    {
        LabDevice DeviceLogin(string devLabel, string password);

        LabDevice DeviceLogin(string devLabel);
    }
}
