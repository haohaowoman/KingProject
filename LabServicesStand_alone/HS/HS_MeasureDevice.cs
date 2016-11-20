using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabDeviceModule;
namespace LabMCESystem.Servers.HS
{
    public class HS_MeasureDevice : SignalMeasurementBase
    {
        #region Base Class Override

        protected override void OnClosed()
        {
            throw new NotImplementedException();
        }

        protected override bool OnClosing()
        {
            throw new NotImplementedException();
        }

        protected override void OnLogin()
        {
            throw new NotImplementedException();
        }

        protected override void OnRegisted()
        {
            throw new NotImplementedException();
        }

        protected override bool OnRun()
        {
            throw new NotImplementedException();
        }

        protected override void OnRunning()
        {
            throw new NotImplementedException();
        }

        protected override void OnStopped()
        {
            throw new NotImplementedException();
        }

        protected override bool OnStopping()
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// 初始化所有采集通道
        /// </summary>
        private void InitialMeasureChannels()
        {
            // 热边
           
        }

    }
}
