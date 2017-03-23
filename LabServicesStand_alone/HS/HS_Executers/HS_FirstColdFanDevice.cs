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
        public HS_FirstColdFanDevice(string designMark) : base(0, new SafeRange(0, 2900), new PIDParam() { Ts = 5000, Kp = 1, Td = 0, Ti = 0 })
        {
            DesignMark = designMark;

            ExecuteChanged += HS_FirstColdFanDevice_ExecuteChanged;

            UpdateFedback += HS_FirstColdFanDevice_UpdateFedback;

            ExecuteOvered += HS_FirstColdFanDevice_ExecuteOvered;

            ExecutePredicate = FanCtrlPredicate;
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Start();
            PeriodInterval = 1000;
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
        /// 风机变频器故障检测通道。
        /// </summary>
        public StatusChannel FanFreqErrorChannel { get; set; }
        /// <summary>
        /// 变频器运行状态监测通道。
        /// </summary>
        public StatusChannel FanBeRunChannel { get; set; }
        /// <summary>
        /// 变频器停止状态监测通道。
        /// </summary>
        public StatusChannel FanBeStopChannel { get; set; }
        /// <summary>
        /// 变频器远程本地状态通道。
        /// </summary>
        public StatusChannel FanRemoteLocalChannel { get; set; }

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
        /// <summary>
        /// 记录高电流 低转速次数。
        /// </summary>
        private int _hAmmeterLRoStopTick = 0;
        /// <summary>
        /// 高电流低转速停止次数。
        /// </summary>
        private const int HALRSTickCount = 10;

        #endregion
        // 更新风机数据。
        private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateFanStatus();
        }

        private bool FanCtrlPredicate(object sender, ref double eVal)
        {
            bool be = true;
            //be &= FanRemoteLocalChannel.Status;
            //if (be && !FanReadyChannel.Status)
            //{
            //    FanDeviceImp.Fan.CallReady();
            //}

            // 如果风机运行过程中 重新设置转速 侧需要当前电机的转速稳定才能设置
            // 条件为：当前已设置转速与反馈转速小于50公差。
            if (FanBeRunChannel?.Status == true)
            {
                Tolerance tr = new Tolerance(50);
                if (!tr.IsInTolerance(FanDeviceImp.Fan.SettedRotateSpeed, FanDeviceImp.Fan.RotateSpeed))
                {
                    be = false;
                }
            }
            return be;
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
                if (FanDeviceImp.Fan.SetRotateSpeed(executedVal))
                {
                    //System.Threading.Thread.Sleep(1000);
                    if (FanBeStopChannel.Status)
                    {
                        FanDeviceImp.Fan.Run();
                        _hAmmeterLRoStopTick = 0;
                    }

                    if (FanBeRunChannel.Status)
                    {
                        ExecuteOver();
                    }
                }
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
            int erCode = FanDeviceImp.Fan.ErrorCode;
            if (erCode > 0)
            {
                Debug.Assert(FanFreqErrorChannel != null);
                if (!FanFreqErrorChannel.Status)
                {
                    FanFreqErrorChannel.Status = true;
                }
                string estr = FanDeviceImp.Fan.ErrorSummary(erCode);

                FanFreqErrorChannel.Summary = $"变频器故障 {erCode}:{estr ?? string.Empty}。";
            }
            // 当电流过大，实际反馈为0是 主动停止变频器。
            if (FanBeRunChannel.Status &&
                FanDeviceImp.Fan.Ammeter > 300 && FanDeviceImp.Fan.RotateSpeed <= 5)
            {
                _hAmmeterLRoStopTick++;
                if (_hAmmeterLRoStopTick >= HALRSTickCount)
                {
                    FanDeviceImp.Fan.Stop();
                    _hAmmeterLRoStopTick = 0;
                }
            }
            return true;
        }

        protected override bool OnExecute(ref double eVal)
        {
            bool rb = base.OnExecute(ref eVal);
            eVal = TargetVal;
            return rb;
        }

        public override void Reset()
        {
            base.Reset();
            if (IsCOMControlMode)
            {
                if (FanBeRunChannel.Status)
                {
                    FanDeviceImp.Fan.DownToZero();
                    _hAmmeterLRoStopTick = 0;
                }
            }
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

        private float[] _values = new float[4];

        public bool Connection
        {
            get
            {
                bool b = CommunicationReady() == 1 ? true : false;
                return b;
            }
        }

        public double Ammeter { get { return _values[2]; } }

        public bool AbbIsRun { get; private set; } = false;

        public double RotateSpeed
        {
            get { return _values[1]; }

        }

        public double SettedRotateSpeed { get { return _values[0]; } }


        public int ErrorCode { get { return (int)_values[3]; } }

        public double Freqence { get { return _values[0]; } }

        public void CallReady()
        {
            lock (_syncLocker)
            {
                SetFJInit();
            }
        }

        public bool SetRotateSpeed(double speed)
        {
            bool bs = false;
            lock (_syncLocker)
            {
                bs = SetHz((float)speed);
                Debug.WriteLine($"Set fan device rotate speed {bs}.");
                for (int i = 0; i < 10; i++)
                {
                    GetValues();

                    if (Math.Round(SettedRotateSpeed) == Math.Round(speed))
                    {
                        bs &= true;
                        break;
                    }

                    System.Threading.Thread.Sleep(50);
                }
            }
            return bs;
        }

        public bool Run()
        {
            if (!AbbIsRun)
            {
                lock (_syncLocker)
                {
                    int r = SetStart();
                    AbbIsRun = r == 1 ? true : false;
                }
            }
            return AbbIsRun;
        }
        /// <summary>
        /// 控制转速至0并停机。
        /// </summary>
        /// <returns></returns>
        public bool DownToZero()
        {
            bool bs = false;
            if (AbbIsRun)
            {
                lock (_syncLocker)
                {
                    bs = SetHz(0);
                }
                AbbIsRun = false;
            }
            return bs;
        }
        /// <summary>
        /// 直接停止变频器。
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            int rl = 0;
            lock (_syncLocker)
            {
                rl = SetStop();
            }
            return rl == 1 ? true : false;
        }

        public void ResetError()
        {
            if (ErrorCode != 0)
            {
                lock (_syncLocker)
                {
                    SetRest();
                    System.Threading.Thread.Sleep(1000);
                    Stop();
                }
            }
        }

        public float[] GetValues()
        {
            IntPtr vs = GetValue();
            Marshal.Copy(vs, _values, 0, _values.Length);
            return _values;
        }

        public string ErrorSummary(int ecode)
        {
            string ers = null;
            IntPtr sptr = GetErroExplain((short)ecode);
            if (sptr != null)
            {
                ers = Marshal.PtrToStringAuto(sptr);
            }
            return ers;
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

        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
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
        extern public static int CommunicationReady();
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        extern public static IntPtr GetErroExplain(short code);
        [DllImport(@"RS485Ctrl\RS485Ctrl.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "GetVlue")]
        extern public static IntPtr GetValue();
    }
}
