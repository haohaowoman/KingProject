using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic
{
    /// <summary>
    /// 定义了两个不同量纲单位的数值转化。
    /// </summary>
    public interface IUnitValueConverter
    {
        string SourceUnit { get; set; }

        string TargetUnit { get; set; }

        double Convert(double src);
    }
}
