using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newopcdll
{

    static public class OpcTools
    {
        public static List<string> DO { get; private set; }
        public static List<string> AO { get; private set; }
        public static List<string> DI { get; private set; }
        public static List<string> AI { get; private set; }
        public static List<string> HMI{ get; private set; }
                
        static OpcTools()
        {

             DO = new List<string>();

            DO.Add("EV0101_OPEN");
            DO.Add("EV0103_OPEN");
            DO.Add("EV0102_OPEN");
            DO.Add("EV0104_OPEN");
            DO.Add("FJ_START");
            DO.Add("FJ_STOP");
            DO.Add("FJ_WARN");
            DO.Add("HEATER_WARN");
            DO.Add("LAB_TT_WARN");
            DO.Add("1#DOOR_WARN");
            DO.Add("2#DOOR_WARN");
            DO.Add("3#DOOR_WARN");
            DO.Add("EV0101_CLOSE");
            DO.Add("EV0103_CLOSE");
            DO.Add("EV0102_CLOSE");
            DO.Add("EV0104_CLOSE");
            DO.Add("RL_1#_START");
            DO.Add("RL_2#_START");
            DO.Add("RL_3#_START");
            DO.Add("RL_4#_START");
            DO.Add("RL_5#_START");
            DO.Add("ELL_1#_START");
            DO.Add("ELL_2#_START");


            DI = new List<string>();

            DI.Add("FJ_POWER");
            DI.Add("FJ_AUTO");
            DI.Add("FJ_BPQ_READY");
            DI.Add("FJ_BPQ_RUN");
            DI.Add("FJ_BPQ_STOP");
            DI.Add("FJ_BPQ_FAULT");
            DI.Add("POWER");
            DI.Add("RL_1#_FAULT");
            DI.Add("RL_1#_RUN");
            DI.Add("RL_1#_AUTO");
            DI.Add("RL_2#_FAULT");
            DI.Add("RL_2#_RUN");
            DI.Add("RL_2#_AUTO");
            DI.Add("1#DOOR_DETECTION");
            DI.Add("FJ_QZC_TT_ALARM");
            DI.Add("FJ_HZC_TT_ALARM");
            DI.Add("FJ_WINDING_U_TT_ALARM");
            DI.Add("FJ_WINDING_V_TT_ALARM");
            DI.Add("FJ_WINDING_W_TT_ALARM");
            DI.Add("FJ_Q_ZD_ALARM");
            DI.Add("FJ_H_ZD_ALARM");
            DI.Add("FJ_Q_TT_WARN");
            DI.Add("FJ_H_TT_WARN");
            DI.Add("FJ_WINDING_U_TT_WARN");
            DI.Add("FJ_WINDING_V_TT_WARN");
            DI.Add("FJ_WINDING_W_TT_WARN");
            DI.Add("FJ_Q_ZD_WARN");
            DI.Add("FJ_H_ZD_WARN");
            DI.Add("2#DOOR_DETECTION");
            DI.Add("3#DOOR_DETECTION");
            DI.Add("RL_3#_FAULT");
            DI.Add("RL_3#_RUN");
            DI.Add("RL_3#_AUTO");
            DI.Add("RL_4#_FAULT");
            DI.Add("RL_4#_RUN");
            DI.Add("RL_4#_AUTO");
            DI.Add("RL_5#_FAULT");
            DI.Add("RL_5#_RUN");
            DI.Add("RL_5#_AUTO");
            DI.Add("ELL_1#_FAULT");
            DI.Add("ELL_1#_RUN");
            DI.Add("ELL_1#_AUTO");
            DI.Add("ELL_2#_FAULT");
            DI.Add("ELL_2#_RUN");
            DI.Add("ELL_2#_AUTO");
            DI.Add("EV0101_OPENED");
            DI.Add("EV0101_CLOSED");
            DI.Add("EV0103_OPENED");
            DI.Add("EV0103_CLOSED");
            DI.Add("EV0102_OPENED");
            DI.Add("EV0102_CLOSED");
            DI.Add("EV0104_OPENED");
            DI.Add("EV0104_CLOSED");


            AI = new List<string>();
            AI.Add("PV0103_FEEDBACK");
            AI.Add("PV0104_FEEDBACK");
            AI.Add("PV0107_FEEDBACK");
            AI.Add("PV0108_FEEDBACK");
            AI.Add("PV0109_FEEDBACK");
            AI.Add("FV0101A_FEEDBACK");
            AI.Add("FV0101B_FEEDBACK");
            AI.Add("FV0102A_FEEDBACK");
            AI.Add("FV0102B_FEEDBACK");
            AI.Add("FJ_SPEED");
            AI.Add("FJ_ELECCURRENT");
            AI.Add("FJ_QZC_TT");
            AI.Add("FJ_HZC_TT");
            AI.Add("FJ_WINDING_U_TT");
            AI.Add("FJ_WINDING_V_TT");
            AI.Add("FJ_WINDING_W_TT");
            AI.Add("FJ_Q_ZD");
            AI.Add("FJ_H_ZD");
            AI.Add("LAB_TT6-1");
            AI.Add("LAB_TT6-2");
            AI.Add("LAB_TT6-3");
            AI.Add("LAB_TT6-4");




             AO = new List<string>();
            AO.Add("PV0103_SET");
            AO.Add("PV0104_SET");
            AO.Add("PV0107_SET");
            AO.Add("PV0108_SET");
            AO.Add("PV0109_SET");
            AO.Add("FV0101A_SET");
            AO.Add("FV0101B_SET");
            AO.Add("FV0102A_SET");
            AO.Add("FV0102B_SET");
            AO.Add("FJ_PL_SET");

             HMI = new List<string>();
            HMI.Add("HMI.PV0103_HMI");
            HMI.Add("HMI.PV0104_HMI");
            HMI.Add("HMI.PV0107_HMI");
            HMI.Add("HMI.PV0108_HMI");
            HMI.Add("HMI.PV0109_HMI");

            HMI.Add("HMI.FV0101A_HMI");
            HMI.Add("HMI.FV0101B_HMI");
            HMI.Add("HMI.FV0102A_HMI");
            HMI.Add("HMI.FV0102B_HMI");

            HMI.Add("HMI.PV0103_SET_HMI");
            HMI.Add("HMI.PV0104_SET_HMI");
            HMI.Add("HMI.PV0107_SET_HMI");
            HMI.Add("HMI.PV0108_SET_HMI");
            HMI.Add("HMI.PV0109_SET_HMI");
            HMI.Add("HMI.FV0101A_SET_HMI");
            HMI.Add("HMI.FV0101B_SET_HMI");
            HMI.Add("HMI.FV0102A_SET_HMI");
            HMI.Add("HMI.FV0102B_SET_HMI");























        }
    }
}
