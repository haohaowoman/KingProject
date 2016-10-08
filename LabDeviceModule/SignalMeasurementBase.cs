using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.BaseService.ExperimentDataExchange;

namespace LabMCESystem.LabDeviceModule
{
    public abstract class SignalMeasurementBase : DeviceUnit
    {
        #region Properties

        private IExpDataPush _pusher;

        protected IExpDataPush Pusher { get { return _pusher; } }

        #endregion

        #region Operators

        public void ConnectMeasurDevDataExchange(IMeasureDevDataConnect connecter)
        {
            _pusher = connecter.DataPusher;
        }

        #endregion

    }
}
