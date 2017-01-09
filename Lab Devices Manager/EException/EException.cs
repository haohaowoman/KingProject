using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.EException
{
    /// <summary>
    /// 异常类型。
    /// </summary>
    [Serializable]
    public enum EExcepType
    {
        /// <summary>
        /// 警告的异常。
        /// </summary>
        Warning,
        /// <summary>
        /// 故障的异常。
        /// </summary>
        Fault,
    }

    /// <summary>
    /// 定义试验异常的基类。
    /// </summary>
    [Serializable]
    public class EException : LabElement.LabElement,
        IComparable<EException>, ICloneable,
        IEquatable<EException>
    {
        /// <summary>
        /// 获取/设置异常的处理意见。
        /// </summary>
        public string DealOpinion { get; set; }
        /// <summary>
        /// 获取/设置异常的所属类型。
        /// </summary>
        public EExcepType ExcepType { get; set; } = EExcepType.Fault;

        public object Clone()
        {
            var r = new EException();
            CopyTo(r);
            return r;
        }

        public int CompareTo(EException other)
        {
            int c = -1;
            if (ExcepType == other.ExcepType)
            {
                c = Label.CompareTo(other.Label);
            }
            else if (ExcepType > other.ExcepType)
            {
                c = 1;
            }
            return c;
        }

        /// <summary>
        /// 将对象的基本属性复制到目标试验异常对象。
        /// </summary>
        /// <param name="eet">目标异常元素。</param>
        public void CopyTo(EException eet)
        {
            eet.Label = Label;
            eet.Summary = Summary;
            eet.DealOpinion = DealOpinion;
            eet.ExcepType = ExcepType;
        }

        public bool Equals(EException other)
        {
            // 确定常的基本属性相同 则相等。
            return object.ReferenceEquals(this, other) ||
                (
                Label == other.Label &&
                Summary == other.Summary &&
                ExcepType == other.ExcepType &&
                DealOpinion == other.DealOpinion
                );
        }

        public override bool Equals(object obj)
        {
            if (!ReferenceEquals(this, obj))
            {
                var ro = obj as EException;
                if (ro != null)
                {
                    return this.Equals(ro);
                }
                else
                {
                    return false;
                }

            }
            return true;
        }

        public override int GetHashCode()
        {
            StringBuilder sb = new StringBuilder(Label);
            sb.Append('#');
            sb.Append(Summary);
            sb.Append('#');
            sb.Append(ExcepType);
            sb.Append('#');
            sb.Append(DealOpinion);

            return sb.ToString().GetHashCode();
        }

        //public static bool operator ==(EException l, EException r)
        //{
        //    return l.Equals(r);
        //}

        //public static bool operator !=(EException l, EException r)
        //{
        //    return l.Equals(r);
        //}
    }
}
