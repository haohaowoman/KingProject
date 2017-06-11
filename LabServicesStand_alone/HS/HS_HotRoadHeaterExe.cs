using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabElement;
using mcLogic.Execute;
namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 热路电炉的执行逻辑。
    /// 包括有电炉的打开，关闭等功能。
    /// </summary>
    class HS_HotRoadHeaterExe //: HS_ElectricHeaterExecuter
    {
        public HS_HotRoadHeaterExe(string designMark,FeedbackChannel heaterChannel) //: base(designMark, heaterChannel)
        {
           
        }

        #region Operators
        
        //protected override void CloseHeatersFlow()
        //{
        //    // 在些添加热边加热器的关闭流程逻辑。
        //}

        //protected override void OpenHeatersFlow()
        //{
        //    // 在此添加热边加热器的打开流程逻辑。
        //} 

        #endregion
    }
}
