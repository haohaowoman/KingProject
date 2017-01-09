using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Execute;
namespace mcLogic.Execute.Watcher
{
    /// <summary>
    /// 表示执行器监视数据
    /// </summary>
    public struct EWatcherData
    {
        public DateTime Time { get; set; }

        public double ExecuteValue { get; set; }

        public double FedbackValue { get; set; }

    }

    /// <summary>
    /// 用于实现对执行器的监控
    /// </summary>
    public class ExecuteWatcher
    {
        /// <summary>
        /// 获取/设置
        /// </summary>
        public Executer TargetExecuter { get; private set; }

        /// <summary>
        /// 获取所监控的数据
        /// </summary>
        public List<EWatcherData> WatchData { get; private set; }
        
        /// <summary>
        /// 指定执行器创建监控对象
        /// </summary>
        /// <param name="ex">执行器</param>
        public ExecuteWatcher(Executer ex)
        {
            ex.ExecuteChanged += Ex_ExecuteChanged;
            TargetExecuter = ex;
            WatchData = new List<EWatcherData>();
        }

        /// <summary>
        /// 调用以显示监视对话框
        /// </summary>
        public void ShowWatcherDialog()
        {

        }

        private void Ex_ExecuteChanged(object sender, double executedVal)
        {
            var d = new EWatcherData() { Time = DateTime.Now, ExecuteValue = executedVal };

            var fe = TargetExecuter as IDataFeedback;
            if (fe != null)
            {
                d.FedbackValue = fe.FedbackData;
            }

            WatchData.Add(d);

            // 最大只储存1000个数据
            if (WatchData.Count > 1000)
            {
                WatchData.RemoveAt(0);
            }
        }
    }
}
