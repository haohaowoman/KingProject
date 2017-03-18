using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Execute;
using mcLogic;
using LabMCESystem.LabElement;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;

namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 环散系统风机变频器控制器
    /// 风机可以通过选择进行PLC 模拟量控制变频面频率，也可以通过rs485通讯方式控制频率
    /// 对一冷实现段入口流量的控制是通过风机进行变频控制
    /// 根据风机和变频器的滞后性可以设置PID参数
    /// Ts = 2s Kp = 0.8 Ti = 3s Td = 0
    /// 变频器到风机转速的转换关系：。。。
    /// 在此进行风机的状态，数据进行操作 风机的数据采集。
    /// </summary>
    class HS_FirstColdFanDevice : PredicatePositionPID, IDisposable
    {
        public HS_FirstColdFanDevice(string designMark) : base(0, new SafeRange(0, 2900), new PIDParam() { Ts = 20000, Kp = 1, Td = 0, Ti = 0 })
        {
            DesignMark = designMark;

            ExecuteChanged += HS_FirstColdFanDevice_ExecuteChanged;

            UpdateFedback += HS_FirstColdFanDevice_UpdateFedback;

            ExecuteOvered += HS_FirstColdFanDevice_ExecuteOvered;

            //ExecutePredicate = UpdateFanStatus;
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Start();
        }

        #region Properties

        private Timer _updateTimer = new Timer(1000);

        /// <summary>
        /// 获取/设置表示风机设备的通道。
        /// </summary>
        public FeedbackChannel FanDeviceChannel { get; set; }
        /// <summary>
        /// 获取/设置风机PCL频率设定通道通道。
        /// </summary>
        public FeedbackChannel FanFrequencePlcChannel { get; set; }
        /// <summary>
        /// 获取/设置风机启动通道。
        /// </summary>
        public StatusOutputChannel FanStartChannel { get; set; }
        /// <summary>
        /// 获取/设置风机停止通道。
        /// </summary>
        public StatusOutputChannel FanStopChannel { get; set; }

        /// <summary>
        /// 获取/设置风机的远程连接通道。
        /// </summary>
        public StatusChannel FanConnectionChannel { set; get; }
        /// <summary>
        /// 风机已准备好状态通道。
        /// </summary>
        public StatusChannel FanReadyChannel { get; set; }
        /// <summary>
        /// 风机电流通道。
        /// </summary>
        public IAnalogueMeasure FanIChannel { get; set; }

        /// <summary>
        /// 获取/设置风机是否通道COM 485通讯进行频率控制，否则通道PLC模拟量控制变频器频率。
        /// </summary>
        public bool IsCOMControlMode { get; set; } = true;

        /// <summary>
        /// 获取风机的转速。
        /// </summary>
        public double RevolvingSpeed { get; private set; }

        private string _comport;
        /// <summary>
        /// 获取/设置风机控制器的串行端口名称。
        /// </summary>
        public string COMPort
        {
            get { return _comport; }
            set { _comport = value; }
        }

        #endregion
        // 更新风机数据。
        private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateFanStatus();
        }

        // 在此更新风机的转速状态。
        private void HS_FirstColdFanDevice_UpdateFedback(IDataFeedback sender)
        {
            sender.FedbackData = FanDeviceChannel.MeasureValue;
        }

        // 使用 控制风机设备。
        private void HS_FirstColdFanDevice_ExecuteChanged(object sender, double executedVal)
        {
            // 设置风机频率。
            if (IsCOMControlMode)
            {
                FanDeviceImp.Fan.RotateSpeed = executedVal;
                FanDeviceImp.Fan.Run();
            }
            else
            {
                Debug.Assert(FanFrequencePlcChannel != null);
                FanFrequencePlcChannel.AOValue = executedVal;
                (FanFrequencePlcChannel.Controller as Executer)?.ExecuteBegin();

                //
                Debug.Assert(FanStartChannel != null);
                var exe = FanStartChannel.Controller as SimplePulseExecuter;
                Debug.Assert(exe != null);
                exe.ExecuteBegin();
            }

        }

        private void HS_FirstColdFanDevice_ExecuteOvered(object obj)
        {

            // 重置风机频率。
            if (IsCOMControlMode)
            {
                FanDeviceImp.Fan.Stop();
            }
            else
            {
                Debug.Assert(FanStopChannel != null);
                var exe = FanStopChannel.Controller as SimplePulseExecuter;
                Debug.Assert(exe != null);
                exe.ExecuteBegin();

                Debug.Assert(FanFrequencePlcChannel != null);

                (FanFrequencePlcChannel.Controller as Executer)?.ExecuteOver();
            }
        }

        // 需要实时地采集获取风机的运行状态。
        private bool UpdateFanStatus()
        {
            FanDeviceImp.Fan.GetValues();
            FanConnectionChannel.Status = FanDeviceImp.Fan.Connection;
            FanIChannel.MeasureValue = FanDeviceImp.Fan.Ammeter;
            FanDeviceChannel.MeasureValue = FanDeviceImp.Fan.RotateSpeed;
            return true;
        }

        protected override bool OnExecute(ref double eVal)
        {
            bool rb = base.OnExecute(ref eVal);
            eVal = TargetVal;
            return rb;
        }

        public new void Dispose()
        {
            base.Dispose();

            _updateTimer.Stop();
            _updateTimer.Close();
            
            FanDeviceImp.Fan.Dispose();
        }
    }

    class FanDeviceImp : IDisposable
    {
        private FanDeviceImp()
        {
            int s = InitSerial(@"\\.\COM10");
        }

        static private FanDeviceImp _dev = null;

        private object _syncLocker = new object();

        private float[] _values = new float[5];

        public bool Connection { get { return CommunicationReady(); } }

        public double Ammeter { get { return _values[2]; } }

        public double RotateSpeed
        {
            get { return _values[1]; }
            set
            {
                lock (_syncLocker)
                {
                    SetHz((float)value);
                }
            }
        }

        public int ErrorCode { get { return (int)_values[4]; } }

        public double Freqence { get { return _values[3]; } }

        public void CallReady()
        {
            lock (_syncLocker)
            {
                SetFJInit();
            }
        }

        public void Run()
        {
            lock (_syncLocker)
            {
                SetStart();
            }
        }

        public void Stop()
        {
            lock (_syncLocker)
            {
                SetStop();
            }
        }

        public void ResetError()
        {
            lock (_syncLocker)
            {
                SetRest();
            }
        }

        public float[] GetValues()
        {
            IntPtr vs = GetValue();
            Marshal.Copy(vs, _values, 0, 5);
            return _values;
        }

        public void Dispose()
        {
            lock (_syncLocker)
            {
                AllRelease();
            }
        }

        static public FanDeviceImp Fan
        {
            get
            {
                if (_dev == null)
                {
                    _dev = new FanDeviceImp();
                }
                return _dev;
            }
        }

        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int InitSerial(string com);
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int SetStart();
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int SetStop();
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int SetRest();
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static int SetFJInit();
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetHZ")]
        extern public static bool SetHz(float hz);
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static bool AllRelease();
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static bool CommunicationReady();
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl)]
        extern public static string GetErroExplain(short code);
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetVlue")]
        extern public static IntPtr GetValue();
    }
}
