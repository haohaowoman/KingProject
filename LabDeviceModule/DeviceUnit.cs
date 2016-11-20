using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
using LabMCESystem.BaseService;
namespace LabMCESystem.LabDeviceModule
{
    public abstract class DeviceUnit
    {
        #region Fields

        private IDeviceOperator _devOperator;

        private LabDevice _device;
        #endregion

        #region Properties

        protected IDeviceOperator DevOperator { get { return _devOperator; } }

        public LabDevice Device
        {
            set
            {
                if (_device != null)
                {
                    _device.DeviceStateChanged -= _device_DeviceStateChanged;
                }

                _device = value;

                if (_device != null)
                {
                    _device.DeviceStateChanged += _device_DeviceStateChanged;
                }                
            }

            get
            {
                return _device;
            }
        }

        private void _device_DeviceStateChanged(object sender, DeviceStateChangedEventArgs e)
        {
            if (!Object.ReferenceEquals(sender, _device))
            {
                return;
            }

            switch (e.NewState)
            {
                case DeviceState.Created:
                    break;
                case DeviceState.Registed:

                    OnRegisted();

                    break;
                case DeviceState.Connected:

                    OnLogin();

                    break;
                case DeviceState.Running:

                    OnRunning();

                    break;
                case DeviceState.Stopped:

                    OnStopped();

                    break;
                case DeviceState.Closed:

                    OnClosed();

                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Operators

        public void ConnectElementManagerment(IDeviceConnect connecter)
        {
            _devOperator = connecter.DeviceOperator;
        }

        public bool RegistMain()
        {
            return _devOperator.RegistNewDevice(Device);
        }

        public void LoginMain()
        {
            Device = _devOperator.LoginDevice(Device.RegistID);            
        }

        public void ExitLogMain()
        {
            _devOperator.ExitLogDevice(Device);
        }

        public void Run()
        {
            if (OnRun())
            {
                Device.State = DeviceState.Running;
            }
        }

        public void Stop()
        {
            if (OnStopping())
            {
                Device.State = DeviceState.Stopped;
            }            
        }

        public void Close()
        {
            if (OnClosing())
            {
                _devOperator.ExitLogDevice(_device);

                Device.State = DeviceState.Closed;
            }            
        }

        protected abstract void OnRegisted();

        protected abstract void OnLogin();

        protected abstract bool OnRun();

        protected abstract void OnRunning();

        protected abstract bool OnStopping();

        protected abstract void OnStopped();

        protected abstract bool OnClosing();

        protected abstract void OnClosed();

        public override string ToString()
        {
            return _device.ToString();
        }
        #endregion

    }
}
