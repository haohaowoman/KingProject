using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;

namespace LabMCESystem.BaseService
{
    public interface ILabElementManageable : IDeviceOperator, IExpAreaOperator
    {
        #region Events

        event NotifyCollectionChangedEventHandler DevicesChanged;

        event NotifyCollectionChangedEventHandler ExperimentAreaesChanged;

        #endregion

        #region Operations

        IReadOnlyList<LabDevice> Devices { get; }

        IReadOnlyList<ExperimentArea> ExperimnetAreaes { get; }

        ExperimentArea LookUpExpArea(string label);

        LabDevice LookUpDevice(string label);

        #endregion
    }
}
