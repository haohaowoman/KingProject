using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Logic.Execute
{
    /// <summary>
    /// 拥有数据反馈的输出
    /// </summary>
    public interface IDataFedback
    {
        /// <summary>
        /// 获取/设置反馈数据
        /// </summary>
        float FedbackVal { get; set; }

    }


}
