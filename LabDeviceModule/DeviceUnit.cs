using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
using LabMCESystem.BaseService;
using LabMCESystem.EException;

namespace LabMCESystem.LabDeviceModule
{
    public abstract class DeviceUnit
    {
        #region Fields

        private IDeviceOperator _devOperator;

        private LabDevice _device;
        #endregion

        #region Properties
        /// <summary>
        /// 获取设备的异常监测器。
        /// 在使用前需要先调用InitialExceptionWatcher进行初始化，否则无效。
        /// </summary>
        public DevEExceptionWatcher ExceptionWatcher { get; private set; }
        
        protected IDeviceOperator DevOperator { get { return _devOperator; } }

        public IExcepManagement ExcepManager { get; private set; }

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

        public void ConnectElementManager(IDeviceConnect connecter)
        {
            _devOperator = connecter.DeviceOperator;
        }

        public void ConnectEExceptionManager(IExcepManagement excepMnanger)
        {
            ExcepManager = excepMnanger;
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
        
        /// <summary>
        /// 重写此函数以响应异常报告事件。
        /// </summary>
        /// <param name="e">异常报告事件参数。</param>
        protected virtual void OnExceptionWatcherReport(WatcherReportEExceptionEventArgs e)
        {
            ExcepManager?.ActivateEException(e.EExceptionInfor, e);
        }

        /// <summary>
        /// 初始化设备的异常监测器。
        /// </summary>
        public void InitialExceptionWatcher()
        {
            DevEExceptionWatcher deWatcher = new DevEExceptionWatcher(); ;
            if (OnInitialExceptionWatcher(ref deWatcher))
            {
                ExceptionWatcher = deWatcher;
                ExceptionWatcher.ReportEException += ExceptionWatcher_ReportEException;
            }
        }

        /// <summary>
        /// 派生类可重写此虚函数，在其中执行ExceptionWatcher的初始化操作。
        /// </summary>
        /// <returns>如果初化成功则返回true，可得到有ExceptionWatcher的有效实例。</returns>
        protected virtual bool OnInitialExceptionWatcher(ref DevEExceptionWatcher deWatcher)
        {
            return true;
        }

        private void ExceptionWatcher_ReportEException(object sender, WatcherReportEExceptionEventArgs e)
        {
            OnExceptionWatcherReport(e);
        }

        #endregion

    }
}
