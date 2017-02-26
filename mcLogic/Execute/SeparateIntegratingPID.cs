using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Execute
{
    /// <summary>
    /// 积分分离位置算法PID控制器，可有抑制拥有积分饱合对象在区间内出差振荡
    /// 需要首先给定控制目标值允许的饱合区间范围
    /// </summary>
    public class SeparateIntegratingPID : PositionPIDExecuter
    {
        public SeparateIntegratingPID(double targetVal, SafeRange srange) : base(targetVal, srange)
        {
        }

        public SeparateIntegratingPID(double targetVal, SafeRange srange, PIDParam param) : base(targetVal, srange, param)
        {

        }
        
        /// <summary>
        /// 获取/设置是否在公差范围外使用分离积分算法
        /// </summary>
        public bool IsSpearateIntegrating { get; set; } = true;

        #region Override

        protected override bool OnPIDMath(ref double eVal)
        {
            bool br = base.OnPIDMath(ref eVal);
            if (br && IsSpearateIntegrating && ExecuteTCount >= 1)
            {
                // 在此减去积分项的影响
                double ki, kd;
                Param.GetPostionPIDParam(out ki, out kd);
                eVal = eVal - ki * EIntegrating;
            }
            return br;
        }

        #endregion
    }
}
