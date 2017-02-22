using LabMCESystem.LabElement;
using mcLogic.Execute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.EException;
namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 在此文件进行初始化方法的编写。
    /// </summary>
    public partial class HS_MeasCtrlDevice
    {

        //为OPC交互分数据读写组。
        /// <summary>
        /// opc DI 读取组。
        /// </summary>
        public const string DIGroupName = "DIGroup";

        public const string DOHMISetGroupName = "DOHMISetGroup";

        public const string EOVHMISetGroupName = "EOVHIMSetGroup";

        public const string EOVHMIGroupName = "EOVHMIGroup";

        public const string SwitchEOVHMIGroupName = "SwitchEOVHMIGroup";

        public const string SwitchEOVHMISetGroupName = "SwitchEOVHMISetGroup";

        public const string FanHMIGroupName = "FanHMIGroup";

        public const string FanHMISetGroupName = "FanHMISetGroup";

        public const string HeaterHMISetGroupName = "HeaterHMISetGroup";

        public const string HeaterHMIGroupName = "HeaterHMIGroup";

        public OPCGroup DIGroup { get; private set; } = new OPCGroup(DIGroupName);

        public OPCGroup DOHMISetGroup { get; private set; } = new OPCGroup(DOHMISetGroupName);

        public OPCGroup EOVHMISetGroup { get; private set; } = new OPCGroup(EOVHMISetGroupName);

        public OPCGroup EOVHMIGroup { get; private set; } = new OPCGroup(EOVHMIGroupName);

        public OPCGroup SwitchEOVHMISetGroup { get; private set; } = new OPCGroup(SwitchEOVHMISetGroupName);

        public OPCGroup SwitchEOVHMIGroup { get; private set; } = new OPCGroup(SwitchEOVHMIGroupName);

        public OPCGroup FanHMIGroup { get; private set; } = new OPCGroup(FanHMIGroupName);

        public OPCGroup FanHMISetGroup { get; private set; } = new OPCGroup(FanHMISetGroupName);

        public OPCGroup HeaterHMISetGroup { get; private set; } = new OPCGroup(HeaterHMISetGroupName);

        public OPCGroup HeaterHMIGroup { get; private set; } = new OPCGroup(HeaterHMIGroupName);

        //零散设备通道 这此通道只进行了创建 并未添加进行设备组。
        Dictionary<string, Channel> _disChannels = new Dictionary<string, Channel>();

        /// <summary>
        /// 初始化PLC中的DI O的状态通道。
        /// </summary>
        /// <param name="dev"></param>
        private void InitialStatusChannel(LabDevice dev)
        {
            StatusChannel sc = null;
            // 创建添加状态通道。

            #region 变频柜

            sc = dev.CreateStatusChannelIn("风机控制电源合闸");
            sc.Prompt = "FJ_POWER";
            sc.Summary = "风机控制电源全阐";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机远程控制");
            sc.Prompt = "FJ_AUTO";
            sc.Summary = "风机远程控制";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机变频器准备就绪");
            sc.Prompt = "FJ_BPQ_READY";
            sc.Summary = "风机变频器准备就绪";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机变频器运行");
            sc.Prompt = "FJ_BPQ_RUN";
            sc.Summary = "风机变频器运行";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机变频器停止");
            sc.Prompt = "FJ_BPQ_STOP";
            sc.Summary = "风机变频器停止";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机变频器故障");
            sc.Prompt = "FJ_BPQ_FAULT";
            sc.Summary = "风机变频器故障";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("总电源合阐信号");
            sc.Prompt = "POWER";
            sc.Summary = "总电源合阐信号";

            DIGroup.AddSubChannel(sc);
            /*---------------------------*/
            #endregion

            #region 热路 加热器远程控制

            sc = dev.CreateStatusChannelIn("热路1#加热器远程控制");
            sc.Prompt = "I 1.0";
            sc.Summary = "热路1#加热器远程控制";

            sc = dev.CreateStatusChannelIn("热路2#加热器远程控制");
            sc.Prompt = "I 1.1";
            sc.Summary = "热路2#加热器远程控制";

            sc = dev.CreateStatusChannelIn("热路3#加热器远程控制");
            sc.Prompt = "I 1.2";
            sc.Summary = "热路3#加热器远程控制";

            sc = dev.CreateStatusChannelIn("热路4#加热器远程控制");
            sc.Prompt = "I 1.3";
            sc.Summary = "热路4#加热器远程控制";

            sc = dev.CreateStatusChannelIn("热路5#加热器远程控制");
            sc.Prompt = "I 1.4";
            sc.Summary = "热路5#加热器远程控制";

            sc = dev.CreateStatusChannelIn("二冷1#加热器远程控制");
            sc.Prompt = "I 1.6";
            sc.Summary = "二冷1#加热器远程控制";

            sc = dev.CreateStatusChannelIn("二冷2#加热器远程控制");
            sc.Prompt = "I 1.7";
            sc.Summary = "二冷2#加热器远程控制";

            #endregion

            #region 风机报警状态

            sc = dev.CreateStatusChannelIn("风机已准备");
            sc.Prompt = "FJ_READY";
            sc.Summary = "风机已准备好";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机前轴承温度报警");
            sc.Prompt = "FJ_QZC_TT_ALARM";
            sc.Summary = "风机电机前轴承温度报警";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机后轴承温度报警");
            sc.Prompt = "FJ_HZC_TT_ALARM";
            sc.Summary = "风机电机后轴承温度报警";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机绕组U温度报警");
            sc.Prompt = "FJ_WINDING_U_TT_ALARM";
            sc.Summary = "风机电机绕组U温度报警";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机绕组V温度报警");
            sc.Prompt = "FJ_WINDING_V_TT_ALARM";
            sc.Summary = "风机电机绕组V温度报警";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机绕组W温度报警");
            sc.Prompt = "FJ_WINDING_W_TT_ALARM";
            sc.Summary = "风机电机绕组W温度报警";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机前轴振动报警");
            sc.Prompt = "FJ_Q_ZD_ALARM";
            sc.Summary = "风机电机前轴振动报警";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机后轴振动报警");
            sc.Prompt = "FJ_H_ZD_ALARM";
            sc.Summary = "风机电机后轴振动报警";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/
            #region 风机停机

            sc = dev.CreateStatusChannelIn("风机电机前轴承温度超温停机");
            sc.Prompt = "FJ_Q_TT_ALARM_STOP";
            sc.Summary = "风机电机前轴承温度超温停机";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机后轴承温度超温停机");
            sc.Prompt = "FJ_H_TT_ALARM_STOP";
            sc.Summary = "风机电机后轴承温度超温停机";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机绕组U温度超温停机");
            sc.Prompt = "FJ_WINDING_U_TT_ALARM_STOP";
            sc.Summary = "风机电机绕组U温度超温停机";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机绕组V温度超温停机");
            sc.Prompt = "FJ_WINDING_V_TT_ALARM_STOP";
            sc.Summary = "风机电机绕组V温度超温停机";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机绕组W温度超温停机");
            sc.Prompt = "FJ_WINDING_W_ALARM_STOP";
            sc.Summary = "风机电机绕组W温度超温停机";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机前轴振动停机");
            sc.Prompt = "FJ_Q_ZD_ALARM_STOP";
            sc.Summary = "风机电机前轴振动停机";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/

            sc = dev.CreateStatusChannelIn("风机电机后轴振动停机");
            sc.Prompt = "FJ_H_ZD_ALARM_STOP";
            sc.Summary = "风机电机后轴振动停机";

            FanHMIGroup.AddSubChannel(sc);
            /*---------------------------*/
            #endregion

            #endregion

            #region plc DO 可控制状态

            #region 电磁开关阀的 状态和画面操作 创建零散通道

            // plc 变量关系 将创建零散设备通道 用于控制和采集 EOV 通道数据。

            StatusChannel eovStatus = LabDevice.CreateChannel(dev, "EV0101_OPENED", ExperimentStyle.Status) as StatusChannel;
            eovStatus.Prompt = "EV0101_OPENED";
            eovStatus.Summary = "电动阀EV0101开极限";

            eovStatus.NotifyValueUpdated += (sender, e) =>
            {
                var sweov = Device["EV0101"] as IStatusExpress;
                if (sweov != null && (sender as IStatusExpress).Status)
                {
                    sweov.Status = true;
                }
            };

            _disChannels.Add(eovStatus.Label, eovStatus);

            SwitchEOVHMIGroup.AddSubChannel(eovStatus);
            /*---------------------------*/

            eovStatus = LabDevice.CreateChannel(dev, "EV0101_CLOSED", ExperimentStyle.Status) as StatusChannel;
            eovStatus.Prompt = "EV0101_CLOSED";
            eovStatus.Summary = "电动阀EV0101关极限";

            eovStatus.NotifyValueUpdated += (sender, e) =>
            {
                var sweov = Device["EV0101"] as IStatusExpress;
                if (sweov != null && (sender as IStatusExpress).Status)
                {
                    sweov.Status = false;
                }
            };

            _disChannels.Add(eovStatus.Label, eovStatus);

            SwitchEOVHMIGroup.AddSubChannel(eovStatus);
            /*---------------------------*/

            eovStatus = LabDevice.CreateChannel(dev, "EV0102_OPENED", ExperimentStyle.Status) as StatusChannel;
            eovStatus.Prompt = "EV0102_OPENED";
            eovStatus.Summary = "电动阀EV0102开极限";

            eovStatus.NotifyValueUpdated += (sender, e) =>
            {
                var sweov = Device["EV0102"] as IStatusExpress;
                if (sweov != null && (sender as IStatusExpress).Status)
                {
                    sweov.Status = true;
                }
            };

            _disChannels.Add(eovStatus.Label, eovStatus);

            SwitchEOVHMIGroup.AddSubChannel(eovStatus);
            /*---------------------------*/

            eovStatus = LabDevice.CreateChannel(dev, "EV0102_CLOSED", ExperimentStyle.Status) as StatusChannel;
            eovStatus.Prompt = "EV0102_CLOSED";
            eovStatus.Summary = "电动阀EV0102关极限";

            eovStatus.NotifyValueUpdated += (sender, e) =>
            {
                var sweov = Device["EV0102"] as IStatusExpress;
                if (sweov != null && (sender as IStatusExpress).Status)
                {
                    sweov.Status = false;
                }
            };

            _disChannels.Add(eovStatus.Label, eovStatus);

            SwitchEOVHMIGroup.AddSubChannel(eovStatus);
            /*---------------------------*/

            eovStatus = LabDevice.CreateChannel(dev, "EV0103_OPENED", ExperimentStyle.Status) as StatusChannel;
            eovStatus.Prompt = "EV0103_OPENED";
            eovStatus.Summary = "电动阀EV0103开极限";

            eovStatus.NotifyValueUpdated += (sender, e) =>
            {
                var sweov = Device["EV0103"] as IStatusExpress;
                if (sweov != null && (sender as IStatusExpress).Status)
                {
                    sweov.Status = true;
                }
            };

            _disChannels.Add(eovStatus.Label, eovStatus);

            SwitchEOVHMIGroup.AddSubChannel(eovStatus);
            /*---------------------------*/

            eovStatus = LabDevice.CreateChannel(dev, "EV0103_CLOSED", ExperimentStyle.Status) as StatusChannel;
            eovStatus.Prompt = "EV0103_CLOSED";
            eovStatus.Summary = "电动阀EV0103关极限";

            eovStatus.NotifyValueUpdated += (sender, e) =>
            {
                var sweov = Device["EV0103"] as IStatusExpress;
                if (sweov != null && (sender as IStatusExpress).Status)
                {
                    sweov.Status = false;
                }
            };

            _disChannels.Add(eovStatus.Label, eovStatus);

            SwitchEOVHMIGroup.AddSubChannel(eovStatus);
            /*---------------------------*/

            eovStatus = LabDevice.CreateChannel(dev, "EV0104_OPENED", ExperimentStyle.Status) as StatusChannel;
            eovStatus.Prompt = "EV0104_OPENED";
            eovStatus.Summary = "电动阀EV0104开极限";

            eovStatus.NotifyValueUpdated += (sender, e) =>
            {
                var sweov = Device["EV0104"] as IStatusExpress;
                if (sweov != null && !(sender as IStatusExpress).Status)
                {
                    sweov.Status = true;
                }
            };

            _disChannels.Add(eovStatus.Label, eovStatus);

            SwitchEOVHMIGroup.AddSubChannel(eovStatus);
            /*---------------------------*/

            eovStatus = LabDevice.CreateChannel(dev, "EV0104_CLOSED", ExperimentStyle.Status) as StatusChannel;
            eovStatus.Prompt = "EV0104_CLOSED";
            eovStatus.Summary = "电动阀EV0104关极限";

            eovStatus.NotifyValueUpdated += (sender, e) =>
            {
                var sweov = Device["EV0104"] as IStatusExpress;
                if (sweov != null && !(sender as IStatusExpress).Status)
                {
                    sweov.Status = false;
                }
            };

            _disChannels.Add(eovStatus.Label, eovStatus);

            SwitchEOVHMIGroup.AddSubChannel(eovStatus);
            /*---------------------------*/

            #region EOV控制通道

            // EOV控制通道
            StatusOutputChannel eovControl = LabDevice.CreateChannel(dev, "EV0101_HMI_OPEN", ExperimentStyle.StatusControl) as StatusOutputChannel;
            eovControl.Prompt = "EV0101_HMI_OPEN";
            eovControl.Summary = "EV0101画面操作打开";

            var eovExe = new SimplePulseExecuter() { DesignMark = eovControl.Prompt };
            eovExe.ExecuteOvered += EovExe_ExecuteOvered;
            eovExe.ExecuteChanged += EovExe_ExecuteChanged;

            eovControl.Controller = eovExe;

            _disChannels.Add(eovControl.Label, eovControl);

            SwitchEOVHMISetGroup.AddSubChannel(eovControl);
            /*---------------------------*/

            eovControl = LabDevice.CreateChannel(dev, "EV0101_HMI_CLOSE", ExperimentStyle.StatusControl) as StatusOutputChannel;
            eovControl.Prompt = "EV0101_HMI_CLOSE";

            eovControl.Summary = "EV0101画面操作关闭";

            eovExe = new SimplePulseExecuter() { DesignMark = eovControl.Prompt };
            eovExe.ExecuteOvered += EovExe_ExecuteOvered;
            eovExe.ExecuteChanged += EovExe_ExecuteChanged;

            eovControl.Controller = eovExe;

            _disChannels.Add(eovControl.Label, eovControl);

            SwitchEOVHMISetGroup.AddSubChannel(eovStatus);
            /*---------------------------*/

            eovControl = LabDevice.CreateChannel(dev, "EV0102_HMI_OPEN", ExperimentStyle.StatusControl) as StatusOutputChannel;
            eovControl.Prompt = "EV0102_HMI_OPEN";

            eovControl.Summary = "EV0102画面操作打开";

            eovExe = new SimplePulseExecuter() { DesignMark = eovControl.Prompt };
            eovExe.ExecuteOvered += EovExe_ExecuteOvered;
            eovExe.ExecuteChanged += EovExe_ExecuteChanged;

            eovControl.Controller = eovExe;

            _disChannels.Add(eovControl.Label, eovControl);

            SwitchEOVHMISetGroup.AddSubChannel(eovControl);
            /*---------------------------*/

            eovControl = LabDevice.CreateChannel(dev, "EV0102_HMI_CLOSE", ExperimentStyle.StatusControl) as StatusOutputChannel;
            eovControl.Prompt = "EV0102_HMI_CLOSE";

            eovControl.Summary = "EV0102画面操作关闭";

            eovExe = new SimplePulseExecuter() { DesignMark = eovControl.Prompt };
            eovExe.ExecuteOvered += EovExe_ExecuteOvered;
            eovExe.ExecuteChanged += EovExe_ExecuteChanged;

            eovControl.Controller = eovExe;

            _disChannels.Add(eovControl.Label, eovControl);

            SwitchEOVHMISetGroup.AddSubChannel(eovControl);
            /*---------------------------*/

            eovControl = LabDevice.CreateChannel(dev, "EV0103_HMI_OPEN", ExperimentStyle.StatusControl) as StatusOutputChannel;
            eovControl.Prompt = "EV0103_HMI_OPEN";

            eovControl.Summary = "EV0103画面操作打开";

            eovExe = new SimplePulseExecuter() { DesignMark = eovControl.Prompt };
            eovExe.ExecuteOvered += EovExe_ExecuteOvered;
            eovExe.ExecuteChanged += EovExe_ExecuteChanged;

            eovControl.Controller = eovExe;

            _disChannels.Add(eovControl.Label, eovControl);

            SwitchEOVHMISetGroup.AddSubChannel(eovControl);
            /*---------------------------*/

            eovControl = LabDevice.CreateChannel(dev, "EV0103_HMI_CLOSE", ExperimentStyle.StatusControl) as StatusOutputChannel;
            eovControl.Prompt = "EV0103_HMI_CLOSE";

            eovControl.Summary = "EV0103画面操作关闭";

            eovExe = new SimplePulseExecuter() { DesignMark = eovControl.Prompt };
            eovExe.ExecuteOvered += EovExe_ExecuteOvered;
            eovExe.ExecuteChanged += EovExe_ExecuteChanged;

            eovControl.Controller = eovExe;

            _disChannels.Add(eovControl.Label, eovControl);

            SwitchEOVHMISetGroup.AddSubChannel(eovControl);
            /*---------------------------*/

            eovControl = LabDevice.CreateChannel(dev, "EV0104_HMI_OPEN", ExperimentStyle.StatusControl) as StatusOutputChannel;
            eovControl.Prompt = "EV0104_HMI_OPEN";

            eovControl.Summary = "EV0104画面操作打开";

            eovExe = new SimplePulseExecuter() { DesignMark = eovControl.Prompt };
            eovExe.ExecuteOvered += EovExe_ExecuteOvered;
            eovExe.ExecuteChanged += EovExe_ExecuteChanged;

            eovControl.Controller = eovExe;

            _disChannels.Add(eovControl.Label, eovControl);

            SwitchEOVHMISetGroup.AddSubChannel(eovControl);
            /*---------------------------*/

            eovControl = LabDevice.CreateChannel(dev, "EV0104_HMI_CLOSE", ExperimentStyle.StatusControl) as StatusOutputChannel;
            eovControl.Prompt = "EV0104_HMI_CLOSE";

            eovControl.Summary = "EV0104画面操作关闭";

            eovExe = new SimplePulseExecuter() { DesignMark = eovControl.Prompt };
            eovExe.ExecuteOvered += EovExe_ExecuteOvered;
            eovExe.ExecuteChanged += EovExe_ExecuteChanged;

            eovControl.Controller = eovExe;

            _disChannels.Add(eovControl.Label, eovControl);

            SwitchEOVHMISetGroup.AddSubChannel(eovControl);
            /*---------------------------*/

            #endregion

            #region 开关阀延时报警

            // 开关阀延时报警

            StatusChannel eovWarn = LabDevice.CreateChannel(dev, "EV0101_OPENWARN", ExperimentStyle.Status) as StatusChannel;
            eovWarn.Prompt = "EV0101_OPENWARN";
            eovWarn.Summary = "EV0101打开延时报警";

            _disChannels.Add(eovWarn.Label, eovWarn);

            SwitchEOVHMIGroup.AddSubChannel(eovWarn);
            /*---------------------------*/

            eovWarn = LabDevice.CreateChannel(dev, "EV0101_CLOSEWARN", ExperimentStyle.Status) as StatusChannel;
            eovWarn.Prompt = "EV0101_CLOSEWARN";
            eovWarn.Summary = "EV0101关闭延时报警";

            _disChannels.Add(eovWarn.Label, eovWarn);

            SwitchEOVHMIGroup.AddSubChannel(eovWarn);
            /*---------------------------*/

            eovWarn = LabDevice.CreateChannel(dev, "EV0102_OPENWARN", ExperimentStyle.Status) as StatusChannel;
            eovWarn.Prompt = "EV0102_OPENWARN";
            eovWarn.Summary = "EV0102打开延时报警";

            _disChannels.Add(eovWarn.Label, eovWarn);

            SwitchEOVHMIGroup.AddSubChannel(eovWarn);
            /*---------------------------*/

            eovWarn = LabDevice.CreateChannel(dev, "EV0102_CLOSEWARN", ExperimentStyle.Status) as StatusChannel;
            eovWarn.Prompt = "EV0102_CLOSEWARN";
            eovWarn.Summary = "EV0102关闭延时报警";

            _disChannels.Add(eovWarn.Label, eovWarn);

            SwitchEOVHMIGroup.AddSubChannel(eovWarn);
            /*---------------------------*/

            eovWarn = LabDevice.CreateChannel(dev, "EV0103_OPENWARN", ExperimentStyle.Status) as StatusChannel;
            eovWarn.Prompt = "EV0103_OPENWARN";
            eovWarn.Summary = "EV0103打开延时报警";

            _disChannels.Add(eovWarn.Label, eovWarn);

            SwitchEOVHMIGroup.AddSubChannel(eovWarn);
            /*---------------------------*/

            eovWarn = LabDevice.CreateChannel(dev, "EV0103_CLOSEWARN", ExperimentStyle.Status) as StatusChannel;
            eovWarn.Prompt = "EV0103_CLOSEWARN";
            eovWarn.Summary = "EV0101关闭延时报警";

            _disChannels.Add(eovWarn.Label, eovWarn);

            SwitchEOVHMIGroup.AddSubChannel(eovWarn);
            /*---------------------------*/

            eovWarn = LabDevice.CreateChannel(dev, "EV0104_OPENWARN", ExperimentStyle.Status) as StatusChannel;
            eovWarn.Prompt = "EV0104_OPENWARN";
            eovWarn.Summary = "EV0104打开延时报警";

            _disChannels.Add(eovWarn.Label, eovWarn);

            SwitchEOVHMIGroup.AddSubChannel(eovWarn);
            /*---------------------------*/

            eovWarn = LabDevice.CreateChannel(dev, "EV0104_CLOSEWARN", ExperimentStyle.Status) as StatusChannel;
            eovWarn.Prompt = "EV0104_CLOSEWARN";
            eovWarn.Summary = "EV0104关闭延时报警";

            _disChannels.Add(eovWarn.Label, eovWarn);

            SwitchEOVHMIGroup.AddSubChannel(eovWarn);
            /*---------------------------*/

            #endregion

            #endregion

            #region 电磁开关阀 状态并控制 创建设备通道 直接逻辑

            // 电炉入口 DN50 电动开关阀， 为防止电炉干烧 紧急异常情况下需要打开此
            StatusOutputChannel sch = dev.CreateStatusOutputChannelIn("EV0101");
            sch.Prompt = "EV0101";
            sch.Summary = "EV0101开关阀开状态";

            Executer exr = new DigitalExecuter() { DesignMark = "EV0101" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 电炉出口应急状态、关机时使用 去消音坑 电动开关阀 常闭
            sch = dev.CreateStatusOutputChannelIn("EV0102");
            sch.Prompt = "EV0102";
            sch.Summary = "EV0102开关阀开状态";

            exr = new DigitalExecuter() { DesignMark = "EV0102" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 电炉入口 DN50 电动开关阀， 为防止电炉干烧 紧急异常情况下需要打开此
            sch = dev.CreateStatusOutputChannelIn("EV0103");
            sch.Prompt = "EV0103";
            sch.Summary = "EV0103开关阀开状态";

            exr = new DigitalExecuter() { DesignMark = "EV0103" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 电炉出口应急状态、关机时使用 去消音坑 电动开关阀 常闭
            sch = dev.CreateStatusOutputChannelIn("EV0104");
            sch.Prompt = "EV0104";
            sch.Summary = "EV0104开关阀开状态";

            exr = new DigitalExecuter() { DesignMark = "EV0104" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            #endregion

            #region 风机起停控制

            // 风机启动（短脉冲）
            sch = dev.CreateStatusOutputChannelIn("FJ_HMI_START");
            sch.Prompt = "FJ_HMI_START";
            sch.Summary = "风机启动";

            exr = new SimplePulseExecuter() { DesignMark = "FJ_HMI_START" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            FanHMISetGroup.AddSubChannel(sch);
            /*---------------------------*/

            // 风机停止（短脉冲）
            sch = dev.CreateStatusOutputChannelIn("FJ_HMI_STOP");
            sch.Prompt = "FJ_HMI_STOP";
            sch.Summary = "风机停止";

            exr = new DigitalExecuter() { DesignMark = "FJ_HMI_STOP" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            FanHMISetGroup.AddSubChannel(sch);
            /*---------------------------*/
            #endregion

            #region 操作台
            // 报警复位
            sch = dev.CreateStatusOutputChannelIn("报警复位");
            sch.Prompt = "RESETWARN";
            sch.Summary = "将操作台报警复位";

            exr = new SimplePulseExecuter() { DesignMark = "RESETWARN" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 复位控制事件。
            sch.Execute += (sender, e) =>
            {
                var resetCh = sender as StatusOutputChannel;
                if (resetCh != null)
                {
                    (resetCh.Controller as SimplePulseExecuter)?.ExecuteBegin();
                }
            };

            exr.ExecuteChanged += (sender, e) =>
            {
                // 执行复位脉冲。
                var pulseExe = sender as SimplePulseExecuter;
                if (pulseExe != null)
                {
                    DOHMISetGroup.Write(pulseExe.DesignMark,
                        pulseExe.NextPulseBit == PulseBit.HighBit ? true : false);
#if DEBUG
                    Console.WriteLine($"Pulse executer {pulseExe} execute {pulseExe.NextPulseBit}. ");
#endif
                }
            };

            DOHMISetGroup.AddSubChannel(sch);

            // 风机报警
            sch = dev.CreateStatusOutputChannelIn("风机报警");
            sch.Prompt = "FJ_ALARM";
            sch.Summary = "风机报警";

            exr = new DigitalExecuter() { DesignMark = "FJ_ALARM" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            DOHMISetGroup.AddSubChannel(sch);
            DIGroup.AddSubChannel(sch);
            /*---------------------------*/
            // 加热器报警
            sch = dev.CreateStatusOutputChannelIn("加热器报警");
            sch.Prompt = "HEATER_ALARM";
            sch.Summary = "加热器报警";

            exr = new DigitalExecuter() { DesignMark = "HEATER_ALARM" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            DOHMISetGroup.AddSubChannel(sch);
            DIGroup.AddSubChannel(sch);
            /*---------------------------*/
            // 实验室温度报警
            sch = dev.CreateStatusOutputChannelIn("实验室报警");
            sch.Prompt = "LAB_TT_ALARM";
            sch.Summary = "实验室报警";

            exr = new DigitalExecuter() { DesignMark = "LAB_TT_ALARM" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            DOHMISetGroup.AddSubChannel(sch);
            DIGroup.AddSubChannel(sch);
            /*---------------------------*/

            #endregion

            #endregion

            #region 各设备的远程连接在线状态
            sc = dev.CreateStatusChannelIn("PLCRemoteConnection");
            sc.Prompt = "PLCRC";
            sc.Summary = "PLC远程连接在线状态";

            sc = dev.CreateStatusChannelIn("FanRemoteConnection");
            sc.Prompt = "FunRC";
            sc.Summary = "风机远程连接在线状态";

            sc = dev.CreateStatusChannelIn("ADDevRemoteConnection");
            sc.Prompt = "ADRC";
            sc.Summary = "数据采集箱远程连接在线状态";

            sc = dev.CreateStatusChannelIn("RL_1#RemoteConnection");
            sc.Prompt = "RL_1#RC";
            sc.Summary = "热路1号加热器远程连接在线状态";

            sc = dev.CreateStatusChannelIn("RL_2#RemoteConnection");
            sc.Prompt = "RL_2#RC";
            sc.Summary = "热路2号加热器远程连接在线状态";

            sc = dev.CreateStatusChannelIn("RL_3#RemoteConnection");
            sc.Prompt = "RL_3#RC";
            sc.Summary = "热路3号加热器远程连接在线状态";

            sc = dev.CreateStatusChannelIn("RL_4#RemoteConnection");
            sc.Prompt = "RL_4#RC";
            sc.Summary = "热路4号加热器远程连接在线状态";

            sc = dev.CreateStatusChannelIn("RL_5#RemoteConnection");
            sc.Prompt = "RL_5#RC";
            sc.Summary = "热路5号加热器远程连接在线状态";

            sc = dev.CreateStatusChannelIn("ELL_1#RemoteConnection");
            sc.Prompt = "ELL_1#RC";
            sc.Summary = "二冷路1号加热器远程连接在线状态";

            sc = dev.CreateStatusChannelIn("ELL_2#RemoteConnection");
            sc.Prompt = "ELL_2#RC";
            sc.Summary = "二冷路2号加热器远程连接在线状态";

            #endregion
        }

        /// <summary>
        /// 初始化所有采集通道
        /// </summary>
        private void InitialMeasureChannels(LabDevice dev)
        {
            AnalogueMeasureChannel mCh;

            #region 热边采集卡通道

            // 热边入口流量 创建为可控制的反馈通道。

            var rlFlow = dev.CreateFeedbackChannelIn("FT0102");
            rlFlow.Unit = "Kg/h";
            rlFlow.Prompt = "06_Ch4";
            rlFlow.Summary = "热边入口流量";
            rlFlow.Range = new QRange(0, 3200);

            var sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 3200));
            sensor.Unit = "Kg/h";
            sensor.Label = "FT0102";
            sensor.SensorNumber = "FT0102 000001";

            rlFlow.Collector = sensor;
            // 热边电炉入口空气压力            
            mCh = dev.CreateAIChannelIn("PT0104");
            mCh.Unit = "KPa";
            mCh.Prompt = "03_Ch1";
            mCh.Summary = "热边电炉入口空气压力";
            mCh.Range = new QRange(0, 2500);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 2500));
            sensor.Unit = "KPa";
            sensor.Label = "PT0104";
            sensor.SensorNumber = "PT0104 000001";

            mCh.Collector = sensor;
            // 热边电炉入口空气温度            
            mCh = dev.CreateAIChannelIn("TT0102");
            mCh.Unit = "℃";
            mCh.Prompt = "01_Ch2";
            mCh.Summary = "热边电炉入口空气温度";
            mCh.Range = new QRange(0, 60);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 60));
            sensor.Unit = "℃";
            sensor.Label = "TT0102";
            sensor.SensorNumber = "TT0102 000001";

            mCh.Collector = sensor;
            // 热边电炉出口空气压力            
            mCh = dev.CreateAIChannelIn("PT0106");
            mCh.Unit = "KPa";
            mCh.Prompt = "03_Ch2";
            mCh.Summary = "热边电炉出口空气压力";
            mCh.Range = new QRange(0, 2500);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 2500));
            sensor.Unit = "KPa";
            sensor.Label = "PT0106";
            sensor.SensorNumber = "PT0104 000001";

            mCh.Collector = sensor;
            // 热边电炉出口温度
            mCh = dev.CreateAIChannelIn("TT0104");
            mCh.Unit = "℃";
            mCh.Prompt = "02_Ch4";
            mCh.Summary = "热边电炉出口空气温度";
            mCh.Range = new QRange(0, 700);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 700));
            sensor.Unit = "℃";
            sensor.Label = "TT0104";
            sensor.SensorNumber = "TT0104 000001";

            mCh.Collector = sensor;
            // 热边实验段入口空气压力            
            mCh = dev.CreateAIChannelIn("PT0108");
            mCh.Unit = "KPa";
            mCh.Prompt = "03_Ch3";
            mCh.Summary = "热边实验段入口空气压力";
            mCh.Range = new QRange(0, 2500);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 2500));
            sensor.Unit = "KPa";
            sensor.Label = "PT0108";
            sensor.SensorNumber = "PT0108 000001";

            mCh.Collector = sensor;
            // 热边实验段入口空气温度 创建为可控制反馈通道。
            var rlTemp = dev.CreateFeedbackChannelIn("TT0106");
            rlTemp.Unit = "℃";
            rlTemp.Prompt = "02_Ch5";
            rlTemp.Summary = "热边实验段入口空气温度";
            rlTemp.Range = new QRange(0, 700);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 700));
            sensor.Unit = "℃";
            sensor.Label = "TT0106";
            sensor.SensorNumber = "TT0106 000001";

            rlTemp.Collector = sensor;
            // 热边实验段出口空气压力
            mCh = dev.CreateAIChannelIn("PT0109");
            mCh.Unit = "KPa";
            mCh.Prompt = "03_Ch4";
            mCh.Summary = "热边实验段出口空气压力";
            mCh.Range = new QRange(0, 2500);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 2500));
            sensor.Unit = "KPa";
            sensor.Label = "PT0109";
            sensor.SensorNumber = "PT0109 000001";

            mCh.Collector = sensor;
            // 热边实验段出口空气温度
            mCh = dev.CreateAIChannelIn("TT0107");
            mCh.Unit = "℃";
            mCh.Prompt = "02_Ch6";
            mCh.Summary = "热边实验段出口空气温度";
            mCh.Range = new QRange(0, 700);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 700));
            sensor.Unit = "℃";
            sensor.Label = "TT0107";
            sensor.SensorNumber = "TT0107 000001";

            mCh.Collector = sensor;
            #endregion

            #region 二冷采集通道

            // 二冷入口空气流量
            var elFlow = dev.CreateFeedbackChannelIn("FT0101");
            elFlow.Unit = "Kg/h";
            elFlow.Prompt = "06_Ch3";
            elFlow.Summary = "二冷入口空气流量";
            elFlow.Range = new QRange(0, 6200);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 6200));
            sensor.Unit = "Kg/h";
            sensor.Label = "FT0101";
            sensor.SensorNumber = "FT0102 000001";

            elFlow.Collector = sensor;
            // 二冷电炉入口空气压力
            mCh = dev.CreateAIChannelIn("PT0103");
            mCh.Unit = "KPa";
            mCh.Prompt = "01_Ch6";
            mCh.Summary = "二冷电炉入口空气压力";
            mCh.Range = new QRange(0, 1000);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 1000));
            sensor.Unit = "KPa";
            sensor.Label = "PT0103";
            sensor.SensorNumber = "PT0103 000001";

            mCh.Collector = sensor;
            // 二冷电炉入口空气温度
            mCh = dev.CreateAIChannelIn("TT0101");
            mCh.Unit = "℃";
            mCh.Prompt = "01_Ch1";
            mCh.Summary = "二冷电炉入口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT0101";
            sensor.SensorNumber = "TT0101 000001";

            mCh.Collector = sensor;
            // 二冷电炉出口空气压力
            mCh = dev.CreateAIChannelIn("PT0105");
            mCh.Unit = "KPa";
            mCh.Prompt = "02_Ch1";
            mCh.Summary = "二冷电炉出口空气压力";
            mCh.Range = new QRange(0, 1000);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 1000));
            sensor.Unit = "KPa";
            sensor.Label = "PT0105";
            sensor.SensorNumber = "PT0105 000001";

            mCh.Collector = sensor;
            // 二冷电炉出口空气温度
            mCh = dev.CreateAIChannelIn("TT0103");
            mCh.Unit = "℃";
            mCh.Prompt = "01_Ch3";
            mCh.Summary = "二冷电炉出口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT0103";
            sensor.SensorNumber = "TT0103 000001";

            mCh.Collector = sensor;
            // 二冷实验段入口空气压力
            mCh = dev.CreateAIChannelIn("PT0107");
            mCh.Unit = "KPa";
            mCh.Prompt = "02_Ch2";
            mCh.Summary = "二冷实验段入口空气压力";
            mCh.Range = new QRange(0, 1000);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 1000));
            sensor.Unit = "KPa";
            sensor.Label = "PT0107";
            sensor.SensorNumber = "PT0107 000001";

            mCh.Collector = sensor;
            // 二冷实验段入口空气温度
            var elTemp = dev.CreateFeedbackChannelIn("TT0105");
            elTemp.Unit = "℃";
            elTemp.Prompt = "01_Ch4";
            elTemp.Summary = "二冷实验段入口空气温度";
            elTemp.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT0105";
            sensor.SensorNumber = "TT0105 000001";

            elTemp.Collector = sensor;
            // 二冷实验段出口空气压力
            mCh = dev.CreateAIChannelIn("PT0110");
            mCh.Unit = "KPa";
            mCh.Prompt = "02_Ch3";
            mCh.Summary = "二冷实验段出口空气压力";
            mCh.Range = new QRange(0, 1000);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 1000));
            sensor.Unit = "KPa";
            sensor.Label = "PT0110";
            sensor.SensorNumber = "PT0110 000001";

            mCh.Collector = sensor;
            // 二冷实验段出口空气温度
            mCh = dev.CreateAIChannelIn("TT0108");
            mCh.Unit = "℃";
            mCh.Prompt = "01_Ch5";
            mCh.Summary = "二冷实验段出口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT0108";
            sensor.SensorNumber = "TT0108 000001";

            mCh.Collector = sensor;
            #endregion

            #region 一冷实验段采集通道

            // 一冷入口空气流量

            // 将流量创建为可控制的反馈通道。
            var firtFlow = dev.CreateFeedbackChannelIn("FT0103");
            firtFlow.Unit = "Kg/h";
            firtFlow.Prompt = "06_Ch5";
            firtFlow.Summary = "一冷入口空气流量";
            firtFlow.Range = new QRange(0, 30000);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 30000));
            sensor.Unit = "Kg/h";
            sensor.Label = "FT0103";
            sensor.SensorNumber = "FT0103 000001";

            firtFlow.Collector = sensor;

            // 一冷入口空气压力
            mCh = dev.CreateAIChannelIn("PT01");
            mCh.Unit = "KPa";
            mCh.Prompt = "05_Ch1";
            mCh.Summary = "一冷入口空气压力";
            mCh.Range = new QRange(0, 1000);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 1000));
            sensor.Unit = "KPa";
            sensor.Label = "PT01";
            sensor.SensorNumber = "PT01 000001";

            mCh.Collector = sensor;
            // 一冷入口空气温度
            mCh = dev.CreateAIChannelIn("TT01");
            mCh.Unit = "℃";
            mCh.Prompt = "03_Ch5";
            mCh.Summary = "一冷入口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT01";
            sensor.SensorNumber = "TT01 000001";

            mCh.Collector = sensor;
            // 一冷入口空气压力
            mCh = dev.CreateAIChannelIn("PT02");
            mCh.Unit = "KPa";
            mCh.Prompt = "05_Ch2";
            mCh.Summary = "一冷入口空气压力";
            mCh.Range = new QRange(0, 1000);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 1000));
            sensor.Unit = "KPa";
            sensor.Label = "PT02";
            sensor.SensorNumber = "PT02 000001";

            mCh.Collector = sensor;
            // 一冷入口空气温度
            mCh = dev.CreateAIChannelIn("TT02");
            mCh.Unit = "℃";
            mCh.Prompt = "03_Ch6";
            mCh.Summary = "一冷入口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT02";
            sensor.SensorNumber = "TT02 000001";

            mCh.Collector = sensor;
            // 一冷出口空气压力
            mCh = dev.CreateAIChannelIn("PT03");
            mCh.Unit = "KPa";
            mCh.Prompt = "05_Ch3";
            mCh.Summary = "一冷出口空气压力";
            mCh.Range = new QRange(0, 1000);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 1000));
            sensor.Unit = "KPa";
            sensor.Label = "PT03";
            sensor.SensorNumber = "PT03 000001";

            mCh.Collector = sensor;
            // 一冷出口空气温度
            mCh = dev.CreateAIChannelIn("TT03");
            mCh.Unit = "℃";
            mCh.Prompt = "04_Ch1";
            mCh.Summary = "一冷出口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT03";
            sensor.SensorNumber = "TT03 000001";

            mCh.Collector = sensor;
            #endregion
        }

        /// <summary>
        /// 初始化PLC控制反馈通道。
        /// </summary>
        private void InitialPLCChannels(LabDevice dev)
        {
            FeedbackChannel ch;
            Executer exr;

            FeedbackChannel disCh;
            #region 热边PLC电磁阀 电动调节阀 AO IO通道

            /// 热边PLC电磁阀 电动调节阀 AO通道
            /// 
            // 过滤器出口 DN80 常阀状态 可用于系统放气 去消音坑
            ch = dev.CreateFeedbackChannelIn("PV0104");
            ch.Unit = "%";
            ch.Prompt = "HMI.PV0104_HMI";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀PVO1O4开度";

            exr = new HS_EOVPIDExecuter("HMI.PV0104_SET_HMI", 0, ch);

            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            disCh = LabDevice.CreateChannel(dev, "HMI.PV0104_SET_HMI", ExperimentStyle.Feedback) as FeedbackChannel;
            disCh.Prompt = "HMI.PV0104_SET_HMI";
            disCh.Unit = "%";
            disCh.Range = new QRange(0, 100);
            disCh.Summary = "电动调节阀PV0104画面开度设定";

            EOVHMISetGroup.AddSubChannel(disCh);

            EOVHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            // 电炉入口 DN65 需要保证系统 加热器的基本流量 流量粗调作用 保证50%常开状态
            ch = dev.CreateFeedbackChannelIn("FT0102A");
            ch.Unit = "%";
            ch.Prompt = "HMI.FV0102A_HMI";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀FT0102A开度";

            exr = new HS_EOVPIDExecuter("HMI.FV0102A_SET_HMI", 50, ch) { PipeDiameter = 65 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            disCh = LabDevice.CreateChannel(dev, "HMI.FV0102A_SET_HMI", ExperimentStyle.Feedback) as FeedbackChannel;
            disCh.Prompt = "HMI.FV0102A_SET_HMI";
            disCh.Unit = "%";
            disCh.Range = new QRange(0, 100);
            disCh.Summary = "电动调节阀FV0102A画面开度设定";

            EOVHMISetGroup.AddSubChannel(disCh);

            EOVHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            // 电炉入口 DN40 流量细调作用 
            ch = dev.CreateFeedbackChannelIn("FT0102B");
            ch.Prompt = "HMI.FV0102B_HMI";
            ch.Unit = "%";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀FT0102B开度";

            exr = new HS_EOVPIDExecuter("HMI.FV0102B_SET_HMI", 0, ch) { PipeDiameter = 40 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            disCh = LabDevice.CreateChannel(dev, "HMI.FV0102B_SET_HMI", ExperimentStyle.Feedback) as FeedbackChannel;
            disCh.Prompt = "HMI.FV0102B_SET_HMI";
            disCh.Unit = "%";
            disCh.Range = new QRange(0, 100);
            disCh.Summary = "电动调节阀FV0102B画面开度设定";

            EOVHMISetGroup.AddSubChannel(disCh);

            EOVHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            // 热边入实验段入口 DN80 电动调节阀 此处可处于常开状态
            ch = dev.CreateFeedbackChannelIn("PV0108");
            ch.Unit = "%";
            ch.Prompt = "HMI.PV0108_HMI";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀PV0108开度";

            exr = new HS_EOVPIDExecuter("HMI.PV0108_SET_HMI", 90, ch) { PipeDiameter = 80 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            disCh = LabDevice.CreateChannel(dev, "HMI.PV0108_SET_HMI", ExperimentStyle.Feedback) as FeedbackChannel;
            disCh.Prompt = "HMI.PV0108_SET_HMI";
            disCh.Unit = "%";
            disCh.Range = new QRange(0, 100);
            disCh.Summary = "电动调节阀PV0108画面开度设定";

            EOVHMISetGroup.AddSubChannel(disCh);

            EOVHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            // 热边 出实验段出口 DN80 电动调节阀 此处可处于常开状态 配合使用可 以达到流阻调节 共同调节压力与流量的作用
            ch = dev.CreateFeedbackChannelIn("PV0109");
            ch.Unit = "%";
            ch.Prompt = "HMI.PV0109_HMI";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀PV0109开度";

            exr = new HS_EOVPIDExecuter("HMI.PV0109_SET_HMI", 90, ch) { PipeDiameter = 80 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            disCh = LabDevice.CreateChannel(dev, "HMI.PV0109_SET_HMI", ExperimentStyle.Feedback) as FeedbackChannel;
            disCh.Prompt = "HMI.PV0109_SET_HMI";
            disCh.Unit = "%";
            disCh.Range = new QRange(0, 100);
            disCh.Summary = "电动调节阀PV0109画面开度设定";

            EOVHMISetGroup.AddSubChannel(disCh);

            EOVHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            #endregion

            #region 二冷 PLC控制电磁阀 AO IO通道

            // 过滤器出口 DN80  常阀状态 可用于系统放气 去消音坑
            ch = dev.CreateFeedbackChannelIn("PV0103");
            ch.Unit = "%";
            ch.Prompt = "HMI.PV0103_HMI";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀PV0103开度";

            exr = new HS_EOVPIDExecuter("HMI.PV0103_SET_HMI", 0, ch);
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            disCh = LabDevice.CreateChannel(dev, "HMI.PV0103_SET_HMI", ExperimentStyle.Feedback) as FeedbackChannel;
            disCh.Prompt = "HMI.PV0103_SET_HMI";
            disCh.Unit = "%";
            disCh.Range = new QRange(0, 100);
            disCh.Summary = "电动调节阀PV0103画面开度设定";

            EOVHMISetGroup.AddSubChannel(disCh);

            EOVHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            // 电炉入口 DN150 需要保证系统 加热器的基本流量 流量粗调作用 保证50%常开状态
            ch = dev.CreateFeedbackChannelIn("FT0101A");
            ch.Unit = "%";
            ch.Prompt = "HMI.FV0101A_HMI";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀FT0101A开度";

            exr = new HS_EOVPIDExecuter("HMI.FV0101A_SET_HMI", 50, ch) { PipeDiameter = 150 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            disCh = LabDevice.CreateChannel(dev, "HMI.FV0101A_SET_HMI", ExperimentStyle.Feedback) as FeedbackChannel;
            disCh.Prompt = "HMI.FV0101A_SET_HMI";
            disCh.Unit = "%";
            disCh.Range = new QRange(0, 100);
            disCh.Summary = "电动调节阀FV0101A_SET_HMI画面开度设定";

            EOVHMISetGroup.AddSubChannel(disCh);

            EOVHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            // 电炉入口 DN50 流量细调作用
            ch = dev.CreateFeedbackChannelIn("FT0101B");
            ch.Unit = "%";
            ch.Prompt = "HMI.FV0101B_HMI";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀FT0101B开度";

            exr = new HS_EOVPIDExecuter("HMI.FV0101B_SET_HMI", 0, ch) { PipeDiameter = 50 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            disCh = LabDevice.CreateChannel(dev, "HMI.FV0101B_SET_HMI", ExperimentStyle.Feedback) as FeedbackChannel;
            disCh.Prompt = "HMI.FV0101B_SET_HMI";
            disCh.Unit = "%";
            disCh.Range = new QRange(0, 100);
            disCh.Summary = "电动调节阀FV0101B_SET_HMI画面开度设定";

            EOVHMISetGroup.AddSubChannel(disCh);

            EOVHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            // 二冷入实验段入口 DN80 电动调节阀 此处可处于常开状态
            ch = dev.CreateFeedbackChannelIn("PV0107");
            ch.Unit = "%";
            ch.Prompt = "HMI.PV0107_HMI";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀PV0107开度";

            exr = new HS_EOVPIDExecuter("HMI.PV0107_SET_HMI", 80, ch) { PipeDiameter = 80 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            disCh = LabDevice.CreateChannel(dev, "HMI.PV0107_SET_HMI", ExperimentStyle.Feedback) as FeedbackChannel;
            disCh.Prompt = "HMI.PV0107_SET_HMI";
            disCh.Unit = "%";
            disCh.Range = new QRange(0, 100);
            disCh.Summary = "电动调节阀PV0107_SET_HMI画面开度设定";

            EOVHMISetGroup.AddSubChannel(disCh);

            EOVHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            #endregion

            #region 一冷 实验段 PLC AO IO通道

            AnalogueMeasureChannel amc;
            // 风机变频器转速反馈 控制
            amc = dev.CreateAIChannelIn("风机变频器电机转速反馈");
            amc.Unit = "r/s";
            amc.Prompt = "HMI.MOTOSPEED_HMI";
            amc.Range = new QRange(0, 5000);
            amc.Summary = "风机变频器电机转速反馈";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            // 风机变频器电流反馈
            amc = dev.CreateAIChannelIn("风机变频器电机电流反馈");
            amc.Unit = "A";
            amc.Range = new QRange(0, 10);
            amc.Summary = "风机变频器电机电流反馈";
            amc.Prompt = "HMI.MOTOCURRENT_HMI";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            #region 风机仪表箱

            // 风机前轴承温度TT1
            amc = dev.CreateAIChannelIn("风机电机前轴承温度TT1");
            amc.Unit = "℃";
            amc.Prompt = "HMI.FJ_QZC_TT_HMI";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机前轴承温度TT1";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("风机电机后轴承温度TT2");
            amc.Unit = "℃";
            amc.Prompt = "HMI.FJ_HZC_TT_HMI";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机后轴承温度TT2";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("风机电机绕组U温度TT3");
            amc.Unit = "℃";
            amc.Prompt = "HMI.FJ_WINDING_U_TT_HMI";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机绕组U温度TT3";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("风机电机绕组V温度TT4");
            amc.Unit = "℃";
            amc.Prompt = "HMI.FJ_WINDING_V_TT_HMI";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机绕组V温度TT4";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("风机电机绕组W温度TT5");
            amc.Unit = "℃";
            amc.Prompt = "HMI.FJ_WINDING_W_TT_HMI";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机绕组W温度TT5";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("风机电机前轴振动");
            amc.Prompt = "HMI.FJ_Q_ZD_HMI";
            amc.Summary = "风机电机前轴振动";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("风机电机后轴振动");
            amc.Prompt = "HMI.FJ_H_ZD_HMI";
            amc.Summary = "风机电机后轴振动";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            var fpsetch = dev.CreateAOChannelIn("风机变频器频率设定");
            fpsetch.Prompt = "HMI.FJ_PL_SET_HMI";
            fpsetch.Summary = "风机变频器频率画面设定";

            FanHMISetGroup.AddSubChannel(fpsetch);
            /*---------------------------*/

            #endregion

            #endregion

            #region 工作室温度

            amc = dev.CreateAIChannelIn("工作室温度TT6-1");
            amc.Unit = "℃";
            amc.Prompt = "HMI.LAB_TT6-1_HMI";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-1";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("工作室温度TT6-2");
            amc.Unit = "℃";
            amc.Prompt = "HMI.LAB_TT6-2_HMI";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-2";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("工作室温度TT6-3");
            amc.Unit = "℃";
            amc.Prompt = "HMI.LAB_TT6-3_HMI";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-3";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("工作室温度TT6-4");
            amc.Unit = "℃";
            amc.Prompt = "HMI.LAB_TT6-4_HMI";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-4";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            #endregion
        }

        /// <summary>
        /// 初始化PLC的交互组。
        /// </summary>
        /// <param name="dev"></param>
        private bool InitialOpcInteractGroup()
        {
            HS_PLCReadWriter.CreateOpcInteract();
            bool suc = true;
            suc &= HS_PLCReadWriter.OpenOpcInteract();
            if (suc)
            {
                suc &= DIGroup.InitialGroup();
                suc &= DOHMISetGroup.InitialGroup();
                suc &= EOVHMIGroup.InitialGroup();
                suc &= EOVHMISetGroup.InitialGroup();
                suc &= SwitchEOVHMIGroup.InitialGroup();
                suc &= SwitchEOVHMISetGroup.InitialGroup();
                suc &= FanHMIGroup.InitialGroup();
                suc &= FanHMISetGroup.InitialGroup();
                suc &= HeaterHMISetGroup.InitialGroup();
                suc &= HeaterHMIGroup.InitialGroup();

                EOVHMIGroup.Read();
            }
            return suc;
        }

        /// <summary>
        /// 关闭所有OPC交互组。
        /// </summary>
        private void CloseOpcInteractGroup()
        {
            if (HS_PLCReadWriter.OpcIsOpened)
            {
                DIGroup.CloseGroup();
                DOHMISetGroup.CloseGroup();
                EOVHMIGroup.CloseGroup();
                EOVHMISetGroup.CloseGroup();
                SwitchEOVHMIGroup.CloseGroup();
                SwitchEOVHMISetGroup.CloseGroup();
                FanHMIGroup.CloseGroup();
                FanHMISetGroup.CloseGroup();
                HeaterHMISetGroup.CloseGroup();
                HeaterHMIGroup.CloseGroup();

                HS_PLCReadWriter.CloseOpcInteract();
            }

        }

        /// <summary>
        /// 初始化所有电炉所需要的通道
        /// </summary>
        private void InitialDianluChannels(LabDevice dev)
        {
            FeedbackChannel ch;
            // 加热器图纸为给出标记 所以自定义标记

            // 热边电加热器 通道 
            ch = dev.CreateFeedbackChannelIn("HotRoad1#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "热路1#加热器";
            ch.Prompt = "HR1#H";
            
            ch = dev.CreateFeedbackChannelIn("HotRoad2#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "热路2#加热器";
            ch.Prompt = "HR2#H";
            
            ch = dev.CreateFeedbackChannelIn("HotRoad3#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "热路3#加热器";
            ch.Prompt = "HR3#H";
            
            ch = dev.CreateFeedbackChannelIn("HotRoad4#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "热路4#加热器";
            ch.Prompt = "HR4#H";
            
            ch = dev.CreateFeedbackChannelIn("HotRoad5#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "热路5#加热器";
            ch.Prompt = "HR5#H";
            
            // 二冷边加热器 通道
            ch = dev.CreateFeedbackChannelIn("SecendCold1#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "二冷路1#号加热器";
            ch.Prompt = "SC1#H";
            
            ch = dev.CreateFeedbackChannelIn("SecendCold2#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "二冷路2#号加热器";
            ch.Prompt = "SC2#H";
            
            // 加热器故障 运行/停止 远程/就地状态通道。

            // 热路1号加热器。
            StatusChannel heaterSt = dev.CreateStatusChannelIn("RL_1#_FAULT");
            heaterSt.Prompt = "RL_1#_FAULT";
            heaterSt.Summary = "热路1#加热器故障";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_1#_RUN");
            heaterSt.Prompt = "RL_1#_RUN";
            heaterSt.Summary = "热路1#加热器运行/停止";
            HeaterHMIGroup.AddSubChannel(heaterSt);
            
            heaterSt = dev.CreateStatusChannelIn("RL_1#_AUTO");
            heaterSt.Prompt = "RL_1#_AUTO";
            heaterSt.Summary = "热路1#加热器远程/就地";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_1#HEATER_READY");
            heaterSt.Prompt = "RL_1#HEATER_READY";
            heaterSt.Summary = "热路1#加热器准备好";
            HeaterHMIGroup.AddSubChannel(heaterSt);
            
            /*---------------------------------------*/

            // 热路2号加热器。
            heaterSt = dev.CreateStatusChannelIn("RL_2#_FAULT");
            heaterSt.Prompt = "RL_2#_FAULT";
            heaterSt.Summary = "热路2#加热器故障";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_2#_RUN");
            heaterSt.Prompt = "RL_2#_RUN";
            heaterSt.Summary = "热路2#加热器运行/停止";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_2#_AUTO");
            heaterSt.Prompt = "RL_2#_AUTO";
            heaterSt.Summary = "热路2#加热器远程/就地";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_2#HEATER_READY");
            heaterSt.Prompt = "RL_2#HEATER_READY";
            heaterSt.Summary = "热路2#加热器准备好";
            HeaterHMIGroup.AddSubChannel(heaterSt);
            /*---------------------------------------*/

            // 热路3号加热器。
            heaterSt = dev.CreateStatusChannelIn("RL_3#_FAULT");
            heaterSt.Prompt = "RL_3#_FAULT";
            heaterSt.Summary = "热路3#加热器故障";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_3#_RUN");
            heaterSt.Prompt = "RL_3#_RUN";
            heaterSt.Summary = "热路3#加热器运行/停止";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_3#_AUTO");
            heaterSt.Prompt = "RL_3#_AUTO";
            heaterSt.Summary = "热路1#加热器远程/就地";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_3#HEATER_READY");
            heaterSt.Prompt = "RL_3#HEATER_READY";
            heaterSt.Summary = "热路3#加热器准备好";
            HeaterHMIGroup.AddSubChannel(heaterSt);
            /*---------------------------------------*/

            // 热路4号加热器。
            heaterSt = dev.CreateStatusChannelIn("RL_4#_FAULT");
            heaterSt.Prompt = "RL_4#_FAULT";
            heaterSt.Summary = "热路4#加热器故障";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_4#_RUN");
            heaterSt.Prompt = "RL_4#_RUN";
            heaterSt.Summary = "热路4#加热器运行/停止";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_4#_AUTO");
            heaterSt.Prompt = "RL_4#_AUTO";
            heaterSt.Summary = "热路4#加热器远程/就地";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_4#HEATER_READY");
            heaterSt.Prompt = "RL_4#HEATER_READY";
            heaterSt.Summary = "热路4#加热器准备好";
            HeaterHMIGroup.AddSubChannel(heaterSt);
            /*---------------------------------------*/

            // 热路5号加热器。
            heaterSt = dev.CreateStatusChannelIn("RL_5#_FAULT");
            heaterSt.Prompt = "RL_5#_FAULT";
            heaterSt.Summary = "热路5#加热器故障";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_5#_RUN");
            heaterSt.Prompt = "RL_5#_RUN";
            heaterSt.Summary = "热路5#加热器运行/停止";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_5#_AUTO");
            heaterSt.Prompt = "RL_5#_AUTO";
            heaterSt.Summary = "热路5#加热器远程/就地";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("RL_5#HEATER_READY");
            heaterSt.Prompt = "RL_5#HEATER_READY";
            heaterSt.Summary = "热路5#加热器准备好";
            HeaterHMIGroup.AddSubChannel(heaterSt);
            /*---------------------------------------*/

            // 二冷路1号加热器。
            heaterSt = dev.CreateStatusChannelIn("ELL_1#_FAULT");
            heaterSt.Prompt = "ELL_1#_FAULT";
            heaterSt.Summary = "二冷1#加热器故障";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("ELL_1#_RUN");
            heaterSt.Prompt = "ELL_1#_RUN";
            heaterSt.Summary = "二冷1#加热器运行/停止";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("ELL_1#_AUTO");
            heaterSt.Prompt = "ELL_1#_AUTO";
            heaterSt.Summary = "二冷1#加热器远程/就地";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("EL_1#HEATER_READY");
            heaterSt.Prompt = "EL_1#HEATER_READY";
            heaterSt.Summary = "二冷1#加热器准备好";
            HeaterHMIGroup.AddSubChannel(heaterSt);
            /*---------------------------------------*/

            // 二冷路2号加热器。
            heaterSt = dev.CreateStatusChannelIn("ELL_2#_FAULT");
            heaterSt.Prompt = "ELL_2#_FAULT";
            heaterSt.Summary = "二冷2#加热器故障";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("ELL_2#_RUN");
            heaterSt.Prompt = "ELL_2#_RUN";
            heaterSt.Summary = "二冷2#加热器运行/停止";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("ELL_2#_AUTO");
            heaterSt.Prompt = "ELL_2#_AUTO";
            heaterSt.Summary = "二冷2#加热器远程/就地";
            HeaterHMIGroup.AddSubChannel(heaterSt);

            heaterSt = dev.CreateStatusChannelIn("EL_2#HEATER_READY");
            heaterSt.Prompt = "EL_2#HEATER_READY";
            heaterSt.Summary = "二冷2#加热器准备好";
            HeaterHMIGroup.AddSubChannel(heaterSt);
            /*---------------------------------------*/

            // 加热器画面启动停止的离散控制通道。

            // 热路1号加热器。
            StatusOutputChannel hSop = dev.CreateStatusOutputChannelIn("RL_1#HEATER_HMI_START");// LabDevice.CreateChannel(dev, "RL_1#HEATER_HMI_START", ExperimentStyle.StatusControl) as StatusOutputChannel;
            hSop.Prompt = "RL_1#HEATER_HMI_START";
            hSop.Summary = "热路1#加热器画面启动";

            var hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);

            // stop
            hSop = dev.CreateStatusOutputChannelIn("RL_1#HEATER_HMI_STOP");
            hSop.Prompt = "RL_1#HEATER_HMI_STOP";
            hSop.Summary = "热路1#加热器画面停止";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);
            /*------------------------------------*/

            // 热路2号加热器。
            hSop = dev.CreateStatusOutputChannelIn("RL_2#HEATER_HMI_START");
            hSop.Prompt = "RL_2#HEATER_HMI_START";
            hSop.Summary = "热路2#加热器画面启动";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);

            // stop
            hSop = dev.CreateStatusOutputChannelIn("RL_2#HEATER_HMI_STOP");
            hSop.Prompt = "RL_2#HEATER_HMI_STOP";
            hSop.Summary = "热路2#加热器画面停止";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);

            /*------------------------------------*/

            // 热路3号加热器。
            hSop = dev.CreateStatusOutputChannelIn("RL_3#HEATER_HMI_START");
            hSop.Prompt = "RL_3#HEATER_HMI_START";
            hSop.Summary = "热路3#加热器画面启动";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);

            // stop
            hSop = dev.CreateStatusOutputChannelIn("RL_3#HEATER_HMI_STOP");
            hSop.Prompt = "RL_3#HEATER_HMI_STOP";
            hSop.Summary = "热路3#加热器画面停止";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);
            /*------------------------------------*/

            // 热路4号加热器。
            hSop = dev.CreateStatusOutputChannelIn("RL_4#HEATER_HMI_START");
            hSop.Prompt = "RL_4#HEATER_HMI_START";
            hSop.Summary = "热路4#加热器画面启动";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);

            // stop
            hSop = dev.CreateStatusOutputChannelIn("RL_4#HEATER_HMI_STOP");
            hSop.Prompt = "RL_4#HEATER_HMI_STOP";
            hSop.Summary = "热路4#加热器画面停止";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);
            /*------------------------------------*/

            // 热路5号加热器。
            hSop = dev.CreateStatusOutputChannelIn("RL_5#HEATER_HMI_START");
            hSop.Prompt = "RL_5#HEATER_HMI_START";
            hSop.Summary = "热路5#加热器画面启动";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);

            // stop
            hSop = dev.CreateStatusOutputChannelIn("RL_5#HEATER_HMI_STOP");
            hSop.Prompt = "RL_5#HEATER_HMI_STOP";
            hSop.Summary = "热路5#加热器画面停止";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);
            /*------------------------------------*/

            // 二冷路1号加热器。
            hSop = dev.CreateStatusOutputChannelIn("EL_1#HEATER_HMI_START");
            hSop.Prompt = "EL_1#HEATER_HMI_START";
            hSop.Summary = "二冷1#加热器画面启动";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);

            // stop
            hSop = dev.CreateStatusOutputChannelIn("EL_1#HEATER_HMI_STOP");
            hSop.Prompt = "EL_1#HEATER_HMI_STOP";
            hSop.Summary = "二冷1#加热器画面停止";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);
            /*------------------------------------*/

            // 二冷路2号加热器。
            hSop = dev.CreateStatusOutputChannelIn("EL_2#HEATER_HMI_START");
            hSop.Prompt = "EL_2#HEATER_HMI_START";
            hSop.Summary = "二冷2#加热器画面启动";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);

            // stop
            hSop = dev.CreateStatusOutputChannelIn("EL_2#HEATER_HMI_STOP");
            hSop.Prompt = "EL_2#HEATER_HMI_STOP";
            hSop.Summary = "二冷1#加热器画面停止";

            hexe = new SimplePulseExecuter() { DesignMark = hSop.Prompt };
            hSop.Controller = hexe;
            hexe.ExecuteChanged += HS_HeaterHMI_ExecuteChanged;
            _executerMap.Add(hSop.Label, hexe);

            _disChannels.Add(hSop.Label, hSop);

            HeaterHMISetGroup.AddSubChannel(hSop);
            /*------------------------------------*/
        }

        /// <summary>
        /// 初始化实验段风机变频器设备
        /// </summary>
        private void InitialFanDevice(LabDevice dev)
        {
            FeedbackChannel ch = dev.CreateFeedbackChannelIn("FirstColdFan");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 30000);
            HS_FirstColdFanDevice he = new HS_FirstColdFanDevice("FirstColdFan");
            ch.Controller = he;
            _executerMap.Add(ch.Label, he);
        }

    }
}
