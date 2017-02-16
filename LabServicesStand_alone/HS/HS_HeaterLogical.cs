using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mcLogic.Execute;
using LabMCESystem.Servers.HS.HS_Executers;
using LabMCESystem.LabElement;

namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 在此文件进行加热器的逻辑编写。
    /// </summary>
    public partial class HS_MeasCtrlDevice
    {
        internal Dictionary<string, HS_HeaterContrller> Heaters { get; private set; } = new Dictionary<string, HS_HeaterContrller>(7);

        private void InitialHeaters()
        {
            // 热路1号加热器。
            HS_HeaterContrller heater = new HS_HeaterContrller("RL_1#Heater", "COM3", 1);

            heater.HeaterChannel = Device["HotRoad1#Heater"] as FeedbackChannel;
            var exe = new HS_ElectricHeaterExecuter(heater.HeaterChannel.Label, heater);
            exe.ExecutePredicate = HotRoadHeaterExecuterPredicate;
            heater.HeaterChannel.Execute += HeaterChannel_Execute;

            heater.HeaterChannel.Collector = exe;
            _executerMap.Add(heater.HeaterChannel.Label, exe);
            
            heater.HeaterConnectionChannel = Device["RL_1#RemoteConnection"] as StatusChannel;

            heater.HeaterFualtChannel = Device["RL_1#_FAULT"] as StatusChannel;

            heater.HeaterRunStatusChannel = Device["RL_1#_AUTO"] as StatusChannel;

            heater.HeaterReadyChannel = Device["RL_1#HEATER_READY"] as StatusChannel;

            heater.HeaterStartChannel = Device["RL_1#HEATER_HMI_START"] as StatusOutputChannel;

            heater.HeaterStopChannel = Device["RL_1#HEATER_HMI_STOP"] as StatusOutputChannel;

            Heaters.Add(heater.Caption, heater);
            /* -------------------------------------- */

            // 热路2号加热器。
            heater = new HS_HeaterContrller("RL_2#Heater", "COM4", 1);

            heater.HeaterChannel = Device["HotRoad2#Heater"] as FeedbackChannel;
            exe = new HS_ElectricHeaterExecuter(heater.HeaterChannel.Label, heater);
            exe.ExecutePredicate = HotRoadHeaterExecuterPredicate;
            heater.HeaterChannel.Execute += HeaterChannel_Execute;

            heater.HeaterChannel.Collector = exe;
            _executerMap.Add(heater.HeaterChannel.Label, exe);
            
            heater.HeaterConnectionChannel = Device["RL_2#RemoteConnection"] as StatusChannel;

            heater.HeaterFualtChannel = Device["RL_2#_FAULT"] as StatusChannel;

            heater.HeaterRunStatusChannel = Device["RL_2#_AUTO"] as StatusChannel;

            heater.HeaterReadyChannel = Device["RL_2#HEATER_READY"] as StatusChannel;

            heater.HeaterStartChannel = Device["RL_2#HEATER_HMI_START"] as StatusOutputChannel;

            heater.HeaterStopChannel = Device["RL_2#HEATER_HMI_STOP"] as StatusOutputChannel;

            Heaters.Add(heater.Caption, heater);
            /* -------------------------------------- */
            
            // 热路3号加热器。
            heater = new HS_HeaterContrller("RL_3#Heater", "COM5", 1);

            heater.HeaterChannel = Device["HotRoad3#Heater"] as FeedbackChannel;
            exe = new HS_ElectricHeaterExecuter(heater.HeaterChannel.Label, heater);
            exe.ExecutePredicate = HotRoadHeaterExecuterPredicate;
            heater.HeaterChannel.Execute += HeaterChannel_Execute;

            heater.HeaterChannel.Collector = exe;
            _executerMap.Add(heater.HeaterChannel.Label, exe);
            
            heater.HeaterConnectionChannel = Device["RL_3#RemoteConnection"] as StatusChannel;

            heater.HeaterFualtChannel = Device["RL_3#_FAULT"] as StatusChannel;

            heater.HeaterRunStatusChannel = Device["RL_3#_AUTO"] as StatusChannel;

            heater.HeaterReadyChannel = Device["RL_3#HEATER_READY"] as StatusChannel;

            heater.HeaterStartChannel = Device["RL_3#HEATER_HMI_START"] as StatusOutputChannel;

            heater.HeaterStopChannel = Device["RL_3#HEATER_HMI_STOP"] as StatusOutputChannel;

            Heaters.Add(heater.Caption, heater);
            /* -------------------------------------- */


            // 热路4号加热器。
            heater = new HS_HeaterContrller("RL_4#Heater", "COM6", 1);

            heater.HeaterChannel = Device["HotRoad4#Heater"] as FeedbackChannel;
            exe = new HS_ElectricHeaterExecuter(heater.HeaterChannel.Label, heater);
            exe.ExecutePredicate = HotRoadHeaterExecuterPredicate;
            heater.HeaterChannel.Execute += HeaterChannel_Execute;

            heater.HeaterChannel.Collector = exe;
            _executerMap.Add(heater.HeaterChannel.Label, exe);
            
            heater.HeaterConnectionChannel = Device["RL_4#RemoteConnection"] as StatusChannel;

            heater.HeaterFualtChannel = Device["RL_4#_FAULT"] as StatusChannel;

            heater.HeaterRunStatusChannel = Device["RL_4#_AUTO"] as StatusChannel;

            heater.HeaterReadyChannel = Device["RL_4#HEATER_READY"] as StatusChannel;

            heater.HeaterStartChannel = Device["RL_4#HEATER_HMI_START"] as StatusOutputChannel;

            heater.HeaterStopChannel = Device["RL_4#HEATER_HMI_STOP"] as StatusOutputChannel;

            Heaters.Add(heater.Caption, heater);
            /* -------------------------------------- */

            // 热路5号加热器。
            heater = new HS_HeaterContrller("RL_5#Heater", "COM7", 1);

            heater.HeaterChannel = Device["HotRoad5#Heater"] as FeedbackChannel;
            exe = new HS_ElectricHeaterExecuter(heater.HeaterChannel.Label, heater);
            exe.ExecutePredicate = HotRoadHeaterExecuterPredicate;
            heater.HeaterChannel.Execute += HeaterChannel_Execute;

            heater.HeaterChannel.Collector = exe;
            _executerMap.Add(heater.HeaterChannel.Label, exe);
            
            heater.HeaterConnectionChannel = Device["RL_5#RemoteConnection"] as StatusChannel;

            heater.HeaterFualtChannel = Device["RL_5#_FAULT"] as StatusChannel;

            heater.HeaterRunStatusChannel = Device["RL_5#_AUTO"] as StatusChannel;

            heater.HeaterReadyChannel = Device["RL_5#HEATER_READY"] as StatusChannel;

            heater.HeaterStartChannel = Device["RL_5#HEATER_HMI_START"] as StatusOutputChannel;

            heater.HeaterStopChannel = Device["RL_5#HEATER_HMI_STOP"] as StatusOutputChannel;

            Heaters.Add(heater.Caption, heater);
            /* -------------------------------------- */
            
            // 二冷1号加热器。
            heater = new HS_HeaterContrller("EL_1#Heater", "COM8", 1);

            heater.HeaterChannel = Device["SecendCold1#Heater"] as FeedbackChannel;
            exe = new HS_ElectricHeaterExecuter(heater.HeaterChannel.Label, heater);
            exe.ExecutePredicate = HotRoadHeaterExecuterPredicate;
            heater.HeaterChannel.Execute += HeaterChannel_Execute;

            heater.HeaterChannel.Collector = exe;
            _executerMap.Add(heater.HeaterChannel.Label, exe);
            
            heater.HeaterConnectionChannel = Device["ELL_1#RemoteConnection"] as StatusChannel;

            heater.HeaterFualtChannel = Device["ELL_1#_FAULT"] as StatusChannel;

            heater.HeaterRunStatusChannel = Device["ELL_1#_AUTO"] as StatusChannel;

            heater.HeaterReadyChannel = Device["EL_1#HEATER_READY"] as StatusChannel;

            heater.HeaterStartChannel = Device["EL_1#HEATER_HMI_START"] as StatusOutputChannel;

            heater.HeaterStopChannel = Device["EL_1#HEATER_HMI_STOP"] as StatusOutputChannel;

            Heaters.Add(heater.Caption, heater);
            /* -------------------------------------- */
            
            // 二冷2号加热器。
            heater = new HS_HeaterContrller("EL_2#Heater", "COM9", 1);

            heater.HeaterChannel = Device["SecendCold2#Heater"] as FeedbackChannel;
            exe = new HS_ElectricHeaterExecuter(heater.HeaterChannel.Label, heater);
            exe.ExecutePredicate = HotRoadHeaterExecuterPredicate;
            heater.HeaterChannel.Execute += HeaterChannel_Execute;

            heater.HeaterChannel.Collector = exe;
            _executerMap.Add(heater.HeaterChannel.Label, exe);
            
            heater.HeaterConnectionChannel = Device["ELL_2#RemoteConnection"] as StatusChannel;

            heater.HeaterFualtChannel = Device["ELL_2#_FAULT"] as StatusChannel;

            heater.HeaterRunStatusChannel = Device["ELL_2#_AUTO"] as StatusChannel;

            heater.HeaterReadyChannel = Device["EL_2#HEATER_READY"] as StatusChannel;

            heater.HeaterStartChannel = Device["EL_2#HEATER_HMI_START"] as StatusOutputChannel;

            heater.HeaterStopChannel = Device["EL_2#HEATER_HMI_STOP"] as StatusOutputChannel;

            Heaters.Add(heater.Caption, heater);
            /* -------------------------------------- */

            foreach (var ht in Heaters)
            {
                ht.Value.InitialHeater();
            }
        }

        private void ReleaseHeaters()
        {
            foreach (var heater in Heaters)
            {
                heater.Value.Dispose();
            }
        }
        // 电炉反馈通道控制事件。
        private void HeaterChannel_Execute(object sender, ControllerEventArgs e)
        {
            var ch = sender as FeedbackChannel;
            if (ch != null)
            {
                HS_ElectricHeaterExecuter exe = ch.Collector as HS_ElectricHeaterExecuter;
                if (exe != null)
                {
                    exe.TargetVal = ch.AOValue;
                    exe.ExecuteBegin();
                }
            }
             
        }

        /// <summary>
        /// 加热器的启动停止 脉冲控制执行器控制事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="executedVal"></param>
        private void HS_HeaterHMI_ExecuteChanged(object sender, double executedVal)
        {
            var pulseExe = sender as SimplePulseExecuter;
            if (pulseExe != null)
            {
                HeaterHMISetGroup.Write(pulseExe.DesignMark,
                    pulseExe.NextPulseBit == PulseBit.HighBit ? true : false);
#if DEBUG
                Console.WriteLine($"Pulse executer {pulseExe} execute {pulseExe.NextPulseBit}. ");
#endif
            }
        }
    }
}
