using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mcLogic.Execute
{
    /// <summary>
    /// 定义拥有数据反馈的输出的方法
    /// </summary>
    public interface IDataFeedback
    {
        /// <summary>
        /// 获取/设置反馈数据
        /// </summary>
        double FedbackData { get; set; }

        /// <summary>
        /// 需要更新反馈数据时发生
        /// </summary>
        event UpdateFedbackValEventHandler UpdateFedback;

        /// <summary>
        /// 反馈超出安全范围时发生
        /// </summary>
        event FedbackDataOutOfSafeRangeEventHandler FedbackDataOutOfSafeRange;
    }

    /// <summary>
    /// 更新反馈数据事件委托
    /// </summary>
    /// <param name="sender"></param>
    public delegate void UpdateFedbackValEventHandler(IDataFeedback sender);

    /// <summary>
    /// 反馈数据超出Executer安全范围事件委托
    /// </summary>
    /// <param name="sender">Executer事件源</param>
    /// <param name="nFedback">被设定的反馈</param>
    public delegate void FedbackDataOutOfSafeRangeEventHandler(object sender, double eFedback);
}
