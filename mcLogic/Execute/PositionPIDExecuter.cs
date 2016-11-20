using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Logic.Execute
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
            
        }


        #region Override

        protected override bool OnPIDMath(ref double eVal)
        {
            // 对积分项进行积分
            EIntegrating += ECurrent;

            // 计算微分项，用差分代替微分
            EDefferentiation = ELastOne;

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

                eVal = Param.Kp * ECurrent + Ki * EIntegrating + Kd * EDefferentiation + _initExecuteVal;
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
            if (ExecuteTCount == 1)
            {
                _initExecuteVal = ExecuteVal;
            }
        }

        #endregion

        #region Operations

        

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

        #endregion
    }
}
