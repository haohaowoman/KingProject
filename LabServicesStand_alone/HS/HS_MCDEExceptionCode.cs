using LabMCESystem.EException;
using LabMCESystem.LabElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Execute;
namespace LabMCESystem.Servers.HS
{
    public partial class HS_MeasCtrlDevice
    {


        /// <summary>
        /// 初始化各个异常通道，并将其加入Watcher。
        /// </summary>
        public void InitialEExceptions()
        {
            #region 风机报警 停机

            ChannelExceptionSrc ces = new ChannelExceptionSrc()
            {
                Label = "风机报警",
                Summary = "一冷风机出现故障报警，请立即处理。",
                ExcepType = EExcepType.Fault,
                DealOpinion = "建议立即停止一冷段工作，停止试验，关闭风机。"
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
            double wTemp = 80;
            double wZd = 7.1;

            var fFTg = new OutRangeTrigger(Device["FJ_QZC_TT_HMI"] as IAnalogueMeasure);
            fFTg.InRange = new QRange(0, wTemp);
            ces.Triggers.Add(fFTg);

            fFTg = new OutRangeTrigger(Device["FJ_HZC_TT_HMI"] as IAnalogueMeasure);
            fFTg.InRange = new QRange(0, wTemp);
            ces.Triggers.Add(fFTg);

            fFTg = new OutRangeTrigger(Device["FJ_WINDING_U_TT_HMI"] as IAnalogueMeasure);
            fFTg.InRange = new QRange(0, wTemp);
            ces.Triggers.Add(fFTg);

            fFTg = new OutRangeTrigger(Device["FJ_WINDING_V_TT_HMI"] as IAnalogueMeasure);
            fFTg.InRange = new QRange(0, wTemp);
            ces.Triggers.Add(fFTg);

            fFTg = new OutRangeTrigger(Device["FJ_WINDING_W_TT_HMI"] as IAnalogueMeasure);
            fFTg.InRange = new QRange(0, wTemp);
            ces.Triggers.Add(fFTg);

            fFTg = new OutRangeTrigger(Device["FJ_Q_ZD_HMI"] as IAnalogueMeasure);
            fFTg.InRange = new QRange(0, wZd);
            ces.Triggers.Add(fFTg);

            fFTg = new OutRangeTrigger(Device["FJ_H_ZD_HMI"] as IAnalogueMeasure);
            fFTg.InRange = new QRange(0, wZd);
            ces.Triggers.Add(fFTg);

            //------------------
            Action<WatcherReportEExceptionEventArgs> ne = DealFanFaultLogic;
            ExceptionWatcher.AddExceptionSrc(ces, ne);

            #endregion

            #region 风机警告
            wTemp = 60;
            wZd = 6.4;
            ces = new ChannelExceptionSrc()
            {
                Label = "风机警告",
                Summary = "一冷风机出现警告，检查风机振动、电流、温度等参数。",
                ExcepType = EExcepType.Warning,
                DealOpinion = "建议请降低风机转速。"
            };

            var fWTg = new OutRangeTrigger(Device["FJ_QZC_TT_HMI"] as IAnalogueMeasure);
            fWTg.InRange = new QRange(0, wTemp);
            ces.Triggers.Add(fWTg);

            fWTg = new OutRangeTrigger(Device["FJ_HZC_TT_HMI"] as IAnalogueMeasure);
            fWTg.InRange = new QRange(0, wTemp);
            ces.Triggers.Add(fWTg);

            fWTg = new OutRangeTrigger(Device["FJ_WINDING_U_TT_HMI"] as IAnalogueMeasure);
            fWTg.InRange = new QRange(0, wTemp);
            ces.Triggers.Add(fWTg);

            fWTg = new OutRangeTrigger(Device["FJ_WINDING_V_TT_HMI"] as IAnalogueMeasure);
            fWTg.InRange = new QRange(0, wTemp);
            ces.Triggers.Add(fWTg);

            fWTg = new OutRangeTrigger(Device["FJ_WINDING_W_TT_HMI"] as IAnalogueMeasure);
            fWTg.InRange = new QRange(0, wTemp);
            ces.Triggers.Add(fWTg);

            fWTg = new OutRangeTrigger(Device["FJ_Q_ZD_HMI"] as IAnalogueMeasure);
            fWTg.InRange = new QRange(0, wZd);
            ces.Triggers.Add(fWTg);

            fWTg = new OutRangeTrigger(Device["FJ_H_ZD_HMI"] as IAnalogueMeasure);
            fWTg.InRange = new QRange(0, wZd);
            ces.Triggers.Add(fWTg);

            ExceptionWatcher.AddExceptionSrc(ces);
            #endregion

            // 实验室温度报警
            ces = new ChannelExceptionSrc()
            {
                Label = "实验室温度报警",
                Summary = "实验室温度过高",
                ExcepType = EExcepType.Fault,
                DealOpinion = "请立即关闭所有相关加热设备，并加大气体流量，以实现实验室快速降温。"
            };

            var labTempTrigger = new OutRangeTrigger(Device["LAB_TT6-1_HMI"] as IAnalogueMeasure);
            labTempTrigger.InRange = new QRange(0, LabFaultTemprature);
            ces.Triggers.Add(labTempTrigger);

            labTempTrigger = new OutRangeTrigger(Device["LAB_TT6-2_HMI"] as IAnalogueMeasure);
            labTempTrigger.InRange = new QRange(0, LabFaultTemprature);
            ces.Triggers.Add(labTempTrigger);

            labTempTrigger = new OutRangeTrigger(Device["LAB_TT6-3_HMI"] as IAnalogueMeasure);
            labTempTrigger.InRange = new QRange(0, LabFaultTemprature);
            ces.Triggers.Add(labTempTrigger);

            labTempTrigger = new OutRangeTrigger(Device["LAB_TT6-4_HMI"] as IAnalogueMeasure);
            labTempTrigger.InRange = new QRange(0, LabFaultTemprature);
            ces.Triggers.Add(labTempTrigger);

            ExceptionWatcher.AddExceptionSrc(ces);

            // 加热器报警
            ces = new ChannelExceptionSrc()
            {
                Label = "加热器报警",
                Summary = "电炉加热器出现故障报警",
                ExcepType = EExcepType.Fault,
                DealOpinion = "请保持加热器的通过气流，并立即关闭相关加热设备。"
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

            Action<WatcherReportEExceptionEventArgs> hl = DealHeaterFaultLogic;
            ExceptionWatcher.AddExceptionSrc(ces, hl);

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
            if (e.DealLogic != null)
            {
                var logic = e.DealLogic as Action<WatcherReportEExceptionEventArgs>;
                logic?.Invoke(e);
            }
            base.OnExceptionWatcherReport(e);

        }

        /// <summary>
        /// 风机故障处理逻辑。
        /// </summary>
        /// <param name="e"></param>
        private void DealFanFaultLogic(WatcherReportEExceptionEventArgs e)
        {
            // 停止风机电炉的控制。
            var fanCh = Device["FirstColdFan"] as FeedbackChannel;
            fanCh?.StopControllerExecute();
            // 向PLC写入系统故障。
            var sysAlaCh = Device["SysAlaram"] as StatusOutputChannel;
            sysAlaCh?.ControllerExecute();
            
        }
        /// <summary>
        /// 加热器故障处理。
        /// </summary>
        /// <param name="e"></param>
        private void DealHeaterFaultLogic(WatcherReportEExceptionEventArgs e)
        {
            Channel srcCh = e.OriginalSource as Channel;
            if (srcCh != null)
            {
                string hChLabel = null;
                switch (srcCh.Label)
                {
                    case "RL_1#_FAULT":
                        hChLabel = "HotRoad1#Heater";
                        break;
                    case "RL_2#_FAULT":
                        hChLabel = "HotRoad2#Heater";
                        break;
                    case "RL_3#_FAULT":
                        hChLabel = "HotRoad3#Heater";
                        break;
                    case "RL_4#_FAULT":
                        hChLabel = "HotRoad4#Heater";
                        break;
                    case "RL_5#_FAULT":
                        hChLabel = "HotRoad5#Heater";
                        break;
                    case "ELL_1#_FAULT":
                        hChLabel = "SecendCold1#Heater";
                        break;
                    case "ELL_2#_FAULT":
                        hChLabel = "SecendCold1#Heater";
                        break;
                    default:
                        break;
                }
                if (hChLabel != null)
                {
                    var hCh = Device[hChLabel] as FeedbackChannel;

                    hCh?.StopControllerExecute();
                }
            }
        }
    }
}
