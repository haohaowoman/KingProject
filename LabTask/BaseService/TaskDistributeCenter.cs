using LabMCESystem.BaseService.LabTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.Task;

namespace LabMCESystem.BaseService
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

        public void OutMultipleSetters(List<TaskSetter> ss)
        {
            ExecuteMultipleSetters?.Invoke(this, ss);
        }

        public void OutSetter(TaskSetter s)
        {
            ExecuteSetter?.Invoke(this, s);
        }
    }
}
