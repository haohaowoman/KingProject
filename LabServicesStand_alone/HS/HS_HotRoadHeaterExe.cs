using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Execute;
namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 热路电炉的执行逻辑。
    /// 包括有电炉的打开，关闭等功能。
    /// </summary>
    class HS_HotRoadHeaterExe : HS_ElectricHeaterExecuter
    {
        public HS_HotRoadHeaterExe(string designMark, HS_MeasCtrlDevice dev) : base(designMark, dev)
        {
            UpdateFedback += HS_HotRoadHeaterExe_UpdateFedback;

            ExecuteChanged += HS_HotRoadHeaterExe_ExecuteChanged;
        }

        #region Operators
        // 热边电炉执行输出事件。
        private void HS_HotRoadHeaterExe_ExecuteChanged(object sender, double executedVal)
        {
            

        }

        // 热边电炉反馈更新事件。
        private void HS_HotRoadHeaterExe_UpdateFedback(IDataFeedback sender)
        {
            // 读取热边空气入口温度。
            System.Diagnostics.Debug.Assert(HS_Device != null);

            //HS_Device.ValueReader.Read()

        }

        protected override void CloseHeatersFlow()
        {
            // 在些添加热边加热器的关闭流程逻辑。
        }

        protected override void OpenHeatersFlow()
        {
            // 在此添加热边加热器的打开流程逻辑。
        } 

        #endregion
    }
}
