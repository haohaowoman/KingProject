using LabMCESystem.ETask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseService.LabTask
{
    public interface IProductTask
    {
        void OutSetter(TaskSetter s);

        void OutMultipleSetters(List<TaskSetter> ss);


    }
}
