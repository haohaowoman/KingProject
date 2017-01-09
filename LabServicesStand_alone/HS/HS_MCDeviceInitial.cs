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
            sc.Prompt = "I 0.0";
            sc.Summary = "风机控制电源全阐";

            sc = dev.CreateStatusChannelIn("风机远程控制");
            sc.Prompt = "I 0.1";
            sc.Summary = "风机远程控制";

            sc = dev.CreateStatusChannelIn("风机变频器准备就绪");
            sc.Prompt = "I 0.2";
            sc.Summary = "风机变频器准备就绪";

            sc = dev.CreateStatusChannelIn("风机变频器运行");
            sc.Prompt = "I 0.3";
            sc.Summary = "风机变频器运行";

            sc = dev.CreateStatusChannelIn("风机变频器停止");
            sc.Prompt = "I 0.4";
            sc.Summary = "风机变频器停止";

            sc = dev.CreateStatusChannelIn("风机变频器故障");
            sc.Prompt = "I 0.5";
            sc.Summary = "风机变频器故障";

            sc = dev.CreateStatusChannelIn("总电源合阐信号");
            sc.Prompt = "I 0.6";
            sc.Summary = "总电源合阐信号";

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

            sc = dev.CreateStatusChannelIn("风机电机前轴承温度报警");
            sc.Prompt = "I 2.0";
            sc.Summary = "风机电机前轴承温度报警";
            
            sc = dev.CreateStatusChannelIn("风机电机后轴承温度报警");
            sc.Prompt = "I 2.1";
            sc.Summary = "风机电机后轴承温度报警";

            sc = dev.CreateStatusChannelIn("风机电机绕组U温度报警");
            sc.Prompt = "I 2.2";
            sc.Summary = "风机电机绕组U温度报警";

            sc = dev.CreateStatusChannelIn("风机电机绕组V温度报警");
            sc.Prompt = "I 2.3";
            sc.Summary = "风机电机绕组V温度报警";

            sc = dev.CreateStatusChannelIn("风机电机绕组W温度报警");
            sc.Prompt = "I 2.4";
            sc.Summary = "风机电机绕组W温度报警";

            sc = dev.CreateStatusChannelIn("风机电机前轴振动报警");
            sc.Prompt = "I 2.5";
            sc.Summary = "风机电机前轴振动报警";

            sc = dev.CreateStatusChannelIn("风机电机后轴振动报警");
            sc.Prompt = "I 2.6";
            sc.Summary = "风机电机后轴振动报警";

            #region 风机停机

            sc = dev.CreateStatusChannelIn("风机电机前轴承温度超温停机");
            sc.Prompt = "I 2.7";
            sc.Summary = "风机电机前轴承温度超温停机";

            sc = dev.CreateStatusChannelIn("风机电机后轴承温度超温停机");
            sc.Prompt = "I 3.0";
            sc.Summary = "风机电机后轴承温度超温停机";

            sc = dev.CreateStatusChannelIn("风机电机绕组U温度超温停机");
            sc.Prompt = "I 3.1";
            sc.Summary = "风机电机绕组U温度超温停机";

            sc = dev.CreateStatusChannelIn("风机电机绕组V温度超温停机");
            sc.Prompt = "I 3.2";
            sc.Summary = "风机电机绕组V温度超温停机";

            sc = dev.CreateStatusChannelIn("风机电机绕组W温度超温停机");
            sc.Prompt = "I 3.3";
            sc.Summary = "风机电机绕组W温度超温停机";

            sc = dev.CreateStatusChannelIn("风机电机前轴振动停机");
            sc.Prompt = "I 3.4";
            sc.Summary = "风机电机前轴振动停机";

            sc = dev.CreateStatusChannelIn("风机电机后轴振动停机");
            sc.Prompt = "I 3.5";
            sc.Summary = "风机电机后轴振动停机";
            #endregion

            #endregion

            #region plc DO 可控制状态

            // 电炉入口 DN50 电动开关阀， 为防止电炉干烧 紧急异常情况下需要打开此
            StatusOutputChannel sch = dev.CreateStatusOutputChannelIn("EV0101");
            sch.Prompt = "Q 0.1";

            Executer exr = new DigitalExecuter() { DesignMark = "EV0101" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 电炉出口应急状态、关机时使用 去消音坑 电动开关阀 常闭
            sch = dev.CreateStatusOutputChannelIn("EV0102");
            sch.Prompt = "Q 0.2";

            exr = new DigitalExecuter() { DesignMark = "EV0102" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);


            // 电炉入口 DN50 电动开关阀， 为防止电炉干烧 紧急异常情况下需要打开此
            sch = dev.CreateStatusOutputChannelIn("EV0103");
            sch.Prompt = "Q 0.3";

            exr = new DigitalExecuter() { DesignMark = "EV0103" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 电炉出口应急状态、关机时使用 去消音坑 电动开关阀 常闭
            sch = dev.CreateStatusOutputChannelIn("EV0104");
            sch.Prompt = "Q 0.4";

            exr = new DigitalExecuter() { DesignMark = "EV0104" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 风机启动（短脉冲）
            sch = dev.CreateStatusOutputChannelIn("风机启动");
            sch.Prompt = "Q 0.5";

            exr = new DigitalExecuter() { DesignMark = "风机启动" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 风机停止（短脉冲）
            sch = dev.CreateStatusOutputChannelIn("风机停止");
            sch.Prompt = "Q 0.6";

            exr = new DigitalExecuter() { DesignMark = "风机停止" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            #region 操作台

            // 风机报警
            sch = dev.CreateStatusOutputChannelIn("风机报警");
            sch.Prompt = "Q 0.7";

            exr = new DigitalExecuter() { DesignMark = "风机报警" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 加热器报警
            sch = dev.CreateStatusOutputChannelIn("加热器报警");
            sch.Prompt = "Q 0.8";

            exr = new DigitalExecuter() { DesignMark = "加热器报警" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            // 实验室温度报警
            sch = dev.CreateStatusOutputChannelIn("实验室温度报警");
            sch.Prompt = "Q 1.0";

            exr = new DigitalExecuter() { DesignMark = "实验室温度报警" };
            sch.Controller = exr;
            _executerMap.Add(sch.Label, exr);

            #endregion

            #endregion
        }

        /// <summary>
        /// 初始化所有采集通道
        /// </summary>
        private void InitialMeasureChannels(LabDevice dev)
        {
            AnalogueMeasureChannel mCh;

            #region 热边采集卡通道

            // 热边入口流量
            mCh = dev.CreateAIChannelIn("FT0102");
            mCh.Unit = "Kg/h";
            mCh.Prompt = "01_Ch1";
            mCh.Summary = "热边入口流量";

            // 热边电炉入口空气压力            
            mCh = dev.CreateAIChannelIn("PT0104");
            mCh.Unit = "KPa";
            mCh.Prompt = "01_Ch2";
            mCh.Summary = "热边电炉入口空气压力";

            // 热边电炉入口空气温度            
            mCh = dev.CreateAIChannelIn("TT0102");
            mCh.Unit = "℃";
            mCh.Prompt = "01_Ch3";
            mCh.Summary = "热边电炉入口空气温度";

            // 热边电炉出口空气压力            
            mCh = dev.CreateAIChannelIn("PT0106");
            mCh.Unit = "KPa";
            mCh.Prompt = "01_Ch4";
            mCh.Summary = "热边电炉出口空气压力";

            // 热边电炉出口温度
            mCh = dev.CreateAIChannelIn("TT0104");
            mCh.Unit = "℃";
            mCh.Prompt = "01_Ch5";
            mCh.Summary = "热边电炉出口空气温度";

            // 热边实验段入口空气压力            
            mCh = dev.CreateAIChannelIn("PT0108");
            mCh.Unit = "KPa";
            mCh.Prompt = "01_Ch6";
            mCh.Summary = "热边实验段入口空气压力";

            // 热边实验段入口空气温度
            mCh = dev.CreateAIChannelIn("TT0106");
            mCh.Unit = "℃";
            mCh.Prompt = "02_Ch1";
            mCh.Summary = "热边实验段入口空气温度";

            // 热边实验段出口空气压力
            mCh = dev.CreateAIChannelIn("PT0109");
            mCh.Unit = "KPa";
            mCh.Prompt = "02_Ch2";
            mCh.Summary = "热边实验段出口空气压力";

            // 热边实验段出口空气温度
            mCh = dev.CreateAIChannelIn("TT0107");
            mCh.Unit = "℃";
            mCh.Prompt = "02_Ch3";
            mCh.Summary = "热边实验段出口空气温度";

            #endregion

            #region 二冷采集通道

            // 二冷入口空气流量
            mCh = dev.CreateAIChannelIn("FT0101");
            mCh.Unit = "Kg/h";
            mCh.Prompt = "02_Ch4";
            mCh.Summary = "二冷入口空气流量";

            // 二冷电炉入口空气压力
            mCh = dev.CreateAIChannelIn("PT0103");
            mCh.Unit = "KPa";
            mCh.Prompt = "02_Ch5";
            mCh.Summary = "二冷电炉入口空气压力";

            // 二冷电炉入口空气温度
            mCh = dev.CreateAIChannelIn("TT0101");
            mCh.Unit = "℃";
            mCh.Prompt = "02_Ch6";
            mCh.Summary = "二冷电炉入口空气温度";

            // 二冷电炉出口空气压力
            mCh = dev.CreateAIChannelIn("PT0105");
            mCh.Unit = "KPa";
            mCh.Prompt = "03_Ch1";
            mCh.Summary = "二冷电炉出口空气压力";

            // 二冷电炉出口空气温度
            mCh = dev.CreateAIChannelIn("TT0103");
            mCh.Unit = "℃";
            mCh.Prompt = "03_Ch2";
            mCh.Summary = "二冷电炉出口空气温度";

            // 二冷实验段入口空气压力
            mCh = dev.CreateAIChannelIn("PT0107");
            mCh.Unit = "KPa";
            mCh.Prompt = "03_Ch3";
            mCh.Summary = "二冷实验段入口空气压力";

            // 二冷实验段入口空气温度
            mCh = dev.CreateAIChannelIn("TT0105");
            mCh.Unit = "℃";
            mCh.Prompt = "03_Ch4";
            mCh.Summary = "二冷实验段入口空气温度";

            // 二冷实验段出口空气压力
            mCh = dev.CreateAIChannelIn("PT0110");
            mCh.Unit = "KPa";
            mCh.Prompt = "03_Ch5";
            mCh.Summary = "二冷实验段出口空气压力";

            // 二冷实验段出口空气温度
            mCh = dev.CreateAIChannelIn("TT0108");
            mCh.Unit = "℃";
            mCh.Prompt = "03_Ch6";
            mCh.Summary = "二冷实验段出口空气温度";

            #endregion

            #region 一冷实验段采集通道

            // 一冷入口空气流量
            mCh = dev.CreateAIChannelIn("FT0103");
            mCh.Unit = "Kg/h";
            mCh.Prompt = "04_Ch1";
            mCh.Summary = "一冷入口空气流量";

            // 一冷入口空气压力
            mCh = dev.CreateAIChannelIn("PT01");
            mCh.Unit = "KPa";
            mCh.Prompt = "04_Ch2";
            mCh.Summary = "一冷入口空气压力";

            // 一冷入口空气温度
            mCh = dev.CreateAIChannelIn("TT01");
            mCh.Unit = "℃";
            mCh.Prompt = "04_Ch3";
            mCh.Summary = "一冷入口空气温度";

            // 一冷入口空气压力
            mCh = dev.CreateAIChannelIn("PT02");
            mCh.Unit = "KPa";
            mCh.Prompt = "04_Ch4";
            mCh.Summary = "一冷入口空气压力";

            // 一冷入口空气温度
            mCh = dev.CreateAIChannelIn("TT02");
            mCh.Unit = "℃";
            mCh.Prompt = "04_Ch5";
            mCh.Summary = "一冷入口空气温度";

            // 一冷出口空气压力
            mCh = dev.CreateAIChannelIn("PT03");
            mCh.Unit = "KPa";
            mCh.Prompt = "04_Ch6";
            mCh.Summary = "一冷出口空气压力";

            // 一冷出口空气温度
            mCh = dev.CreateAIChannelIn("TT03");
            mCh.Unit = "℃";
            mCh.Prompt = "05_Ch1";
            mCh.Summary = "一冷出口空气温度";

            #endregion
        }

        /// <summary>
        /// 初始化PLC控制反馈通道
        /// </summary>
        private void InitialPLCChannels(LabDevice dev)
        {
            FeedbackChannel ch;
            Executer exr;
            #region 热边PLC电磁阀 电动调节阀 AO IO通道

            /// 热边PLC电磁阀 电动调节阀 AO通道
            /// 
            // 过滤器出口 DN80 常阀状态 可用于系统放气 去消音坑
            ch = dev.CreateFeedbackChannelIn("PV0104");
            ch.Unit = "%";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀PVO1O4开度";

            exr = new HS_EOVPIDExecuter("PV0104", 0);
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            // 电炉入口 DN65 需要保证系统 加热器的基本流量 流量粗调作用 保证50%常开状态
            ch = dev.CreateFeedbackChannelIn("FT0102A");
            ch.Unit = "%";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀FT0102A开度";

            exr = new HS_EOVPIDExecuter("FT0102A", 50) { PipeDiameter = 65 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            // 电炉入口 DN40 流量细调作用 
            ch = dev.CreateFeedbackChannelIn("FT0102B");
            ch.Unit = "%";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀FT0102B开度";

            exr = new HS_EOVPIDExecuter("FT0102B", 0) { PipeDiameter = 40 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            // 热边入实验段入口 DN80 电动调节阀 此处可处于常开状态
            ch = dev.CreateFeedbackChannelIn("PV0108");
            ch.Unit = "%";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀PV0108开度";

            exr = new HS_EOVPIDExecuter("PV0108", 90) { PipeDiameter = 80 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            // 热边 出实验段出口 DN80 电动调节阀 此处可处于常开状态 配合使用可 以达到流阻调节 共同调节压力与流量的作用
            ch = dev.CreateFeedbackChannelIn("PV0109");
            ch.Unit = "%";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀PV0109开度";

            exr = new HS_EOVPIDExecuter("PV0109", 90) { PipeDiameter = 80 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            #endregion

            #region 二冷 PLC控制电磁阀 AO IO通道

            // 过滤器出口 DN80  常阀状态 可用于系统放气 去消音坑
            ch = dev.CreateFeedbackChannelIn("PV0103");
            ch.Unit = "%";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀PV0103开度";

            exr = new HS_EOVPIDExecuter("PV0103", 0);
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            // 电炉入口 DN150 需要保证系统 加热器的基本流量 流量粗调作用 保证50%常开状态
            ch = dev.CreateFeedbackChannelIn("FT0101A");
            ch.Unit = "%";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀FT0101A开度";

            exr = new HS_EOVPIDExecuter("FT0101A", 50) { PipeDiameter = 150 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            // 电炉入口 DN50 流量细调作用
            ch = dev.CreateFeedbackChannelIn("FT0101B");
            ch.Unit = "%";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀FT0101B开度";

            exr = new HS_EOVPIDExecuter("FT0101B", 0) { PipeDiameter = 50 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            // 二冷入实验段入口 DN80 电动调节阀 此处可处于常开状态
            ch = dev.CreateFeedbackChannelIn("PV0107");
            ch.Unit = "%";
            ch.Range = new QRange(0, 100);
            ch.Summary = "电动调节阀PV0107开度";

            exr = new HS_EOVPIDExecuter("PV0107", 80) { PipeDiameter = 80 };
            ch.Controller = exr;
            _executerMap.Add(ch.Label, exr);

            #endregion

            #region 一冷 实验段 PLC AO IO通道
            AnalogueMeasureChannel amc;
            // 风机变频器转速反馈 控制
            amc = dev.CreateAIChannelIn("风机变频器电机转速反馈");
            amc.Unit = "r/s";
            amc.Range = new QRange(0, 5000);
            amc.Summary = "风机变频器电机转速反馈";

            // 风机变频器电流反馈
            amc = dev.CreateAIChannelIn("风机变频器电机电流反馈");
            amc.Unit = "A";
            amc.Range = new QRange(0, 10);
            amc.Summary = "风机变频器电机电流反馈";

            #region 风机仪表箱

            // 风机前轴承温度TT1
            amc = dev.CreateAIChannelIn("风机电机前轴承温度TT1");
            amc.Unit = "℃";
            amc.Prompt = "TT1";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机前轴承温度TT1";

            amc = dev.CreateAIChannelIn("风机电机后轴承温度TT2");
            amc.Unit = "℃";
            amc.Prompt = "TT2";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机后轴承温度TT2";

            amc = dev.CreateAIChannelIn("风机电机绕组U温度TT3");
            amc.Unit = "℃";
            amc.Prompt = "TT3";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机绕组U温度TT3";

            amc = dev.CreateAIChannelIn("风机电机绕组V温度TT4");
            amc.Unit = "℃";
            amc.Prompt = "TT4";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机绕组V温度TT4";

            amc = dev.CreateAIChannelIn("风机电机绕组W温度TT5");
            amc.Unit = "℃";
            amc.Prompt = "TT5";
            amc.Range = new QRange(0, 300);
            amc.Summary = "风机电机绕组W温度TT5";

            amc = dev.CreateAIChannelIn("风机电机前轴振动");
            amc.Prompt = "ZD-1";
            amc.Summary = "风机电机前轴振动";

            amc = dev.CreateAIChannelIn("风机电机后轴振动");
            amc.Prompt = "ZD-2";
            amc.Summary = "风机电机后轴振动";

            #endregion

            #endregion

            #region 工作室温度

            amc = dev.CreateAIChannelIn("工作室温度TT6-1");
            amc.Unit = "℃";
            amc.Prompt = "TT6-1";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-1";

            amc = dev.CreateAIChannelIn("工作室温度TT6-2");
            amc.Unit = "℃";
            amc.Prompt = "TT6-2";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-2";

            amc = dev.CreateAIChannelIn("工作室温度TT6-3");
            amc.Unit = "℃";
            amc.Prompt = "TT6-3";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-3";

            amc = dev.CreateAIChannelIn("工作室温度TT6-4");
            amc.Unit = "℃";
            amc.Prompt = "TT6-4";
            amc.Range = new QRange(0, 100);
            amc.Summary = "工作室温度TT6-4";

            #endregion
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

            HS_ElectricHeaterExecuter hotRoadExe = new HS_HotRoadHeaterExe("HotRoad1#Heater", this) { RequireMinInFlow = Defualt_MinQ_HotRoad };
            hotRoadExe.ExecutePredicate = HotRoadHeaterExecuterPredicate;

            ch.Controller = hotRoadExe;
            _executerMap.Add(ch.Label, hotRoadExe);

            ch = dev.CreateFeedbackChannelIn("HotRoad2#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "热路2#加热器";
            ch.Prompt = "HR2#H";

            hotRoadExe = new HS_HotRoadHeaterExe("HotRoad2#Heater", this) { RequireMinInFlow = Defualt_MinQ_HotRoad };
            hotRoadExe.ExecutePredicate = HotRoadHeaterExecuterPredicate;

            ch.Controller = hotRoadExe;
            _executerMap.Add(ch.Label, hotRoadExe);

            ch = dev.CreateFeedbackChannelIn("HotRoad3#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "热路3#加热器";
            ch.Prompt = "HR3#H";

            hotRoadExe = new HS_HotRoadHeaterExe("HotRoad3#Heater", this) { RequireMinInFlow = Defualt_MinQ_HotRoad };
            hotRoadExe.ExecutePredicate = HotRoadHeaterExecuterPredicate;

            ch.Controller = hotRoadExe;
            _executerMap.Add(ch.Label, hotRoadExe);

            ch = dev.CreateFeedbackChannelIn("HotRoad4#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "热路4#加热器";
            ch.Prompt = "HR4#H";

            hotRoadExe = new HS_HotRoadHeaterExe("HotRoad4#Heater", this) { RequireMinInFlow = Defualt_MinQ_HotRoad };
            hotRoadExe.ExecutePredicate = HotRoadHeaterExecuterPredicate;

            ch.Controller = hotRoadExe;
            _executerMap.Add(ch.Label, hotRoadExe);

            ch = dev.CreateFeedbackChannelIn("HotRoad5#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "热路5#加热器";
            ch.Prompt = "HR5#H";

            hotRoadExe = new HS_HotRoadHeaterExe("HotRoad5#Heater", this) { RequireMinInFlow = Defualt_MinQ_HotRoad };
            hotRoadExe.ExecutePredicate = HotRoadHeaterExecuterPredicate;

            ch.Controller = hotRoadExe;
            _executerMap.Add(ch.Label, hotRoadExe);

            // 二冷边加热器 通道
            ch = dev.CreateFeedbackChannelIn("SecendCold1#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "二冷路1#号加热器";
            ch.Prompt = "SC1#H";

            HS_ElectricHeaterExecuter secHExe = new HS_SecendHeaterExe("SecendCold1#Heater", this) { RequireMinInFlow = Defualt_MinQ_SecendCold };
            secHExe.ExecutePredicate = SecendColdHeaterExecuterPredicate;
            ch.Controller = secHExe;
            _executerMap.Add(ch.Label, secHExe);

            ch = dev.CreateFeedbackChannelIn("SecendCold2#Heater");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            ch.Summary = "二冷路2#号加热器";
            ch.Prompt = "SC2#H";

            secHExe = new HS_SecendHeaterExe("SecendCold2#Heater", this) { RequireMinInFlow = Defualt_MinQ_SecendCold };
            secHExe.ExecutePredicate = SecendColdHeaterExecuterPredicate;
            ch.Controller = secHExe;
            _executerMap.Add(ch.Label, secHExe);
        }

        /// <summary>
        /// 初始化实验段风机变频器设备
        /// </summary>
        private void InitialFanDevice(LabDevice dev)
        {
            FeedbackChannel ch = dev.CreateFeedbackChannelIn("FirstColdFan");
            ch.Unit = "℃";
            ch.Range = new QRange(0, 800);
            HS_FirstColdFanDevice he = new HS_FirstColdFanDevice("FirstColdFan");
            ch.Controller = he;
            _executerMap.Add(ch.Label, he);
        }

    }
}
