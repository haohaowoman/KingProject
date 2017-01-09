using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Demarcate
{
    /// <summary>
    /// 通过线必方程计算得到的工程量的标定器。
    /// </summary>
    public class LinerDemarcater : IDemarcate
    {        
        public LinerDemarcater()
        {

        }

        /// <summary>
        /// 获取/设置常数参数。
        /// </summary>
        public double Param0 { get; set; } = 0;
        /// <summary>
        /// 获取/设置比例参数。
        /// </summary>
        public double Param1 { get; set; } = 1;


        /// <summary>
        /// 通过指定线性方程创建标定器。
        /// </summary>
        /// <param name="param0">常数参数。</param>
        /// <param name="param1">比例参数</param>
        public LinerDemarcater(double param0, double param1)
        {
            Param0 = param0;
            Param1 = param1;
        }

        /// <summary>
        /// 将电信号标定为工程量。
        /// </summary>
        /// <param name="electric">电信号值。</param>
        /// <returns>经过标定的工程量值。</returns>
        public double Demarcate(double electric)
        {
            return Param0 + Param1 * electric;
        }

        /// <summary>
        /// 从范围标定标定器得到线性标定器。
        /// </summary>
        /// <param name="rd">范围标定器。</param>
        /// <returns>线性标定器。</returns>
        public static LinerDemarcater FromRangeDemarcater(RangeDemarcater rd)
        {
            double[] ps = rd.LinerParams();
            return new LinerDemarcater(ps[0], ps[1]);
        }
    }
}
