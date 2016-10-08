using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.BaseService.ExperimentDataExchange;
namespace LabMCESystem.LabDeviceModule
{
    public abstract class MultipleDevUnitBase : SignalMeasurementBase
    {
        #region Properties

        private ISingleExpRTDataRead _singleReader;

        protected ISingleExpRTDataRead SingleReader
        {
            get { return _singleReader; }
            private set { _singleReader = value; }
        }

        #endregion

        #region Operators

        public void ConnectMultipleDevDataExchange(IMulDevDataConnect connecter)
        {
            SingleReader = connecter.SingleDataReader;

            ConnectMeasurDevDataExchange(connecter);
        }

        #endregion
    }
}
