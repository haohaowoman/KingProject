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
    /// 组合通道转换器，用以将已有通道的值转换为其它通道的测量值。
    /// </summary>
    class MultipelChannelConverter
    {
        public List<Channel> Channels { get; private set; } = new List<Channel>();

        public Func<MultipelChannelConverter, double> Algorithm { get; set; }
        /// <summary>
        /// 执行转换算法得到转换值。
        /// </summary>
        /// <returns>转换值。</returns>
        public double Converte()
        {
            return Algorithm?.Invoke(this) ?? 0;
        }
    }

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

        public const string PLCGetHMIGroupName = "HMIGetGroup";

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

        public OPCGroup PLCHMIGetGroup { get; set; } = new OPCGroup(PLCGetHMIGroupName);

        //零散设备通道 这此通道只进行了创建 并未添加进行设备组。
        Dictionary<string, Channel> _disChannels = new Dictionary<string, Channel>();
        /// <summary>
        /// PLC 写入HMI get通道。
        /// </summary>
        List<FeedbackChannel> _plcGetChls = new List<FeedbackChannel>();
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
            sc.Summary = "风机控制电源合阐";

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

            SwitchEOVHMISetGroup.AddSubChannel(eovControl);
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
            sch.Summary = "二冷路流量调节旁通开关阀";

            Executer exr = new DigitalExecuter() { DesignMark = "EV0101" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 电炉出口应急状态、关机时使用 去消音坑 电动开关阀 常闭
            sch = dev.CreateStatusOutputChannelIn("EV0102");
            sch.Prompt = "EV0102";
            sch.Summary = "二冷路加热器后端排空开关阀";

            exr = new DigitalExecuter() { DesignMark = "EV0102" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 电炉入口 DN50 电动开关阀， 为防止电炉干烧 紧急异常情况下需要打开此
            sch = dev.CreateStatusOutputChannelIn("EV0103");
            sch.Prompt = "EV0103";
            sch.Summary = "热路流量调节旁通开关阀";

            exr = new DigitalExecuter() { DesignMark = "EV0103" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 电炉出口应急状态、关机时使用 去消音坑 电动开关阀 常闭
            sch = dev.CreateStatusOutputChannelIn("EV0104");
            sch.Prompt = "EV0104";
            sch.Summary = "热路加热器后端排空开关阀";

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

            sch.Execute += (sender, e) =>
            {
                exr.ExecuteBegin();
            };

            exr.ExecuteChanged += (sender, e) =>
            {
                var pulseExe = sender as SimplePulseExecuter;
                if (pulseExe != null)
                {
                    FanHMISetGroup.Write(pulseExe.DesignMark,
                        pulseExe.NextPulseBit == PulseBit.HighBit ? true : false);
#if DEBUG
                    Console.WriteLine($"Pulse executer {pulseExe} execute {pulseExe.NextPulseBit}. ");
#endif
                }
            };

            FanHMISetGroup.AddSubChannel(sch);
            /*---------------------------*/

            // 风机停止（短脉冲）
            sch = dev.CreateStatusOutputChannelIn("FJ_HMI_STOP");
            sch.Prompt = "FJ_HMI_STOP";
            sch.Summary = "风机停止";

            exr = new SimplePulseExecuter() { DesignMark = "FJ_HMI_STOP" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            sch.Execute += (sender, e) =>
            {
                exr.ExecuteBegin();
            };

            exr.ExecuteChanged += (sender, e) =>
            {
                var pulseExe = sender as SimplePulseExecuter;
                if (pulseExe != null)
                {
                    FanHMISetGroup.Write(pulseExe.DesignMark,
                        pulseExe.NextPulseBit == PulseBit.HighBit ? true : false);
#if DEBUG
                    Console.WriteLine($"Pulse executer {pulseExe} execute {pulseExe.NextPulseBit}. ");
#endif
                }
            };

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

            // 试验运行状态通道。
            sch = dev.CreateStatusOutputChannelIn("ExpRunState");
            sch.Summary = "试验运行状态";
            sch.Prompt = "HMI.SYS_RUN_GET";
            exr = new DigitalExecuter() { DesignMark = sch.Prompt };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            DOHMISetGroup.AddSubChannel(sch);

            // 运行状态控制事件。
            sch.Execute += (sender, e) =>
            {
                var resetCh = sender as StatusOutputChannel;
                if (resetCh != null)
                {
                    var de = resetCh.Controller as DigitalExecuter;
                    if (resetCh.NextStatus)
                    {
                        de.ToHigh();
                    }
                    else
                    {
                        de.ToLow();
                    }
                    de.ExecuteBegin();
                }
            };

            exr.ExecuteChanged += (sender, e) =>
            {
                // 执行复位脉冲。
                var pulseExe = sender as DigitalExecuter;
                if (pulseExe != null)
                {
                    DOHMISetGroup.Write(pulseExe.DesignMark,
                        pulseExe.Enable);
#if DEBUG
                    Console.WriteLine($"Pulse executer {pulseExe} execute {pulseExe.Enable}. ");
#endif
                }
            };

            // 试验运行状态通道。
            sch = dev.CreateStatusOutputChannelIn("SysAlaram");
            sch.Summary = "试验运行故障";
            sch.Prompt = "HMI.SYS_ALARM_GET";
            exr = new SimplePulseExecuter(1000) { DesignMark = sch.Prompt };
            
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            DOHMISetGroup.AddSubChannel(sch);

            // 系统故障事件。
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
                // 脉冲。
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
        /// 初始化所有采集通道,必须第一个进行初始化。
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
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
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
            mCh.Range = new QRange(0, 750);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 750));
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
            rlTemp.Range = new QRange(0, 750);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 750));
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
            mCh.Range = new QRange(0, 750);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 750));
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
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
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
            elTemp.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
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
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
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
            firtFlow.Range = new QRange(0, 33000);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 33000));
            sensor.Unit = "Kg/h";
            sensor.Label = "FT0103";
            sensor.SensorNumber = "FT0103 000001";

            firtFlow.Collector = sensor;

            // 一冷入口空气压力
            mCh = dev.CreateAIChannelIn("PT01");
            mCh.Unit = "KPa";
            mCh.Prompt = "05_Ch1";
            mCh.Summary = "一冷入口空气压力";
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
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
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
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
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
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

            // 一冷出口空气压力
            mCh = dev.CreateAIChannelIn("PT04");
            mCh.Unit = "KPa";
            mCh.Prompt = "05_Ch4";
            mCh.Summary = "一冷出口空气压力";
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
            sensor.Unit = "KPa";
            sensor.Label = "PT04";
            sensor.SensorNumber = "PT04 000001";

            mCh.Collector = sensor;
            // 一冷出口空气温度
            mCh = dev.CreateAIChannelIn("TT04");
            mCh.Unit = "℃";
            mCh.Prompt = "04_Ch2";
            mCh.Summary = "一冷出口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT04";
            sensor.SensorNumber = "TT04 000001";

            mCh.Collector = sensor;

            // 一冷出口空气压力
            mCh = dev.CreateAIChannelIn("PT05");
            mCh.Unit = "KPa";
            mCh.Prompt = "05_Ch5";
            mCh.Summary = "一冷出口空气压力";
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
            sensor.Unit = "KPa";
            sensor.Label = "PT05";
            sensor.SensorNumber = "PT05 000001";

            mCh.Collector = sensor;
            // 一冷出口空气温度
            mCh = dev.CreateAIChannelIn("TT05");
            mCh.Unit = "℃";
            mCh.Prompt = "04_Ch3";
            mCh.Summary = "一冷出口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT05";
            sensor.SensorNumber = "TT05 000001";

            mCh.Collector = sensor;

            // 一冷出口空气压力
            mCh = dev.CreateAIChannelIn("PT06");
            mCh.Unit = "KPa";
            mCh.Prompt = "05_Ch6";
            mCh.Summary = "一冷出口空气压力";
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
            sensor.Unit = "KPa";
            sensor.Label = "PT06";
            sensor.SensorNumber = "PT06 000001";

            mCh.Collector = sensor;
            // 一冷出口空气温度
            mCh = dev.CreateAIChannelIn("TT06");
            mCh.Unit = "℃";
            mCh.Prompt = "04_Ch4";
            mCh.Summary = "一冷出口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT06";
            sensor.SensorNumber = "TT06 000001";

            mCh.Collector = sensor;

            // 一冷出口空气压力
            mCh = dev.CreateAIChannelIn("PT07");
            mCh.Unit = "KPa";
            mCh.Prompt = "06_Ch1";
            mCh.Summary = "一冷出口空气压力";
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
            sensor.Unit = "KPa";
            sensor.Label = "PT07";
            sensor.SensorNumber = "PT07 000001";

            mCh.Collector = sensor;
            // 一冷出口空气温度
            mCh = dev.CreateAIChannelIn("TT07");
            mCh.Unit = "℃";
            mCh.Prompt = "04_Ch5";
            mCh.Summary = "一冷出口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT07";
            sensor.SensorNumber = "TT03 000001";

            mCh.Collector = sensor;

            // 一冷出口空气压力
            mCh = dev.CreateAIChannelIn("PT08");
            mCh.Unit = "KPa";
            mCh.Prompt = "06_Ch2";
            mCh.Summary = "一冷出口空气压力";
            mCh.Range = new QRange(0, 150);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 150));
            sensor.Unit = "KPa";
            sensor.Label = "PT08";
            sensor.SensorNumber = "PT03 000001";

            mCh.Collector = sensor;
            // 一冷出口空气温度
            mCh = dev.CreateAIChannelIn("TT08");
            mCh.Unit = "℃";
            mCh.Prompt = "04_Ch6";
            mCh.Summary = "一冷出口空气温度";
            mCh.Range = new QRange(0, 100);

            sensor = new LinerSensor(new QRange(4, 20), new QRange(0, 100));
            sensor.Unit = "℃";
            sensor.Label = "TT08";
            sensor.SensorNumber = "TT08 000001";

            mCh.Collector = sensor;
            #endregion

            // 从数据及更新已有采集通道。从文件读取。
            string eleSetPath = Environment.CurrentDirectory + @"\ElementsSet.xml";
            try
            {
                ChannelsDataset.ReadXml(eleSetPath);
                if (ChannelsDataset.Channels.ChannelsCount < dev.Channels.Count)
                {
                    throw new ArgumentOutOfRangeException("数据集储存通道数小于已初始化话通道数，无效。");
                }
                ChannelsDataset.Channels.UpdateToChannels(dev.Channels);
                // 创建用户自定义通道。
                int hChCount = dev.Channels.Count;
                int esChCount = ChannelsDataset.Channels.Count;
                if (hChCount < esChCount)
                {
                    for (int i = hChCount; i < esChCount; i++)
                    {
                        var nch = ChannelsDataset.Channels.CreateAMeasureChannel(dev, ChannelsDataset.Channels.Rows[hChCount][0] as string);

                        if (nch != null)
                        {
                            var chs = nch.Collector as LinerSensor;
                            if (chs != null)
                            {
                                nch.Unit = chs.Unit;
                                nch.Range = chs.Range;
                            }
                            CustomChannels.Add(nch);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ChannelsDataset.Clear();
                ChannelsDataset.Channels.AddChannels(dev.Channels);
                SaveElements();
            }


            // 使用已有的通道创建虚拟的 压差 与 散热率通道。
            mCh = dev.CreateAIChannelIn("RL_PRESSUREDIFF");
            mCh.Prompt = "RL_PRE_DIFF";
            mCh.Summary = "热路压差";
            mCh.Unit = "KPa";

            var cov = new MultipelChannelConverter();
            cov.Channels.Add(dev["PT0108"]);
            cov.Channels.Add(dev["PT0109"]);
            cov.Algorithm = (converter) =>
            {
                var pt0108 = converter.Channels[0] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(pt0108 != null);
                var pt0109 = converter.Channels[1] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(pt0109 != null);
                return pt0108.MeasureValue - pt0109.MeasureValue;
            };

            mCh.Collector = cov;

            mCh = dev.CreateAIChannelIn("RL_HEAT");
            mCh.Prompt = "RL_HEAT";
            mCh.Summary = "热路散热量";
            mCh.Unit = "KW";

            cov = new MultipelChannelConverter();
            cov.Channels.Add(dev["TT0106"]);
            cov.Channels.Add(dev["TT0107"]);
            cov.Channels.Add(dev["FT0102"]);
            cov.Algorithm = (converter) =>
            {
                var TIn = converter.Channels[0] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(TIn != null);
                var TOut = converter.Channels[1] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(TOut != null);
                var flow = converter.Channels[2] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(flow != null);

                return 1.000 * flow.MeasureValue * Math.Abs(TIn.MeasureValue - TOut.MeasureValue) / 3600.0;
            };

            mCh.Collector = cov;

            mCh = dev.CreateAIChannelIn("EL_PRESSUREDIFF");
            mCh.Prompt = "EL_PRE_DIFF";
            mCh.Summary = "二冷压差";
            mCh.Unit = "KPa";

            cov = new MultipelChannelConverter();
            cov.Channels.Add(dev["PT0107"]);
            cov.Channels.Add(dev["PT0110"]);
            cov.Algorithm = (converter) =>
            {
                var pt0107 = converter.Channels[0] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(pt0107 != null);
                var pt0110 = converter.Channels[1] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(pt0110 != null);
                return pt0107.MeasureValue - pt0110.MeasureValue;
            };

            mCh.Collector = cov;

            mCh = dev.CreateAIChannelIn("EL_HEAT");
            mCh.Prompt = "EL_HEAT";
            mCh.Summary = "二冷吸热量";
            mCh.Unit = "J";

            cov = new MultipelChannelConverter();
            cov.Channels.Add(dev["TT0105"]);
            cov.Channels.Add(dev["TT0108"]);
            cov.Channels.Add(dev["FT0101"]);
            cov.Algorithm = (converter) =>
            {
                var TIn = converter.Channels[0] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(TIn != null);
                var TOut = converter.Channels[1] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(TOut != null);
                var flow = converter.Channels[2] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(flow != null);

                return 1.000 * flow.MeasureValue * Math.Abs(TIn.MeasureValue - TOut.MeasureValue) / 3600.0;
            };

            mCh.Collector = cov;

            mCh = dev.CreateAIChannelIn("HEAT_EMISS_EFFIC");
            mCh.Prompt = "HEAT_EMISS_EFFIC";
            mCh.Summary = "散热效率";
            mCh.Unit = "%";

            cov = new MultipelChannelConverter();
            cov.Channels.Add(dev["TT0106"]);
            cov.Channels.Add(dev["TT0107"]);
            cov.Channels.Add(dev["TT0105"]);
            cov.Channels.Add(dev["TT01"]);

            cov.Algorithm = (converter) =>
            {
                double n = 0;
                var rlTIn = converter.Channels[0] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(rlTIn != null);
                var rlTOut = converter.Channels[1] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(rlTOut != null);
                var elTIn = converter.Channels[2] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(elTIn != null);
                var ylTIn = converter.Channels[3] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(ylTIn != null);
                n = (rlTIn.MeasureValue - (elTIn.MeasureValue + ylTIn.MeasureValue) / 2.0);
                if (n != 0)
                {
                    n = (rlTIn.MeasureValue - rlTOut.MeasureValue) * 100.0 / n;
                }
                else
                {
                    n = -99999;
                }
                return n;
            };

            mCh.Collector = cov;

            mCh = dev.CreateAIChannelIn("YL_HEAT");
            mCh.Prompt = "YL_HEAT";
            mCh.Summary = "一冷吸热量";
            mCh.Unit = "J";

            cov = new MultipelChannelConverter();
            cov.Channels.Add(dev["TT01"]);
            cov.Channels.Add(dev["TT05"]);
            cov.Channels.Add(dev["FT0103"]);
            cov.Algorithm = (converter) =>
            {
                var TIn = converter.Channels[0] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(TIn != null);
                var TOut = converter.Channels[1] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(TOut != null);
                var flow = converter.Channels[2] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(flow != null);

                return 1000 * flow.MeasureValue * Math.Abs(TIn.MeasureValue - TOut.MeasureValue) / 3600.0;
            };

            mCh.Collector = cov;


            mCh = dev.CreateAIChannelIn("YL_PRESSUREDIFF");
            mCh.Prompt = "YL_PRE_DIFF";
            mCh.Summary = "一冷压差";
            mCh.Unit = "KPa";

            cov = new MultipelChannelConverter();
            cov.Channels.Add(dev["PT01"]);
            cov.Channels.Add(dev["PT05"]);
            cov.Algorithm = (converter) =>
            {
                var pt01 = converter.Channels[0] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(pt01 != null);
                var pt05 = converter.Channels[1] as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(pt05 != null);
                return pt01.MeasureValue - pt05.MeasureValue;
            };

            mCh.Collector = cov;

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
            ch.Summary = "热路加热器前端泄压调节阀";

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
            ch.Summary = "热路流量粗调调节阀";

            exr = new HS_EOVPIDExecuter("HMI.FV0102A_SET_HMI", 50, ch) { PipeDiameter = /*65*/44 };
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
            ch.Summary = "热路流量细调调节阀";

            exr = new HS_EOVPIDExecuter("HMI.FV0102B_SET_HMI", 0, ch) { PipeDiameter = /*40*/17 };
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
            ch.Summary = "热路产品入口调节阀";

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
            ch.Summary = "热路产品出口调节阀";

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
            ch.Summary = "二冷路加热器前端泄压调节阀";

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
            ch.Summary = "二冷路流量粗调调节阀";

            exr = new HS_EOVPIDExecuter("HMI.FV0101A_SET_HMI", 50, ch) { PipeDiameter = /*150*/99 };
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
            ch.Summary = "二冷路流量细调调节阀";

            exr = new HS_EOVPIDExecuter("HMI.FV0101B_SET_HMI", 0, ch) { PipeDiameter = /*50*/24 };
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
            ch.Summary = "二冷路产品入口调节阀";

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
            amc = dev.CreateAIChannelIn("MOTOSPEED_HMI");
            amc.Unit = "r/s";
            amc.Prompt = "HMI.MOTOSPEED_HMI";
            amc.Range = new QRange(0, 5000);
            amc.Summary = "风机变频器电机转速反馈";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            // 风机变频器电流反馈
            amc = dev.CreateAIChannelIn("MOTOCURRENT_HMI");
            amc.Unit = "A";
            amc.Range = new QRange(0, 10);
            amc.Summary = "风机变频器电机电流反馈";
            amc.Prompt = "HMI.MOTOCURRENT_HMI";

            //FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            #region 风机仪表箱

            // 风机前轴承温度TT1
            amc = dev.CreateAIChannelIn("FJ_QZC_TT_HMI");
            amc.Unit = "℃";
            amc.Prompt = "HMI.FJ_QZC_TT_HMI";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机前轴承温度TT1";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("FJ_HZC_TT_HMI");
            amc.Unit = "℃";
            amc.Prompt = "HMI.FJ_HZC_TT_HMI";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机后轴承温度TT2";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("FJ_WINDING_U_TT_HMI");
            amc.Unit = "℃";
            amc.Prompt = "HMI.FJ_WINDING_U_TT_HMI";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机绕组U温度TT3";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("FJ_WINDING_V_TT_HMI");
            amc.Unit = "℃";
            amc.Prompt = "HMI.FJ_WINDING_V_TT_HMI";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机绕组V温度TT4";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("FJ_WINDING_W_TT_HMI");
            amc.Unit = "℃";
            amc.Prompt = "HMI.FJ_WINDING_W_TT_HMI";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机绕组W温度TT5";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("FJ_Q_ZD_HMI");
            amc.Prompt = "HMI.FJ_Q_ZD_HMI";
            amc.Summary = "风机电机前轴振动";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("FJ_H_ZD_HMI");
            amc.Prompt = "HMI.FJ_H_ZD_HMI";
            amc.Summary = "风机电机后轴振动";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            var fpsetch = dev.CreateFeedbackChannelIn("FJ_PL_SET_HMI");
            fpsetch.Unit = "Hz";
            fpsetch.Range = new QRange(0, 70);
            fpsetch.Prompt = "HMI.FJ_PL_SET_HMI";
            fpsetch.Summary = "风机变频器频率画面设定";

            var fanFSetExe = new NoLogicExecuter(new mcLogic.SafeRange(0, 70));
            fpsetch.Controller = fanFSetExe;
            _executerMap.Add(fpsetch.Label, fanFSetExe);

            fpsetch.Execute += (sender, e) =>
            {
                fanFSetExe.TargetVal = fpsetch.AOValue;
                fanFSetExe.ExecuteBegin();
            };
            fpsetch.StopExecute += (sender, e) =>
            {
                fanFSetExe.TargetVal = 0;
                fanFSetExe.ExecuteBegin();
                fanFSetExe.ExecuteOver();
            };

            fanFSetExe.ExecuteChanged += (sender, e) =>
            {
                FanHMISetGroup.Write(fpsetch.Prompt, e);
            };

            FanHMISetGroup.AddSubChannel(fpsetch);
            FanHMIGroup.AddSubChannel(fpsetch);
            /*---------------------------*/

            #endregion

            #endregion

            #region 工作室温度

            amc = dev.CreateAIChannelIn("LAB_TT6-1_HMI");
            amc.Unit = "℃";
            amc.Prompt = "HMI.LAB_TT6-1_HMI";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-1";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("LAB_TT6-2_HMI");
            amc.Unit = "℃";
            amc.Prompt = "HMI.LAB_TT6-2_HMI";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-2";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("LAB_TT6-3_HMI");
            amc.Unit = "℃";
            amc.Prompt = "HMI.LAB_TT6-3_HMI";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-3";

            FanHMIGroup.AddSubChannel(ch);
            /*---------------------------*/

            amc = dev.CreateAIChannelIn("LAB_TT6-4_HMI");
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
                suc &= PLCHMIGetGroup.InitialGroup();
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
                PLCHMIGetGroup.CloseGroup();

                HS_PLCReadWriter.CloseOpcInteract();
            }

        }

        /// <summary>
        /// 初始化所有电炉所需要的通道。
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
        /// 初始化实验段风机变频器设备。
        /// </summary>
        private void InitialFanDevice(LabDevice dev)
        {
            FeedbackChannel ch = dev.CreateFeedbackChannelIn("FirstColdFan");
            ch.Unit = "rpm";
            ch.Prompt = "FanCtrl";
            ch.Summary = "风机控制";

            ch.Range = new QRange(0, 2950);

            HS_FirstColdFanDevice he = new HS_FirstColdFanDevice(ch.Label);

            he.SafeRange = new mcLogic.SafeRange(ch.Range.Low, ch.Range.Height);
            he.FanFrequencePlcChannel = dev["FJ_PL_SET_HMI"] as FeedbackChannel;
            he.FanStartChannel = dev["FJ_HMI_START"] as StatusOutputChannel;
            he.FanStopChannel = dev["FJ_HMI_STOP"] as StatusOutputChannel;
            he.FanReadyChannel = dev["风机变频器准备就绪"] as StatusChannel;
            he.FanConnectionChannel = dev["FanRemoteConnection"] as StatusChannel;
            he.FanIChannel = dev["MOTOCURRENT_HMI"] as IAnalogueMeasure;
            he.FanDeviceChannel = dev["FirstColdFan"] as FeedbackChannel;
            he.FanFreqErrorChannel = dev["风机变频器故障"] as StatusChannel;
            he.FanBeRunChannel = dev["风机变频器运行"] as StatusChannel;
            he.FanBeStopChannel = dev["风机变频器停止"] as StatusChannel;
            he.FanRemoteLocalChannel = dev["风机远程控制"] as StatusChannel;
            ch.Controller = he;
            _executerMap.Add(ch.Label, he);

            ch.Execute += (sender, e) =>
            {
                he.TargetVal = ch.AOValue;
                he.ExecuteBegin();
            };

            ch.StopExecute += (sender, e) =>
            {
                he.ExecuteOver();
                he.Reset();
            };

            var fanFaultReset = dev.CreateAOChannelIn("FanFaultRest");
            fanFaultReset.Prompt = "风机故障复位";
            fanFaultReset.Execute += (sender, e) =>
            {
                FanDeviceImp.Fan.ResetError();
            };
        }

        /// <summary>
        /// 初始化需要向PLC写入的模拟量通道。
        /// </summary>
        private void InitialPLCGetChannels()
        {
            // 需要创建不加入设备的输出通道 并与采集通道匹配，将采集通道的采集值当作新建通道的输出值。

            var getChl = LabDevice.CreateChannel(Device, "TT0101_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT0101_GET";
            getChl.Summary = "二冷温度 TT0101 上位机写入";
            getChl.Collector = Device["TT0101"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT0102_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT0102_GET";
            getChl.Summary = "二冷温度 TT0102 上位机写入";
            getChl.Collector = Device["TT0102"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT0103_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT0103_GET";
            getChl.Summary = "二冷温度 TT0103 上位机写入";
            getChl.Collector = Device["TT0103"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT0105_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT0105_GET";
            getChl.Summary = "二冷温度 TT0105 上位机写入";
            getChl.Collector = Device["TT0105"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT0108_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT0108_GET";
            getChl.Summary = "二冷温度 TT0108 上位机写入";
            getChl.Collector = Device["TT0108"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT0103_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT0103_GET";
            getChl.Summary = "二冷 压 力 PT0103 上位机写入";
            getChl.Collector = Device["PT0103"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT0105_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT0105_GET";
            getChl.Summary = "二冷 压 力 PT0105 上位机写入";
            getChl.Collector = Device["PT0105"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT0107_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT0107_GET";
            getChl.Summary = "二冷 压 力 PT0107 上位机写入";
            getChl.Collector = Device["PT0107"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT0110_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT0110_GET";
            getChl.Summary = "二冷 压 力 PT0110 上位机写入";
            getChl.Collector = Device["PT0110"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT0104_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT0104_GET";
            getChl.Summary = "热 路温度 TT0104 上位机写入";
            getChl.Collector = Device["TT0104"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT0106_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT0106_GET";
            getChl.Summary = "热 路温度 TT0106 上位机写入";
            getChl.Collector = Device["TT0106"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT0107_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT0107_GET";
            getChl.Summary = "热 路温度 TT0107 上位机写入";
            getChl.Collector = Device["TT0107"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT0104_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT0104_GET";
            getChl.Summary = "热 路 压 力 PT0104 上位机写入";
            getChl.Collector = Device["PT0104"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT0106_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT0106_GET";
            getChl.Summary = "热 路 压 力 PT0106 上位机写入";
            getChl.Collector = Device["PT0106"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT0108_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT0108_GET";
            getChl.Summary = "热 路 压 力 PT0108 上位机写入";
            getChl.Collector = Device["PT0108"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT0109_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT0109_GET";
            getChl.Summary = "热 路 压 力 PT0109 上位机写入";
            getChl.Collector = Device["PT0109"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT01_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT01_GET";
            getChl.Summary = "试验 段温度 TT01 上位机写入";
            getChl.Collector = Device["TT01"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT02_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT02_GET";
            getChl.Summary = "试验 段温度 TT02 上位机写入";
            getChl.Collector = Device["TT02"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT03_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT03_GET";
            getChl.Summary = "试验 段温度 TT03 上位机写入";
            getChl.Collector = Device["TT03"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT04_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT04_GET";
            getChl.Summary = "试验 段温度 TT04 上位机写入";
            getChl.Collector = Device["TT04"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT05_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT05_GET";
            getChl.Summary = "试验 段温度 TT05 上位机写入";
            getChl.Collector = Device["TT05"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT06_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT06_GET";
            getChl.Summary = "试验 段温度 TT06 上位机写入";
            getChl.Collector = Device["TT06"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT07_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT07_GET";
            getChl.Summary = "试验 段温度 TT07 上位机写入";
            getChl.Collector = Device["TT07"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "TT08_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.TT08_GET";
            getChl.Summary = "试验 段温度 TT08 上位机写入";
            getChl.Collector = Device["TT08"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT01_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT01_GET";
            getChl.Summary = "试验 段 压 力 PT01 上位机写入";
            getChl.Collector = Device["PT01"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT02_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT02_GET";
            getChl.Summary = "试验 段 压 力 PT02 上位机写入";
            getChl.Collector = Device["PT02"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT03_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT03_GET";
            getChl.Summary = "试验 段 压 力 PT03 上位机写入";
            getChl.Collector = Device["PT03"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT04_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT04_GET";
            getChl.Summary = "试验 段 压 力 PT04 上位机写入";
            getChl.Collector = Device["PT04"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT05_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT05_GET";
            getChl.Summary = "试验 段 压 力 PT05 上位机写入";
            getChl.Collector = Device["PT05"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT06_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT06_GET";
            getChl.Summary = "试验 段 压 力 PT06 上位机写入";
            getChl.Collector = Device["PT06"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT07_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT07_GET";
            getChl.Summary = "试验 段 压 力 PT07 上位机写入";
            getChl.Collector = Device["PT07"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "PT08_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.PT08_GET";
            getChl.Summary = "试验 段 压 力 PT08 上位机写入";
            getChl.Collector = Device["PT08"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "FT0101_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.FT0101_GET";
            getChl.Summary = "低 压试验 管路流量 FT0101 上位机写 入";
            getChl.Collector = Device["FT0101"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "FT0102_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.FT0102_GET";
            getChl.Summary = "低 压试验 管路流量 FT0102 上位机写 入";
            getChl.Collector = Device["FT0102"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------

            getChl = LabDevice.CreateChannel(Device, "FT0103_GET", ExperimentStyle.Feedback) as FeedbackChannel;
            getChl.Prompt = "HMI.FT0103_GET";
            getChl.Summary = "低 压试验 管路流量 FT0103 上位机写 入";
            getChl.Collector = Device["FT0103"];

            _plcGetChls.Add(getChl);
            PLCHMIGetGroup.AddSubChannel(getChl);
            //-----------------------------------
        }
        /// <summary>
        /// 向PLC写入HMI get通道。
        /// </summary>
        private void WriteToPLCGetChannels()
        {
            foreach (var getCh in _plcGetChls)
            {
                var pCh = getCh.Collector as IAnalogueMeasure;
                System.Diagnostics.Debug.Assert(pCh != null);

                getCh.AOValue = pCh.MeasureValue;
            }

            PLCHMIGetGroup.Write();
        }
    }
}
