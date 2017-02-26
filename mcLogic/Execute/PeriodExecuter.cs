using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace mcLogic.Execute
{
    /// <summary>
    /// 可周期动作的执行控制器
    /// 使用System.Timers.Timer实现周期定时功能
    /// </summary>
    public class PeriodExecuter : Executer, IPeriodExecute, IDisposable
    {
        public PeriodExecuter(double targetVal, SafeRange srang):base(targetVal, srang)
        {
            _periodTimer = new Timer();
            _periodTimer.Elapsed += _periodTimer_Elapsed;
        }

        #region Properties,Fields

        // 周期的定时器
        private Timer _periodTimer;
        /// <summary>
        /// 获取已有的周期次数。
        /// </summary>
        public int PeriodCount { get; protected set; }
        #endregion

        #region Override

        /// <summary>
        /// 执行开始，默认调用Start()。
        /// </summary>
        public override void ExecuteBegin()
        {
            Start();
        }

        public override void ExecuteOver()
        {
            _periodTimer.Stop();
            base.ExecuteOver();
        }

        protected override bool OnExecute(ref double eVal)
        {
            eVal = TargetVal;
            return true;
        }

        public override void Reset()
        {
            _periodTimer.Stop();
            base.Reset();
        }
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

        public void Dispose()
        {
            _periodTimer?.Close();
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

        #endregion
        
        private void _periodTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PeriodCount++;
            Execute();
        }

    }
}
