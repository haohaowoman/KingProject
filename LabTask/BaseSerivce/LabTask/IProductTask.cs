using LabMCESystem.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.BaseSerivce.LabTask
{
    public interface IProductTask
    {
        void OutSetter(Setter s);

        void OutMultipleSetters(List<Setter> ss);


    }
}
