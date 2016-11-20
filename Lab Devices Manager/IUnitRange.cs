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
    public struct QRange
    {
        /// <summary>
        /// 获取/设置范围低值。
        /// </summary>
        public double Low { get; set; }
        /// <summary>
        /// 获取/设置范围高值。
        /// </summary>
        public double Hight { get; set; }

        public QRange(double l, double h)
        {
            Low = l;
            Hight = h;
        }
        
        /// <summary>
        /// 确定实例是否是一个无效范围。
        /// </summary>
        public bool IsInvalid
        {
            get
            {
                return Hight - Low == 0.0;
            }
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
