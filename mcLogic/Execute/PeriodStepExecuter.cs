using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LabMCESystem.Logic.Execute
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
        public PeriodStepExecuter(float targetVal, SafeRange srange) : base(targetVal, srange)
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
        public PeriodStepExecuter(float targetVal, SafeRange srange, double interval) : this(targetVal, srange)
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
            _periodTimer.Stop();
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
            Execute();            
        }

    }
}
