using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    /// <summary>
    /// 定义数字滤波的方法。
    /// </summary>
    public interface IDigtalFilter<T>
    {
        T Filter(T src);
    }

    interface IRealFilter : IDigtalFilter<double>
    {
        
    }
}
