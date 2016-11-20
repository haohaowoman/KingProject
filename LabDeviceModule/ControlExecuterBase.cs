using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.Task;
using LabMCESystem.BaseService.LabTask;
namespace LabMCESystem.LabDeviceModule
{
    public abstract class ControlExecuterBase : MultipleDevUnitBase
    {
        /// <summary>
        /// 获取设备连接的任务控制器。
        /// </summary>
        public IController Controller { get; private set; }

        public void ConnectTaskDistributeCenter(IControllerConnect connecter)
        {
            // 断开之前的事件连接 
            if (Controller != null)
            {
                Controller.ExecuteSetter -= Controller_ExecuteSetter;
                Controller.ExecuteMultipleSetters -= Controller_ExecuteMultipleSetters;
            }
            Controller = connecter.Controller;

            // 连接控制事件
            if (Controller != null)
            {
                Controller.ExecuteSetter += Controller_ExecuteSetter;
                Controller.ExecuteMultipleSetters += Controller_ExecuteMultipleSetters;
            }
        }

        // 多个控制事件
        private void Controller_ExecuteMultipleSetters(object sender, List<TaskSetter> setters)
        {
            foreach (var setter in setters)
            {
                OnControlTaskReceived(ChannelSetter.FromTaskSetter(setter, Device));
            }
        }

        // 单个控制事件
        private void Controller_ExecuteSetter(object sender, TaskSetter setter)
        {
            OnControlTaskReceived(ChannelSetter.FromTaskSetter(setter, Device));
        }

        /// <summary>
        /// 重载此函数以现实控制任务逻辑。
        /// </summary>
        /// <param name="chSetter">通道设定项，如果ChannelSetter的Channel属性为null则本设备中不包含需要控制的通道。</param>
        protected abstract void OnControlTaskReceived(ChannelSetter chSetter);
    }
}
