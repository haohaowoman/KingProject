using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic
{
    /// <summary>
    /// 封装了电信号到工程量的标定的方法。
    /// </summary>
    public interface IDemarcate
    {
        /// <summary>
        /// 将电信号标定为工程量。
        /// </summary>
        /// <param name="electric">电信号值。</param>
        /// <returns>经过标定的工程量值。</returns>
        double Demarcate(double electric);
    }
}
