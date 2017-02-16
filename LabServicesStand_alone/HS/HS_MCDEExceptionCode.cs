using LabMCESystem.EException;
using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabMCESystem.Servers.HS
{
    public partial class HS_MeasCtrlDevice
    {
        /// <summary>
        /// 初始化各个异常通道，并将其加入Watcher。
        /// </summary>
        public void InitialEExceptions()
        {
            ChannelExceptionSrc ces = new ChannelExceptionSrc()
            {
                Label = "风机报警",
                Summary = "一冷风机出现故障报警，请立即处理。",
                ExcepType = EExcepType.Fault,
                DealOpinion = "见意立即停止一冷段试验，关闭风机。"
            };
            
            StatusChannelTrigger sct = new StatusChannelTrigger(Device["风机变频器故障"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);

            //------------------
            
            sct = new StatusChannelTrigger(Device["风机电机前轴承温度报警"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            
            sct = new StatusChannelTrigger(Device["风机电机后轴承温度报警"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            sct = new StatusChannelTrigger(Device["风机电机绕组U温度报警"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            sct = new StatusChannelTrigger(Device["风机电机绕组V温度报警"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            
            sct = new StatusChannelTrigger(Device["风机电机绕组W温度报警"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            
            sct = new StatusChannelTrigger(Device["风机电机前轴振动报警"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            
            sct = new StatusChannelTrigger(Device["风机电机后轴振动报警"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            
            sct = new StatusChannelTrigger(Device["风机电机前轴承温度超温停机"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            sct = new StatusChannelTrigger(Device["风机电机后轴承温度超温停机"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            sct = new StatusChannelTrigger(Device["风机电机绕组U温度超温停机"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            sct = new StatusChannelTrigger(Device["风机电机绕组V温度超温停机"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            sct = new StatusChannelTrigger(Device["风机电机绕组W温度超温停机"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            sct = new StatusChannelTrigger(Device["风机电机前轴振动停机"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            sct = new StatusChannelTrigger(Device["风机电机后轴振动停机"] as StatusChannel);
            sct.ActionStatus = true;
            ces.Triggers.Add(sct);
            //------------------
            ExceptionWatcher.AddExceptionSrc(ces);

            // 实验室温度报警
            ces = new ChannelExceptionSrc()
            {
                Label = "实验室温度报警",
                Summary = "实验室温度过高",
                ExcepType = EExcepType.Fault,
                DealOpinion = "请立即关闭所有相关加热设备，并加大气体流量，以实现实验室快速降温。"
            };

            var labTempTrigger = new OutRangeTrigger(Device["工作室温度TT6-1"] as IAnalogueMeasure);
            labTempTrigger.InRange = new QRange(0, LabFaultTemprature);
            ces.Triggers.Add(labTempTrigger);

            labTempTrigger = new OutRangeTrigger(Device["工作室温度TT6-2"] as IAnalogueMeasure);
            labTempTrigger.InRange = new QRange(0, LabFaultTemprature);
            ces.Triggers.Add(labTempTrigger);

            labTempTrigger = new OutRangeTrigger(Device["工作室温度TT6-3"] as IAnalogueMeasure);
            labTempTrigger.InRange = new QRange(0, LabFaultTemprature);
            ces.Triggers.Add(labTempTrigger);

            labTempTrigger = new OutRangeTrigger(Device["工作室温度TT6-4"] as IAnalogueMeasure);
            labTempTrigger.InRange = new QRange(0, LabFaultTemprature);
            ces.Triggers.Add(labTempTrigger);

            ExceptionWatcher.AddExceptionSrc(ces);

            // 加热器报警
            ces = new ChannelExceptionSrc()
            {
                Label = "加热器报警",
                Summary = "电炉加热器出现故障报警",
                ExcepType = EExcepType.Fault,
                DealOpinion = "请增大加热器的通过气流，并立即关闭所有相关加热设备。"                
            };

            var heaterETrigger = new StatusChannelTrigger(Device["RL_1#_FAULT"] as StatusChannel);
            heaterETrigger.ActionStatus = true;
            ces.Triggers.Add(heaterETrigger);

            heaterETrigger = new StatusChannelTrigger(Device["RL_2#_FAULT"] as StatusChannel);
            heaterETrigger.ActionStatus = true;
            ces.Triggers.Add(heaterETrigger);

            heaterETrigger = new StatusChannelTrigger(Device["RL_3#_FAULT"] as StatusChannel);
            heaterETrigger.ActionStatus = true;
            ces.Triggers.Add(heaterETrigger);

            heaterETrigger = new StatusChannelTrigger(Device["RL_4#_FAULT"] as StatusChannel);
            heaterETrigger.ActionStatus = true;
            ces.Triggers.Add(heaterETrigger);

            heaterETrigger = new StatusChannelTrigger(Device["RL_5#_FAULT"] as StatusChannel);
            heaterETrigger.ActionStatus = true;
            ces.Triggers.Add(heaterETrigger);
            
            heaterETrigger = new StatusChannelTrigger(Device["ELL_1#_FAULT"] as StatusChannel);
            heaterETrigger.ActionStatus = true;
            ces.Triggers.Add(heaterETrigger);

            heaterETrigger = new StatusChannelTrigger(Device["ELL_2#_FAULT"] as StatusChannel);
            heaterETrigger.ActionStatus = true;
            ces.Triggers.Add(heaterETrigger);

            ExceptionWatcher.AddExceptionSrc(ces);
        }

        /// <summary>
        /// 重写异常报告。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExceptionWatcherReport(WatcherReportEExceptionEventArgs e)
        {

#if DEBUG
            //Console.WriteLine($"{e.EExceptionInfor} was actived on {e.AppearTime} from {e.Source} of {e.OriginalSource}"); 
#endif
            base.OnExceptionWatcherReport(e);
        }
    }
}
