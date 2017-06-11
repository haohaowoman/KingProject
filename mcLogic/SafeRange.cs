using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic
{
    /// <summary>
    /// 控制执行器的安全输出范围
    /// </summary>
    [Serializable]
    public struct SafeRange
    {
        /// <summary>
        /// 获取/设置范围低值
        /// </summary>
        public double Low { set; get; }
        /// <summary>
        /// 获取/设置范围高值
        /// </summary>
        public double Height { get; set; }

        public SafeRange(double l = 0, double h = 0)
        {
            if (h < l)
            {
                throw new ArgumentOutOfRangeException("安全范围的高值不能小于低值。");
            }

            Low = l;
            Height = h;
        }

        /// <summary>
        /// 判断值是否在范围内
        /// </summary>
        /// <param name="val">检测值</param>
        /// <returns>在范围内为True</returns>
        public bool IsSafeIn(double val)
        {
            return val >= Low && val <= Height;
        }

        /// <summary>
        /// 获取范围长度。
        /// </summary>
        public double Length
        {
            get { return Height - Low; }
        }
    }

}
