using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Execute
{
    /// <summary>
    /// PID位置算法执行器
    /// 是全程量输出PID，适合执行机构有控制量对应关系，如阀门开度
    /// </summary>
    public class PositionPIDExecuter : PIDExecuterBase
    {
        public PositionPIDExecuter(double targetVal, SafeRange srange) : this(targetVal, srange, PIDParam.PExecuterParam)
        {

        }

        public PositionPIDExecuter(double targetVal, SafeRange srange, PIDParam param) : base(targetVal, srange, param)
        {
            TargetValChanged += PositionPIDExecuter_TargetValChanged;
        }
        
        #region Override

        protected override bool OnPIDMath(ref double eVal)
        {            
            // 计算微分项，用差分代替微分
            EDefferentiation = ECurrent - ELastOne;

            if (ExecuteTCount == 0)
            {
                // 第一次输出以反馈的输出为默认起点
                eVal = Param.Kp * ECurrent + FedbackData;
            }
            else
            {                
                double Kd = 0;
                double Ki = 0;
                Param.GetPostionPIDParam(out Ki, out Kd);

                if (AllwaysInIntegrating)
                {
                    // 对积分项进行积分
                    EIntegrating += ECurrent;
                    eVal = Param.Kp * ECurrent + Ki * EIntegrating + Kd * EDefferentiation + _initExecuteVal;
                }
                else
                {
                    // 主动分离积分项。
                    if (AllowTolerance.IsInTolerance(TargetVal, FedbackData))
                    {
                        // 对积分项进行积分
                        EIntegrating += ECurrent;
                        eVal = Param.Kp * ECurrent + Ki * EIntegrating + Kd * EDefferentiation + _initExecuteVal;
                    }
                    else
                    {
                        EIntegrating = 0;
                        eVal = Param.Kp * ECurrent + Kd * EDefferentiation + _initExecuteVal;
                    }
                }                
            }
            return true;
        }

        public override void Reset()
        {
            _initExecuteVal = EIntegrating = EDefferentiation = 0;
            base.Reset();
        }

        protected override void OnPIDExecute()
        {
            // 已执行一次输出
            if (ExecuteTCount >= 1)
            {
                _initExecuteVal = ExecuteVal;
            }
        }

        #endregion

        #region Operations

        /// <summary>
        /// 控制目标值发生改变时，如果进行了连续控制的改变，则清除积分项误差，减少积分对突变的影响。
        /// </summary>        
        private void PositionPIDExecuter_TargetValChanged(object sender, TargetValChangeEventArgs e)
        {
            EIntegrating = 0;
        }
        
        #endregion

        #region Properties

        // 记录初始的执行输出值，即ExecuteTCount == 1时的执行值
        private double _initExecuteVal;

        /// <summary>
        /// 获取积分项
        /// </summary>
        public double EIntegrating { get; private set; }

        /// <summary>
        /// 获取微分项
        /// </summary>
        public double EDefferentiation { get; private set; }
        /// <summary>
        /// 获取/设置此值，指示是否总是加入积分项进行计算，如果为False则只会在反馈达到允许公差后加入积分项计算。
        /// </summary>
        public bool AllwaysInIntegrating { get; set; } = false;

        #endregion
    }
}
