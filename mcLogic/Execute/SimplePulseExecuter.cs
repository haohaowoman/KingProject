using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Execute
{
    /// <summary>
    /// 表示脉冲的位。
    /// </summary>
    public enum PulseBit
    {
        LowBit,
        HighBit
    }

    /// <summary>
    /// 简单脉冲执行器，给定一定的脉冲周期自动改变应该输出的脉冲表示位。
    /// 使用简单的定时器进行脉冲周期控制。
    /// </summary>
    public class SimplePulseExecuter : PeriodExecuter
    {
        public SimplePulseExecuter(double pulsePeriod = 500) : base(1, new SafeRange(0, 1))
        {
            PeriodInterval = pulsePeriod;
        }

        private int _tickCount = 0;

        private bool _isShortPulse = true;
        /// <summary>
        /// 获取/设置不否为短脉冲控制。只输出一个脉冲。
        /// </summary>
        public bool IsShortPulse
        {
            get { return _isShortPulse; }
            set
            {
                _isShortPulse = value;                
            }
        }

        /// <summary>
        /// 获取应该输出的下一脉冲位。
        /// </summary>
        public PulseBit NextPulseBit { get; private set; } = PulseBit.LowBit;

        protected override bool OnExecute(ref double eVal)
        {
            NextPulseBit = NextPulseBit == PulseBit.LowBit ? PulseBit.HighBit : PulseBit.LowBit;
                        
            if (_isShortPulse && _tickCount >= 1)
            {
                NextPulseBit = PulseBit.LowBit;
                Stop();
            }
            else
            {
                _tickCount++;
            }
            eVal = NextPulseBit == PulseBit.HighBit ? 1.0 : 0;
            return true;
        }

        public override void Reset()
        {
            base.Reset();
            _tickCount = 0;
        }

        public override void ExecuteOver()
        {
            base.ExecuteOver();
            _tickCount = 0;
        }

        public override void ExecuteBegin()
        {
            _tickCount = 0;
            base.ExecuteBegin();
        }
    }
}

