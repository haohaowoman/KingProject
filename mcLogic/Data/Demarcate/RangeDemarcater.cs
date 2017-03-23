using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Demarcate
{
    /// <summary>
    /// 需要确定电信号范围与工程量表示范围的标定器。
    /// </summary>
    public class RangeDemarcater : IDemarcate
    {
        /// <summary>
        /// 指定电信号表示范围与工程量表示范围创建标定器。
        /// </summary>
        /// <param name="elecRange">电信号表示范围。</param>
        /// <param name="engiRange">工程量表示范围。</param>
        /// <exception cref="ArgumentOutOfRangeException">设备的电信号范围无效。</exception>
        public RangeDemarcater(SafeRange elecRange, SafeRange engiRange)
        {
            if (elecRange.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(elecRange), "设置的电信号范围无效。");
            }
            // electric
            ElecRange = elecRange;
            
            // engineering
            EngiRange = engiRange;
        }

        /// <summary>
        /// 获取/设置电信号表示范围。
        /// </summary>
        public SafeRange ElecRange { get; set; }

        /// <summary>
        /// 获取/设置工程量表示范围。
        /// </summary>
        public SafeRange EngiRange { get; set; }

        /// <summary>
        /// 将电信号标定为工程量。
        /// </summary>
        /// <param name="electric">电信号值。</param>
        /// <returns>经过标定的工程量值。</returns>
        public double Demarcate(double electric)
        {
            double en;
            en = EngiRange.Length / ElecRange.Length * (electric - ElecRange.Low) + EngiRange.Low;
            return en;
        }

        /// <summary>
        /// 计算线性方程参数。
        /// </summary>
        /// <returns>返回线性方程参数。</returns>
        public double[] LinerParams()
        {
            double[] ps = new double[2];

            ps[1] = EngiRange.Length / ElecRange.Length;

            ps[0] = EngiRange.Low - ps[1] * ElecRange.Low;

            return ps;
        }
    }
}
