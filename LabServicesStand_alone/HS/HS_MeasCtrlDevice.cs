using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabDeviceModule;
using LabMCESystem.LabElement;
using LabMCESystem.Logic.Execute;
using System.Timers;
using LabMCESystem.Task;
using LabMCESystem.BaseService.ExperimentDataExchange;

namespace LabMCESystem.Servers.HS
{
    /// <summary>
    /// 环散系统的试验设备类
    /// 在此实现对所有传感器的数据采集，使用采集卡，PLC控制，电炉控制等基本逻辑
    /// 根据设计图纸确定PLC各个控制器的名称 以映射控制方法
    /// 定义规则：如果控制通道拥有自身反馈通道，并需要单独当作设备通道使用，则需要在其对应的输出通道名称后加上 “#In”
    /// 如：电动调节阀通道： PV0104 其反馈如需要单独作为通道使用并上传数据 则其通道名称应为 PV0104#In, PV0104更改为PV0104#Out
    /// 如果不应用此操作，则控制通道更新上传的数据应以反馈数据优先 如果没有返馈 才使用控制数据作为上传数据标准
    /// </summary>
    public class HS_MeasCtrlDevice : ControlExecuterBase
    {
        public HS_MeasCtrlDevice()
        {
            _updateTimer.Elapsed += _updateTimer_Elapsed;

            InitialDevice();
        }

        // 默认设置的热边最小流量
        public const double Defualt_MinQ_HotRoad = 100;
        // 默认设置的二冷最小流量
        public const double Defualt_MinQ_SecendCold = 100;

        
        #region Fields
        // 通过通道名创建执行器的映射关系
        private Dictionary<string, Executer> _executerMap = new Dictionary<string, Executer>();
        /// <summary>
        /// 获取通道对应执行器的映射。
        /// </summary>
        public Dictionary<string, Executer> ExecuterMap
        {
            get { return _executerMap; }
        }

        // 表示 通道提示-采集值 的集合
        private Dictionary<string, double> _measureValues = new Dictionary<string, double>();

        public Dictionary<string, double> MeasureValues
        {
            get { return _measureValues; }
        }

        // 数据更新定时器
        private Timer _updateTimer = new Timer(100);

        #endregion

        #region Properties

        /// <summary>
        /// 获取通道数据读取器。
        /// </summary>
        public ISingleExpRTDataRead ValueReader
        {
            get
            {
                return SingleReader;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// 初始化本机设备
        /// 在此进行创建 登录 和 注册
        /// </summary>
        private void InitialDevice()
        {
            LabDevice dev = new LabDevice("PLC&加热器控制", 01);

            InitialMeasureChannels(dev);

            InitialPLCChannels(dev);

            InitialDianluChannels(dev);

            InitialFanDevice(dev);

            // 单独为FT0101 与 FT0102创建绑定执行器。

            // 二冷
            LabChannel ch = dev["FT0101"];

            System.Diagnostics.Debug.Assert(ch != null);

            HS_FIRQExecuter FIRQE = new HS_FIRQExecuter(ch.Label, this, Defualt_MinQ_SecendCold);
            FIRQE.MulEOVExecuter = new HS_MultipelEOVExecuter
                (
                (HS_EOVPIDExecuter)_executerMap["FT0101A"],
                (HS_EOVPIDExecuter)_executerMap["FT0101B"],
                "FT0101#A&B"
                );

            _executerMap.Add(ch.Label, FIRQE);

            // 热边
            ch = dev["FT0102"];

            System.Diagnostics.Debug.Assert(ch != null);

            FIRQE = new HS_FIRQExecuter(ch.Label, this, Defualt_MinQ_HotRoad);
            FIRQE.MulEOVExecuter = new HS_MultipelEOVExecuter
                (
                (HS_EOVPIDExecuter)_executerMap["FT0102A"],
                (HS_EOVPIDExecuter)_executerMap["FT0102B"],
                "FT0102#A&B"
                );

            _executerMap.Add(ch.Label, FIRQE);

            // 赋值给设备。
            Device = dev;

            // 为执行器添加反馈更新事件与执行事件。
            foreach (var exe in _executerMap)
            {
                // 电磁调节阀 执行器                
                if (exe.Value is HS_EOVPIDExecuter)
                {
                    HS_EOVPIDExecuter tempe = exe.Value as HS_EOVPIDExecuter;

                    tempe.UpdateFedback += HS_EOVsUpdateFedback;

                    tempe.ExecuteChanged += HS_EOVsExecuteChanged;
                }

                // 电磁开关阀执行器
                if (exe.Value is DigitalExecuter)
                {
                    exe.Value.ExecuteChanged += HS_DigitalEOV_ExecuteChanged;
                }
            }

            // 为采集通道映射数据
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 6; j++)
                {
                    _measureValues.Add($"{i:D2}_Ch{j:D2}", 0);
                }
            }
        }

        #region 电磁开关阀执行器事件
        // 电磁开关阀控制。
        private void HS_DigitalEOV_ExecuteChanged(object sender, double executedVal)
        {

        }

        #endregion

        #region 电磁调节阀执行器 事件

        /// <summary>
        /// 电磁阀反馈更新事件函数。
        /// </summary>
        /// <param name="sender">电磁执行器</param>
        private void HS_EOVsUpdateFedback(IDataFedback sender)
        {

        }

        /// <summary>
        /// 电磁阀执行输出事件函数。
        /// </summary>
        /// <param name="sender">电磁阀执行器。</param>
        /// <param name="executedVal">需要执行的值。</param>
        private void HS_EOVsExecuteChanged(object sender, double executedVal)
        {

        }

        #endregion

        /// <summary>
        /// 初始化所有采集通道
        /// </summary>
        private void InitialMeasureChannels(LabDevice dev)
        {
            #region 热边采集卡通道

            // 热边入口流量
            LabChannel mCh = new LabChannel("FT0102", ExperimentWorkStyle.Measure) { Unit = "Kg/h" };
            dev.AddElement(mCh);
            mCh.Prompt = "01_Ch1";

            // 热边电炉入口压力
            mCh = new LabChannel("PT0104", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "01_Ch2";
            // 热边电炉入口温度
            mCh = new LabChannel("TT0102", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "01_Ch3";
            // 热边电炉出口压力
            mCh = new LabChannel("PT0106", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "01_Ch4";
            // 热边电炉出口温度
            mCh = new LabChannel("TT0104", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "01_Ch5";
            // 热边实验段入口压力
            mCh = new LabChannel("PT0108", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "01_Ch6";
            // 热边实验段入口温度
            mCh = new LabChannel("TT0106", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "02_Ch1";
            // 热边实验段出口压力
            mCh = new LabChannel("PT0109", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "02_Ch2";
            // 热边实验段出口温度
            mCh = new LabChannel("TT0107", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "02_Ch3";
            #endregion

            #region 二冷采集通道

            // 二冷入口流量
            mCh = new LabChannel("FT0101", ExperimentWorkStyle.Measure) { Unit = "Kg/h" };
            dev.AddElement(mCh);
            mCh.Prompt = "02_Ch4";

            // 二冷电炉入口压力
            mCh = new LabChannel("PT0103", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "02_Ch5";
            // 二冷电炉入口温度
            mCh = new LabChannel("TT0101", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "02_Ch6";
            // 二冷电炉出口压力
            mCh = new LabChannel("PT0105", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "03_Ch1";
            // 二冷电炉出口温度
            mCh = new LabChannel("TT0103", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "03_Ch2";
            // 二冷实验段入口压力
            mCh = new LabChannel("PT0107", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "03_Ch3";
            // 二冷实验段入口温度
            mCh = new LabChannel("TT0105", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "03_Ch4";
            // 二冷实验段出口压力
            mCh = new LabChannel("PT0110", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "03_Ch5";
            // 二冷实验段出口温度
            mCh = new LabChannel("TT0108", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "03_Ch6";
            #endregion

            #region 一冷实验段采集通道

            // 一冷入口流量
            mCh = new LabChannel("FT0103", ExperimentWorkStyle.Measure) { Unit = "Kg/h" };
            dev.AddElement(mCh);
            mCh.Prompt = "04_Ch1";
            // 一冷入口压力
            mCh = new LabChannel("PT01", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "04_Ch2";
            // 一冷入口温度
            mCh = new LabChannel("TT01", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "04_Ch3";
            // 一冷入口压力
            mCh = new LabChannel("PT02", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "04_Ch4";
            // 一冷入口温度
            mCh = new LabChannel("TT02", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "04_Ch5";
            // 一冷出口压力
            mCh = new LabChannel("PT03", ExperimentWorkStyle.Measure) { Unit = "KPa" };
            dev.AddElement(mCh);
            mCh.Prompt = "04_Ch6";
            // 一冷出口温度
            mCh = new LabChannel("TT03", ExperimentWorkStyle.Measure) { Unit = "℃" };
            dev.AddElement(mCh);
            mCh.Prompt = "05_Ch1";
            #endregion
        }

        /// <summary>
        /// 初始化所有PLC通道
        /// </summary>
        private void InitialPLCChannels(LabDevice dev)
        {
            #region 热边PLC电磁阀 电动调节阀 AO IO通道

            /// 热边PLC电磁阀 电动调节阀 AO通道
            /// 
            // 过滤器出口 DN80 常阀状态 可用于系统放气 去消音坑
            LabChannel ch = new LabChannel("PV0104", ExperimentWorkStyle.Control) { Unit = "%" };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new HS_EOVPIDExecuter("PV0104", 0));

            // 电炉入口 DN65 需要保证系统 加热器的基本流量 流量粗调作用 保证50%常开状态
            ch = new LabChannel("FT0102A", ExperimentWorkStyle.Control) { Unit = "%" };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new HS_EOVPIDExecuter("FT0102A", 50) { PipeDiameter = 65 });

            // 电炉入口 DN40 流量细调作用 
            ch = new LabChannel("FT0102B", ExperimentWorkStyle.Control) { Unit = "%" };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new HS_EOVPIDExecuter("FT0102B", 0) { PipeDiameter = 40 });

            // 电炉入口 DN50 电动开关阀， 为防止电炉干烧 紧急异常情况下需要打开此
            ch = new LabChannel("EV0103", ExperimentWorkStyle.Control) { ConnectSignalType = SignalType.Digital };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new DigitalExecuter() { DesignMark = "EV0103" });

            // 电炉出口应急状态、关机时使用 去消音坑 电动开关阀 常闭
            ch = new LabChannel("EV0104", ExperimentWorkStyle.Control) { ConnectSignalType = SignalType.Digital };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new DigitalExecuter() { DesignMark = "EV0104" });

            // 热边入实验段入口 DN80 电动调节阀 此处可处于常开状态
            ch = new LabChannel("PV0108", ExperimentWorkStyle.Control) { Unit = "%" };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new HS_EOVPIDExecuter("PV0108", 90) { PipeDiameter = 80 });

            // 热边 出实验段出口 DN80 电动调节阀 此处可处于常开状态 配合使用可 以达到流阻调节 共同调节压力与流量的作用
            ch = new LabChannel("PV0109", ExperimentWorkStyle.Control) { Unit = "%" };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new HS_EOVPIDExecuter("PV0109", 90) { PipeDiameter = 80 });

            #endregion

            #region 二冷 PLC控制电磁阀 AO IO通道

            // 过滤器出口 DN80  常阀状态 可用于系统放气 去消音坑
            ch = new LabChannel("PV0103", ExperimentWorkStyle.Control) { Unit = "%" };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new HS_EOVPIDExecuter("PV0103", 0));

            // 电炉入口 DN150 需要保证系统 加热器的基本流量 流量粗调作用 保证50%常开状态
            ch = new LabChannel("FT0101A", ExperimentWorkStyle.Control) { Unit = "%" };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new HS_EOVPIDExecuter("FT0101A", 50) { PipeDiameter = 150 });

            // 电炉入口 DN50 流量细调作用
            ch = new LabChannel("FT0101B", ExperimentWorkStyle.Control) { Unit = "%" };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new HS_EOVPIDExecuter("FT0101B", 0) { PipeDiameter = 50 });

            // 电炉入口 DN50 电动开关阀， 为防止电炉干烧 紧急异常情况下需要打开此
            ch = new LabChannel("EV0101", ExperimentWorkStyle.Control) { ConnectSignalType = SignalType.Digital };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new DigitalExecuter() { DesignMark = "EV0101" });

            // 电炉出口应急状态、关机时使用 去消音坑 电动开关阀 常闭
            ch = new LabChannel("EV0102", ExperimentWorkStyle.Control) { ConnectSignalType = SignalType.Digital };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new DigitalExecuter() { DesignMark = "EV0102" });

            // 二冷入实验段入口 DN80 电动调节阀 此处可处于常开状态
            ch = new LabChannel("PV0107", ExperimentWorkStyle.Control) { Unit = "%" };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new HS_EOVPIDExecuter("PV0107", 80) { PipeDiameter = 80 });

            #endregion

            #region 一冷 实验段 PLC AO IO通道

            #endregion

        }

        /// <summary>
        /// 初始化所有电炉所需要的通道
        /// </summary>
        private void InitialDianluChannels(LabDevice dev)
        {
            // 加热器图纸为给出标记 所以自定义标记

            // 热边电加热器 通道 
            LabChannel ch = new LabChannel("HotRoadHeater", ExperimentWorkStyle.Control) { Unit = "℃" };
            dev.AddElement(ch);

            HS_ElectricHeaterExecuter hotRoadExe = new HS_HotRoadHeaterExe("HotRoadHeater", this) { RequireMinInFlow = Defualt_MinQ_HotRoad };
            hotRoadExe.ExecutePredicate = HotRoadHeaterExecuterPredicate;

            _executerMap.Add(ch.Label, hotRoadExe);

            // 二冷边加热器 通道
            ch = new LabChannel("SecendColdHeater", ExperimentWorkStyle.Control) { Unit = "℃" };
            dev.AddElement(ch);

            HS_ElectricHeaterExecuter secHExe = new HS_SecendHeaterExe("SecendColdHeater", this) { RequireMinInFlow = Defualt_MinQ_SecendCold };
            secHExe.ExecutePredicate = SecendColdHeaterExecuterPredicate;

            _executerMap.Add(ch.Label, hotRoadExe);
        }

        /// <summary>
        /// 初始化实验段风机变频器设备
        /// </summary>
        private void InitialFanDevice(LabDevice dev)
        {
            LabChannel ch = new LabChannel("FirstColdFan", ExperimentWorkStyle.Control) { Unit = "r/s" };
            dev.AddElement(ch);

            _executerMap.Add(ch.Label, new HS_FirstColdFanDevice("FirstColdFan"));
        }

        /// <summary>
        /// 热边电加热器执行条件
        /// </summary>
        /// <param name="sender">执行器</param>
        /// <param name="eVal">执行值</param>
        /// <returns>是否满足</returns>
        private bool HotRoadHeaterExecuterPredicate(object sender, ref double eVal)
        {
            // 在此获取电炉入口流量的反馈 FT0102 FIRQ 0102

            // 如果流量不满足，则不执行本次控制，并首先执行增加入口流量执行机制

            // 如果连续出现流量不足状态则进行保护措施 关闭电炉 打开EV0103 与EV0104进电炉进行冲刷 强制执行故障异常保护

            return true;
        }

        /// <summary>
        /// 二冷电加热器执行条件
        /// </summary>
        /// <param name="sender">执行器</param>
        /// <param name="eVal">执行值</param>
        /// <returns>是否满足</returns>
        private bool SecendColdHeaterExecuterPredicate(object sender, ref double eVal)
        {
            // 在此获取电炉入口流量的反馈 FT0101 FIRQ 0101

            // 如果流量不满足，则不执行本次控制，并首先执行增加入口流量执行机制

            // 如果连续出现流量不足状态则进行保护措施 关闭电炉 打开EV0103 与EV0104进电炉进行冲刷 强制执行故障异常保护


            return true;
        }

        // 定时器进行数据的读取和更新。
        private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 从采集设备读取所有通道数据
            //...
            // 数据模拟
            Random r = new Random();
            double[] mvalues = new double[48];

            for (int i = 0; i < 48; i++)
            {
                mvalues[i] = r.NextDouble() * 100.0;
            }

            int index = 0;
            foreach (var mv in _measureValues)
            {
                _measureValues[mv.Key] = mvalues[index++];
            }

            // 获取
        }

        #endregion

        #region Base class Override

        protected override void OnClosed()
        {

        }

        protected override bool OnClosing()
        {
            return true;
        }

        protected override void OnLogin()
        {

        }

        protected override void OnRegisted()
        {

        }

        protected override bool OnRun()
        {
            return true;
        }

        protected override void OnRunning()
        {

        }

        protected override void OnStopped()
        {

        }

        protected override bool OnStopping()
        {
            return true;
        }

        /// <summary>
        /// 重载，在此实现各个通道的控制任务逻辑。
        /// </summary>
        /// <param name="chSetter">通道设定项</param>
        protected override void OnControlTaskReceived(ChannelSetter chSetter)
        {
            if (chSetter != null)
            {
                // 通道属于本设备

                switch (chSetter.Channel.WorkStyle)
                {
                    case ExperimentWorkStyle.Measure:
                        {
                            // 在此实现各个测量通道的PID逻辑控制。

                        }
                        break;
                    case ExperimentWorkStyle.Control:
                        {
                            // 在此实现单个控制通道的逻辑控制。

                            // 通过通道标签来找到对应的执行器
                            Executer ext;
                            bool bh = _executerMap.TryGetValue(chSetter.Channel.Label, out ext);
                            if (bh)
                            {
                                // 如果通道对应有执行器则通过执行器执行控制
                                ext.TargetVal = chSetter.Setter.TargetValue;
                                ext.ExecuteBegin();
                            }
                        }
                        break;
                    case ExperimentWorkStyle.Both:
                        {

                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
