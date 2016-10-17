using LabMCESystem.BaseSerivce.LabTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.Task;

namespace LabMCESystem.BaseSerivce
{
    /// <summary>
    /// 任务集散中心实现类
    /// </summary>
    public class TaskDistributeCenter : IController, IProductTask, IControllerConnect
    {
        public IController Controller
        {
            get
            {
                return this;
            }
        }

        public event ExceuteMultipleSettersEventHandler ExecuteMultipleSetters;
        public event ExecuteSetterEventHandler ExecuteSetter;

        public void OutMultipleSetters(List<Setter> ss)
        {
            ExecuteMultipleSetters?.Invoke(this, ss);
        }

        public void OutSetter(Setter s)
        {
            ExecuteSetter?.Invoke(this, s);
        }
    }
}
