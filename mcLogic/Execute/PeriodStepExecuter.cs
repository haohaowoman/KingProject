using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace mcLogic.Execute
{
    /// <summary>
    /// 可周期动作的周期步进执行控制器
    /// </summary>
    public class PeriodStepExecuter : StepExecuter, IPeriodExecute, IDisposable
    {
        /// <summary>
        /// 指定目标值与安全输出范围创建
        /// </summary>
        /// <param name="targetVal">目标值</param>
        /// <param name="srange">安全范围</param>
        public PeriodStepExecuter(double targetVal, SafeRange srange) : base(targetVal, srange)
        {
            _periodTimer = new Timer();
            _periodTimer.Elapsed += _periodTimer_Elapsed;
        }

        /// <summary>
        /// 指定时间间隔创建
        /// </summary>
        /// <param name="targetVal">目标值</param>
        /// <param name="srange">安全范围</param>
        /// <param name="interval">时间间隔，以毫秒为单位</param>
        public PeriodStepExecuter(double targetVal, SafeRange srange, double interval) : this(targetVal, srange)
        {
            _periodTimer.Interval = interval;
        }

        #region Fields

        // 周期执行定时器
        Timer _periodTimer;

        #endregion

        #region IPeriodExecute

        public double PeriodInterval
        {
            get
            {
                return _periodTimer.Interval;
            }

            set
            {
                _periodTimer.Interval = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return _periodTimer.Enabled;
            }
        }

        /// <summary>
        /// 获取已有的周期次数。
        /// </summary>
        public int PeriodCount
        {
            get;
            protected set;
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

        #endregion

        private void _periodTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PeriodCount++;
            Execute();            
        }

        public override void ExecuteOver()
        {
            _periodTimer.Stop();
            base.ExecuteOver();
        }
    }

    /// <summary>
    /// 自动周期获取反馈步进执行器
    /// </summary>
    public class AutoFedbackStepExecuter : PeriodStepExecuter, IDataFeedback
    {
        /// <summary>
        /// 指定目标值与安全输出范围创建
        /// </summary>
        /// <param name="targetVal">目标值</param>
        /// <param name="srange">安全范围</param>
        public AutoFedbackStepExecuter(double targetVal, SafeRange srange) : base(targetVal, srange)
        {
        }

        /// <summary>
        /// 指定时间间隔创建
        /// </summary>
        /// <param name="targetVal">目标值</param>
        /// <param name="srange">安全范围</param>
        /// <param name="interval">时间间隔，以毫秒为单位</param>
        public AutoFedbackStepExecuter(double targetVal, SafeRange srange, double interval) : base(targetVal, srange, interval)
        {
        }

        #region IDataFedback

        //反馈数据
        private double _fedbackData;
        /// <summary>
        /// 获取/设置执行器反馈
        /// </summary>
        public double FedbackData
        {
            get
            {
                return _fedbackData;
            }
            set
            {
                _fedbackData = value;

                if (!SafeRange.IsSafeIn(_fedbackData))
                {
                    FedbackDataOutOfSafeRange?.Invoke(this, _fedbackData);
                }
            }
        }

        public event FedbackDataOutOfSafeRangeEventHandler FedbackDataOutOfSafeRange;
        public event UpdateFedbackValEventHandler UpdateFedback;

        #endregion

        #region Override

        protected override bool OnExecute(ref double eVal)
        {
            UpdateFedback?.Invoke(this);

            // 以反馈数据确定当前输出量
            ExecuteVal = FedbackData;

            return true;
        }

        #endregion
    }
}
