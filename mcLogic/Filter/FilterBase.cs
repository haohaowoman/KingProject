using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Filter
{
    /// <summary>
    /// 定义滤波器的抽象基类，如需要使用此库中的滤波方法需要从此类派生。
    /// </summary>
    public abstract class FilterBase : IRealFilter, IDigtalFilter<double[]>
    {
        public FilterBase()
        {
            FilterBase[] a = new FilterBase[45];
        }

        public double[] Filter(double[] src)
        {
            throw new NotImplementedException();
        }

        public double Filter(double src)
        {
            throw new NotImplementedException();

        }
    }
}
