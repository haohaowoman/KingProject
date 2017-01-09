using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem
{
    /// <summary>
    /// 代表属于工程量纲的范围。
    /// </summary>
    [Serializable]
    public struct QRange : IEquatable<QRange>
    {
        /// <summary>
        /// 获取/设置范围低值。
        /// </summary>
        public double Low { get; set; }
        /// <summary>
        /// 获取/设置范围高值。
        /// </summary>
        public double Height { get; set; }

        public QRange(double l, double h)
        {
            Low = l;
            Height = h;
        }

        /// <summary>
        /// 确定实例是否是一个无效范围。
        /// </summary>
        public bool IsInvalid
        {
            get
            {
                return Height - Low == 0.0;
            }
        }

        public bool Equals(QRange other)
        {
            return Low == other.Low && Height == other.Height;
        }

        /// <summary>
        /// 确定一个值是否在本范围内。
        /// </summary>
        /// <param name="v">需要确定的值。</param>
        /// <returns>在范围内返回true。</returns>
        public bool SureInRange(double v)
        {
            return v <= Height && v >= Low;
        }
    }

    /// <summary>
    /// 封装有单位及范围的接口。
    /// </summary>
    public interface IUnitRange
    {
        /// <summary>
        /// 获取/设置量纲单位。
        /// </summary>
        string Unit { get; set; }
        /// <summary>
        /// 获取/设置量纲范围。
        /// </summary>
        QRange Range { get; set; }
    }
}
