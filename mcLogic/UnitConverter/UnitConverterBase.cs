using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.UnitConverter
{
    /// <summary>
    /// 单位转换器基类。
    /// </summary>
    public class UnitConverterBase :
        IUnitValueConverter,
        IEqualityComparer<UnitConverterBase>,
        IEquatable<UnitConverterBase>,
        IComparable<UnitConverterBase>,
        IComparer<UnitConverterBase>
    {
        public UnitConverterBase()
        {

        }

        public UnitConverterBase(string srcUnit, string targetUnit)
        {
            SourceUnit = srcUnit;
            TargetUnit = TargetUnit;
        }

        public string SourceUnit
        {
            get;
            set;
        }

        public string TargetUnit
        {
            get;
            set;
        }

        public int Compare(UnitConverterBase x, UnitConverterBase y)
        {
            return x.CompareTo(y);
        }

        public int CompareTo(UnitConverterBase other)
        {
            return ToString().CompareTo(other.ToString());
        }

        public virtual double Convert(double src)
        {
            return src;
        }

        public bool Equals(UnitConverterBase other)
        {
            return SourceUnit == other.SourceUnit && TargetUnit == other.TargetUnit;
        }

        public bool Equals(UnitConverterBase x, UnitConverterBase y)
        {
            return x == y;
        }

        public int GetHashCode(UnitConverterBase obj)
        {
            return obj.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format($"{SourceUnit} to {TargetUnit}");
        }

        /// <summary>
        /// 获取一个不指定单位或单位相同的转换器。
        /// </summary>
        public static IUnitValueConverter NullUnitConverter { get { return new UnitConverterBase(); } }
    }
}
