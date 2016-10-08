using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService
{
    /// <summary>
    /// Some experiment area base operate methods.
    /// </summary>
    public interface IExpAreaOperator
    {
        bool RegistNewExpArea(ExperimentArea nArea);

        bool DischargeExpArea(string eaLabel);

        bool DischargeExpArea(ExperimentArea area);
    }
}
