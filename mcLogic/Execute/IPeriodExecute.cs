using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Logic.Execute
{
    /// <summary>
    /// 定义可周期性地执行方法
    /// </summary>
    public interface IPeriodExecute
    {
        /// <summary>
        /// 获取/设置周期时间，以毫秒为单位
        /// </summary>
        double PeriodInterval { get; set; }

        /// <summary>
        /// 开始周期执行
        /// </summary>
        void Start();

        /// <summary>
        /// 停止当前周期执行，并恢复到默认状态
        /// </summary>
        void Stop();

        /// <summary>
        /// 暂停当前周期执行
        /// </summary>
        void Suspend();

        /// <summary>
        /// 继续当前已暂定的周期执行
        /// </summary>
        void Resum();

        /// <summary>
        /// 获取当前执行器是否为有效
        /// </summary>
        bool Enabled { get; }
    }
}
