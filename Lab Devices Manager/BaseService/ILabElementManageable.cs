using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;

namespace LabMCESystem.BaseService
{
    public interface ILabElementManageable : IDeviceOperator, IExpAreaOperator, IDeviceElementListen, IExpAreaElementListen
    {
        #region Events

        event NotifyCollectionChangedEventHandler DevicesChanged;

        event NotifyCollectionChangedEventHandler ExperimentAreaesChanged;

        #endregion

        #region Operations

        bool AddNewSensor(LinerSensor newSensor);

        bool RemoveSensor(LinerSensor newSensor);

        bool RemoveSensorAsNumber(string sensorNumber);

        #endregion
    }
}
