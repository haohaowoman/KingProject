using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LabMCESystem.Logic.Execute
{
    /// <summary>
    /// 表示PID参数
    /// </summary>
    public struct PIDParam
    {
        /// <summary>
        /// 获取/设置PID采集周期，以毫秒为单位
        /// </summary>
        public double Ts { get; set; }

        /// <summary>
        /// 获取/设置PID的比例系数，Proportional
        /// </summary>
        public double Kp { get; set; }

        /// <summary>
        /// 获取/设置PID的积分时间， Integrating Time
        /// </summary>
        public double Ti { get; set; }

        /// <summary>
        /// 获取/设置PID的微分时间， Defferentional Time
        /// </summary>
        public double Td { get; set; }

        /// <summary>
        /// 获取位置式PID控制算法的积分和微分放大系数
        /// 公式为 u(k) = Kp * e(t) + Ki * SUM[e(0), e(t)] + Kd *[e(t) - e(t - 1)] + u(0);
        /// </summary>
        /// <param name="Ki">返回积分放大系数</param>
        /// <param name="Kd">返回微分放大系数</param>
        public void GetPostionPIDParam(out double Ki, out double Kd)
        {
            if (Ts == 0)
            {
                Ki = Kd = 0;
            }
            else if (Ti == 0)
            {
                Ki = 0;
                Kd = Kp * Td / Ts;
            }
            else
            {
                Ki = Kp * Ts / Ti;
                Kd = Kp * Td / Ts;
            }
        }

        /// <summary>
        /// 获取增量式PID控制算法参数
        /// 公式为 du(t) = Q0 * e(t) + Q1 * e(t - 1) + Q2 * e(t - 2);
        /// </summary>
        /// <param name="Q0">t 时刻误差系数</param>
        /// <param name="Q1">t - 1 时刻误差系数</param>
        /// <param name="Q2">t - 2时刻误差系数</param>
        public void GetIncrementPIDParam(out double Q0, out double Q1, out double Q2)
        {
            if (Ts == 0)
            {
                Q0 = Q1 = Q2 = 0;
            }
            else if (Ti == 0)
            {
                Q0 = Kp * (1 + Td / Ts);
                Q1 = 0;
                Q2 = Kp * Td / Ts;
            }
            else
            {
                Q0 = Kp * (1 + Ts / Ti + Td / Ts);
                Q1 = -Kp * (1 + 2 * Td / Ts);
                Q2 = Kp * Td / Ts;
            }
        }

        /// <summary>
        /// 获取一个默认的P调节器的参数
        /// </summary>
        public static PIDParam PExecuterParam
        {
            get
            {
                return new PIDParam() { Ts = 100.0, Kp = 1, Ti = 0, Td = 0 };
            }
        }

        /// <summary>
        /// 获取一个默认的PI调节器的参数
        /// </summary>
        public static PIDParam PIExecuterParam
        {
            get
            {
                return new PIDParam() { Ts = 100.0, Kp = 1, Ti = 1, Td = 0 };
            }
        }

        /// <summary>
        /// 获取一个默认的PD调节器的参数
        /// </summary>
        public static PIDParam PDExecuterParam
        {
            get
            {
                return new PIDParam() { Ts = 100.0, Kp = 1, Ti = 0, Td = 1 };
            }
        }

        /// <summary>
        /// 获取一个默认的PID调节器的参数
        /// </summary>
        public static PIDParam PIDExecuterParam
        {
            get
            {
                return new PIDParam() { Ts = 100.0, Kp = 1, Ti = 1, Td = 1 };
            }
        }
    }

    /// <summary>
    /// PID控制逻辑执行器基类
    /// </summary>
    public abstract class PIDExecuterBase : ClosedLoopExecuter, IPeriodExecute, IDisposable
    {
        public PIDExecuterBase(double targetVal, SafeRange srange) : base(targetVal, srange)
        {
            _periodTimer = new Timer();

            _periodTimer.Elapsed += _periodTimer_Elapsed;

            ExecuteChanged += PIDExecuterBase_ExecuteChanged;

            FedbackInTolerance += PIDExecuterBase_FedbackInToleranced;
        }

        /// <summary>
        /// 通过指定PID参数创建
        /// </summary>
        /// <param name="targetVal">执行目标值</param>
        /// <param name="srange">执行器的安全范围</param>
        /// <param name="param">PID参数</param>
        public PIDExecuterBase(double targetVal, SafeRange srange, PIDParam param) : this(targetVal, srange)
        {
            Param = param;
        }

        #region Operators

        // 执行后对执行次数自加
        private void PIDExecuterBase_ExecuteChanged(object sender, double executedVal)
        {
            ExecuteTCount++;
            OnPIDExecute();
        }
        // 周期执行
        private void _periodTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Execute();
        }

        private void PIDExecuterBase_FedbackInToleranced(object obj)
        {
            // 自动停止执行
            if (AutoFinish)
            {
                ExecuteOver();
            }
        }

        #endregion

        #region Override

        public override void Reset()
        {
            ExecuteOver();

            ExecuteTCount = 0;
            ECurrent = 0;
            ELastOne = 0;
            
            base.Reset();
        }
        /// <summary>
        /// 执行开始，默认调用Start()。
        /// </summary>
        public override void ExecuteBegin()
        {
            Start();
        }

        // 重载 进行基本的PID逻辑数据运算
        protected override bool OnExecute(ref double eVal)
        {
            bool be = base.OnExecute(ref eVal);

            if (be)
            {
                // 得到当前的执行误差 与 上一次执行误差
                if (ExecuteTCount >= 1)
                {
                    ELastOne = ECurrent;
                }
                ECurrent = TargetVal - FedbackData;

                be = OnPIDMath(ref eVal);
            }

            return be;
        }

        public override void ExecuteOver()
        {
            _periodTimer.Stop();
            base.ExecuteOver();
        }

        /// <summary>
        /// 重载此虚函数进行PID运算，并得出需要输出的值
        /// </summary>
        /// <param name="eVal">执行值的引用</param>
        /// <returns>是否执行</returns>
        protected abstract bool OnPIDMath(ref double eVal);

        /// <summary>
        /// 得载此虚函数，在得到PID执行值时调用
        /// </summary>
        protected abstract void OnPIDExecute();

        #endregion

        #region Properties， Fields
        // 周期定时器
        private Timer _periodTimer;

        private PIDParam _param;
        /// <summary>
        /// 获取/设置PID参数
        /// </summary>
        public PIDParam Param
        {
            get { return _param; }
            set
            {
                _param = value;
                _periodTimer.Interval = value.Ts;
            }
        }

        /// <summary>
        /// 获取已运算执行次数
        /// </summary>
        public ulong ExecuteTCount { get; private set; }

        /// <summary>
        /// 获取当前的执行误差
        /// </summary>
        public double ECurrent { get; private set; }

        /// <summary>
        /// 获取上一次的执行误差
        /// </summary>
        public double ELastOne { get; private set; }

        /// <summary>
        /// 获取/设置是否在反馈值达到目标值指定的公差范围内自动完成和关闭执行器
        /// </summary>
        public bool AutoFinish { get; set; } = false;

        #endregion

        #region IPeriodExecute

        /// <summary>
        /// 获取/设置PID的执行周期
        /// </summary>
        public double PeriodInterval
        {
            get
            {
                return _param.Ts;
            }

            set
            {
                _periodTimer.Interval = value;
                _param.Ts = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _periodTimer.Enabled;
            }
        }

        public void Resum()
        {
            _periodTimer.Enabled = true;
        }

        public void Start()
        {
            _periodTimer.Start();
        }

        public void Stop()
        {
            ExecuteOver();
        }

        public void Suspend()
        {
            _periodTimer.Enabled = false;
        }

        public void Dispose()
        {
            _periodTimer?.Dispose();
        }

        public void Close()
        {
            _periodTimer?.Close();
        }

        #endregion

    }
}
