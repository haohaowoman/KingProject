using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.LabElement
{
    /// <summary>
    /// 试验段/试验区域
    /// </summary>
    [Serializable]
    public class ExperimentArea : LabGroupElement<ExperimentPoint>
    {
        public ExperimentArea()
        {

        }

        public ExperimentArea(string areaName)
        {
            Label = areaName;
        }

        #region Properties

        #endregion

        #region Override

        #endregion
    }
}

