using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LabMCESystem.Logic.Execute
{
    /// <summary>
    /// 可周期动作的执行控制器
    /// 使用System.Timers.Timer实现周期定时功能
    /// </summary>
    public class PeriodExecuter : Executer, IPeriodExecute, IDisposable
    {
        public PeriodExecuter(float targetVal, SafeRange srang):base(targetVal, srang)
        {
            _periodTimer = new Timer();
            _periodTimer.Elapsed += _periodTimer_Elapsed;
        }

        private void _periodTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Execute();
        }

        #region Properties,Fields

        // 周期的定时器
        private Timer _periodTimer;

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
            _periodTimer.Stop();
            ExecuteOver();
        }

        public void Suspend()
        {
            _periodTimer.Enabled = false;
        } 
        #endregion
    }
}
