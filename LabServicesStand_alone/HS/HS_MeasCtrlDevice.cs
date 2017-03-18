using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.LabDeviceModule;
using LabMCESystem.LabElement;
using mcLogic.Execute;
using System.Timers;
using LabMCESystem.ETask;
using LabMCESystem.BaseService.ExperimentDataExchange;

using static LabMCESystem.Servers.HS.HS_PLCReadWriter;
using mcLogic.Demarcate;
using HS_DeviceInteract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LabMCESystem.Servers.HS.HS_Executers;
using System.Xml.Serialization;
using System.IO;
using mcLogic.Data;
using LabMCESystem.Servers.HS.ChannelSet;

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
    public partial class HS_MeasCtrlDevice : ControlExecuterBase
    {
        public HS_MeasCtrlDevice()
        {
            _updateTimer.Elapsed += _updateTimer_Elapsed;

            InitialDevice();
        }

        // 默认设置的热边最小流量
        public const double Defualt_MinQ_HotRoad = 500;
        // 默认设置的二冷最小流量
        public const double Defualt_MinQ_SecendCold = 500;

        /// <summary>
        /// 在电炉控制时的基础流量，在控制流量下对使用电炉数量计量的基本参数。
        /// </summary>
        static public double HeaterControlBaseFlow = 500;
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

        // 表示 数采箱采集 通道提示-采集值 的集合
        private Dictionary<string, double> _measureValues = new Dictionary<string, double>();

        public Dictionary<string, double> MeasureValues
        {
            get { return _measureValues; }
        }

        private string[] _mvDicKeys;

        // 根据通道名 与 数据值的集合
        private Dictionary<string, ChannelRTData> _chsValueLook;

        public Dictionary<string, ChannelRTData> ChsValueLook
        {
            get { return _chsValueLook; }
            set { _chsValueLook = value; }
        }

        /// <summary>
        /// 获取数采通道电流值。
        /// </summary>
        public double[] AnalogyValues { get; private set; }

        /// <summary>
        /// 保存数采箱通道标号与其对应的下标集合。
        /// </summary>
        private Dictionary<string, int> _devPromptIndexDic;
        // 采集通道数据更新定时器
        private Timer _updateTimer = new Timer(250);

        // 记录已更新数据的次数。
        private ulong _updatedCount = 0;
        /// <summary>
        /// 热路流量通道。
        /// </summary>
        private FeedbackChannel _rlFlowChannel;
        /// <summary>
        /// 二冷流量通道。
        /// </summary>
        private FeedbackChannel _elFlowChannel;
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

        /// <summary>
        /// 获取/设置实验室的报警温度上限。
        /// </summary>
        public double LabFaultTemprature { get; set; } = 50;

        /// <summary>
        /// 获取数采箱的交互。
        /// </summary>
        public HS_DeviceInteract.IADDeviceInteract ADDeviceInteract { get; private set; }

        public List<LinerDemarcater> ADBoxDemarcaters { get; set; }

        public List<SamplePointCollection> SamplePoints { get; private set; }

        /// <summary>
        /// 获取用于储存采集通道于传感器的数据集。
        /// </summary>
        public HS_Elements ChannelsDataset { get; private set; } = new HS_Elements();
        /// <summary>
        /// 获取设备中用户自定义添加的通道集合。
        /// </summary>
        public List<Channel> CustomChannels { get; private set; } = new List<Channel>();
        #endregion

        #region Operators

        /// <summary>
        /// 初始化本机设备
        /// 在此进行创建 登录 和 注册
        /// </summary>
        private void InitialDevice()
        {
            LabDevice dev = new LabDevice("PLC&加热器控制", 01);

            // 赋值给设备。
            Device = dev;

            InitialExceptionWatcher();

            InitialMeasureChannels(dev);

            InitialPLCChannels(dev);

            InitialStatusChannel(dev);

            InitialDianluChannels(dev);

            InitialFanDevice(dev);

            InitialPLCGetChannels();

            // 单独为FT0101 与 FT0102创建绑定执行器。

            // 一冷流量控制
            FeedbackChannel fFlow = dev["FT0103"] as FeedbackChannel;
            System.Diagnostics.Debug.Assert(fFlow != null);

            var fFlowExe = new FirstColdFIRQExecuter(0, new mcLogic.SafeRange(fFlow.Range.Low, fFlow.Range.Height));
            fFlowExe.DesignMark = fFlow.Label;
            fFlowExe.FIRQChannel = fFlow;
            fFlowExe.FanDevChannel = dev["FirstColdFan"] as FeedbackChannel;

            fFlow.Controller = fFlowExe;
            _executerMap.Add(fFlow.Label, fFlowExe);

            fFlow.Execute += (sender, e) =>
            {
                //fFlowExe.TargetVal = fFlow.AOValue;
                //fFlowExe.ExecuteBegin();
            };

            fFlow.StopExecute += (sender, e) =>
            {
                fFlowExe.ExecuteOver();
                fFlowExe.Reset();
            };

            // 二冷
            Channel ch = dev["FT0101"];

            System.Diagnostics.Debug.Assert(ch != null);

            HS_FIRQExecuter FIRQE = new HS_FIRQExecuter(ch.Label, ch as IAnalogueMeasure, Defualt_MinQ_SecendCold);
            FIRQE.MulEOVExecuter = new HS_MultipelEOVExecuter
                (
                (HS_EOVPIDExecuter)_executerMap["FT0101A"],
                (HS_EOVPIDExecuter)_executerMap["FT0101B"],
                "FT0101#A&B"
                );
            FIRQE.ProductInEovChannel = Device["PV0107"] as IFeedback;
            
            var elFlowCh = ch as FeedbackChannel;
            _elFlowChannel = elFlowCh;

            System.Diagnostics.Debug.Assert(elFlowCh != null);

            elFlowCh.Execute += Flow_Execute;
            elFlowCh.StopExecute += Flow_StopExecute;
            FIRQE.SafeRange = new mcLogic.SafeRange(elFlowCh.Range.Low, /*elFlowCh.Range.Height*/50000);
            elFlowCh.Controller = FIRQE;

            _executerMap.Add(ch.Label, FIRQE);


            // 热边
            ch = dev["FT0102"];

            System.Diagnostics.Debug.Assert(ch != null);

            FIRQE = new HS_FIRQExecuter(ch.Label, ch as IAnalogueMeasure, Defualt_MinQ_HotRoad);
            FIRQE.MulEOVExecuter = new HS_MultipelEOVExecuter
                (
                (HS_EOVPIDExecuter)_executerMap["FT0102A"],
                (HS_EOVPIDExecuter)_executerMap["FT0102B"],
                "FT0102#A&B"
                );
            // 重置热路调节PID参数。
            FIRQE.Param = new PIDParam() { Ts = 15000, Kp = 0.64, Ti = 80000, Td = 4000 };
            FIRQE.ProductInEovChannel = Device["PV0108"] as IFeedback;
            FIRQE.ProductOutEovChannel = Device["PV0109"] as IFeedback;
            var rlFlowCh = ch as FeedbackChannel;
            _rlFlowChannel = rlFlowCh;

            System.Diagnostics.Debug.Assert(elFlowCh != null);

            rlFlowCh.Execute += Flow_Execute;
            rlFlowCh.StopExecute += Flow_StopExecute;
            FIRQE.SafeRange = new mcLogic.SafeRange(rlFlowCh.Range.Low, /*rlFlowCh.Range.Height*/50000);
            rlFlowCh.Controller = FIRQE;

            _executerMap.Add(ch.Label, FIRQE);

            InitialEExceptions();

            // 为可控制通道关联控制处理事件。

            // 电动调节阀的控制事件。
            var eovcs = Device.FeedbackChannels;
            foreach (var ec in eovcs)
            {
                if (ec.Unit == "%")
                {
                    ec.Execute += EOVChnannel_Execute;
                    var eovex = ec.Controller as HS_EOVPIDExecuter;
                    if (eovex != null)
                    {
                        eovex.ExecuteChanged += HS_EOVsExecuteChanged;
                        eovex.UpdateFedback += HS_EOVsUpdateFedback;
                    }
                }

            }

            // 电动控制阀控制事件。
            var socs = Device.StatusOutputChannels;
            foreach (var so in socs)
            {
                so.Execute += StatusOutput_Execute;
                var ext = so.Controller as DigitalExecuter;
                if (ext != null)
                {
                    ext.ExecuteChanged += HS_SwitchEOV_ExecuteChanged;
                }
            }

            // 为采集通道映射数据
            int index = 0;
            _devPromptIndexDic = new Dictionary<string, int>(48);
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 6; j++)
                {
                    _measureValues.Add($"{i:D2}_Ch{j:D}", 0);
                    _devPromptIndexDic.Add($"{i:D2}_Ch{j:D}", index++);
                }
            }

            _mvDicKeys = _measureValues.Keys.ToArray();

            // 打开数采箱设备。
            ADDeviceInteract = new EM9118.BoardManage();
            if (ADDeviceInteract != null)
            {
                var adc = Device["ADDevRemoteConnection"] as IStatusExpress;
                if (adc != null)
                {
                    adc.Status = ADDeviceInteract.OpenDevice();
                }

                foreach (var item in ADDeviceInteract.CardsConnection)
                {
                    adc.Status &= item;
                }

                ADDeviceInteract.CardConnectionChanged += (sender, e) =>
                {
                    foreach (var item in ADDeviceInteract.CardsConnection)
                    {
                        adc.Status &= item;
                    }
                };
            }

            // opc group
            bool bsuc = InitialOpcInteractGroup();
            (Device["PLCRemoteConnection"] as IStatusExpress).Status = bsuc;
#if DEBUG
            if (!bsuc)
            {
                Console.WriteLine($"Initial opc interact feild.");
            }
#endif
            InitialHeaters();

            // 为二冷入口温度TT0105添加控制器，执行温度调节任务。
            var elTemp = dev["TT0105"] as FeedbackChannel;

            System.Diagnostics.Debug.Assert(elTemp != null);

            var elTempExecuter = new HS_TempreatureExecute();
            elTempExecuter.AllowTolerance = new Tolerance(1.5);
            elTempExecuter.DesignMark = "ELRoadTempController";
            elTempExecuter.FlowChannel = elFlowCh;
            elTempExecuter.TargetTempChannel = elTemp;
            elTempExecuter.SecTempChannel = dev["TT0103"] as AnalogueMeasureChannel;
            elTempExecuter.HeaterChannels.Add(dev["SecendCold1#Heater"] as FeedbackChannel);
            elTempExecuter.HeaterChannels.Add(dev["SecendCold2#Heater"] as FeedbackChannel);

            elTempExecuter.HeatersFromChannels();

            elTemp.Execute += (sender, e) =>
            {
                elTempExecuter.TargetVal = elTemp.AOValue;
                elTempExecuter.ExecuteBegin();
            };

            elTemp.StopExecute += (sender, e) =>
            {
                elTempExecuter.ExecuteOver();
                elTempExecuter.StopHeatersOutput();
                elTempExecuter.Reset();
            };
            _executerMap.Add(elTemp.Label, elTempExecuter);
            //-------------------------------
            // 热路温度控制器 TT0106控制。
            var rlTemp = dev["TT0106"] as FeedbackChannel;

            System.Diagnostics.Debug.Assert(rlTemp != null);

            var rlTempExecuter = new HS_TempreatureExecute();
            rlTempExecuter.AllowTolerance = new Tolerance(1.5);
            rlTempExecuter.DesignMark = "HotRoadTempController";
            rlTempExecuter.FlowChannel = rlFlowCh;
            rlTempExecuter.TargetTempChannel = rlTemp;
            rlTempExecuter.SecTempChannel = dev["TT0104"] as AnalogueMeasureChannel;
            rlTempExecuter.HeaterChannels.Add(dev["HotRoad1#Heater"] as FeedbackChannel);
            rlTempExecuter.HeaterChannels.Add(dev["HotRoad2#Heater"] as FeedbackChannel);
            rlTempExecuter.HeaterChannels.Add(dev["HotRoad3#Heater"] as FeedbackChannel);
            rlTempExecuter.HeaterChannels.Add(dev["HotRoad4#Heater"] as FeedbackChannel);
            rlTempExecuter.HeaterChannels.Add(dev["HotRoad5#Heater"] as FeedbackChannel);
            rlTempExecuter.HeatersFromChannels();

            _executerMap.Add(rlTemp.Label, rlTempExecuter);

            rlTemp.Execute += (sender, e) =>
            {
                rlTempExecuter.TargetVal = rlTemp.AOValue;
                rlTempExecuter.ExecuteBegin();
            };

            rlTemp.StopExecute += (sender, e) =>
            {
                rlTempExecuter.ExecuteOver();
                rlTempExecuter.StopHeatersOutput();
                rlTempExecuter.Reset();
            };
            //-------------------------------

            // 读取数采箱通道校准样点值文件xml。
            string path = Environment.CurrentDirectory;
            path += @"\SamplePoints.xml";
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<SamplePointCollection>));
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                {
                    SamplePoints = xs.Deserialize(fs) as List<SamplePointCollection>;
                    if (SamplePoints.Count != 48)
                    {
                        throw new ArgumentOutOfRangeException();
                    }

                }
            }
            catch (Exception ex)
            {
                InitialSamplePoints();
            }

            // 从样点创建所有通道的标定器。
            ADBoxDemarcaters = new List<LinerDemarcater>(48);
            UpdateADBoxDemarcaters();

            // 将产品入口阀门、出口阀门全开。
            var eov = Device["PV0107"] as IFeedback;
            System.Diagnostics.Debug.Assert(eov != null);
            eov.AOValue = 100;
            eov.ControllerExecute();

            eov = Device["PV0108"] as IFeedback;
            System.Diagnostics.Debug.Assert(eov != null);
            eov.AOValue = 100;
            eov.ControllerExecute();

            eov = Device["PV0109"] as IFeedback;
            System.Diagnostics.Debug.Assert(eov != null);
            eov.AOValue = 100;
            eov.ControllerExecute();
        }

        
        #region 电磁开关阀执行器事件
        // 电磁开关阀控制。
        private void HS_SwitchEOV_ExecuteChanged(object sender, double executedVal)
        {
            var de = sender as DigitalExecuter;
            if (de != null)
            {
                string realChannelLabel = null;
                // 得到对应的相关的真正执行离散通道。
                switch (de.DesignMark)
                {
                    case "EV0101":
                        {
                            realChannelLabel = de.Enable ? "EV0101_HMI_OPEN" : "EV0101_HMI_CLOSE";
                        }
                        break;
                    case "EV0102":
                        {
                            realChannelLabel = de.Enable ? "EV0102_HMI_OPEN" : "EV0102_HMI_CLOSE";
                        }
                        break;
                    case "EV0103":
                        {
                            realChannelLabel = de.Enable ? "EV0103_HMI_OPEN" : "EV0103_HMI_CLOSE";
                        }
                        break;
                    case "EV0104":
                        {
                            realChannelLabel = de.Enable ? "EV0104_HMI_OPEN" : "EV0104_HMI_CLOSE";
                        }
                        break;
                }
                if (realChannelLabel != null)
                {
                    Channel realChannel;

                    if (_disChannels.TryGetValue(realChannelLabel, out realChannel))
                    {
                        var pulseExe = (realChannel as StatusOutputChannel).Controller as SimplePulseExecuter;

                        pulseExe?.ExecuteBegin();
                    }
                }
            }
        }

        // 电磁开关阀控制通道事件。
        private void StatusOutput_Execute(object sender, ControllerEventArgs e)
        {
            var ev = sender as StatusOutputChannel;
            if (ev != null)
            {
                var de = ev.Controller as DigitalExecuter;
                if (de != null)
                {
                    if (ev.NextStatus)
                    {
                        de.ToHigh();
                    }
                    else
                    {
                        de.ToLow();
                    }
                }
            }
        }

        // 电磁开关阀脉冲执行器控制事件。
        private void EovExe_ExecuteChanged(object sender, double executedVal)
        {
            var pulseExe = sender as SimplePulseExecuter;
            if (pulseExe != null)
            {
                SwitchEOVHMISetGroup.Write(pulseExe.DesignMark,
                    pulseExe.NextPulseBit == PulseBit.HighBit ? true : false);
#if DEBUG
                Console.WriteLine($"Pulse executer {pulseExe} execute {pulseExe.NextPulseBit}. ");
#endif
            }
        }

        private void EovExe_ExecuteOvered(object obj)
        {

#if DEBUG
            Console.WriteLine($"Pulse executer {obj} execute overed. ");
#endif
        }

        #endregion

        #region 电磁调节阀执行器 事件

        /// <summary>
        /// 电磁阀反馈更新事件函数。
        /// </summary>
        /// <param name="sender">电磁执行器</param>
        private void HS_EOVsUpdateFedback(IDataFeedback sender)
        {
            var eove = sender as HS_EOVPIDExecuter;
            if (eove != null)
            {
                try
                {
                    eove.FedbackData = eove.TargetVal;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                    throw;
                }
            }
        }

        /// <summary>
        /// 电磁阀执行输出事件函数。
        /// </summary>
        /// <param name="sender">电磁阀执行器。</param>
        /// <param name="executedVal">需要执行的值。</param>
        private void HS_EOVsExecuteChanged(object sender, double executedVal)
        {
            var eove = sender as HS_EOVPIDExecuter;
            if (eove != null)
            {
                try
                {
                    EOVHMISetGroup.Write(eove.DesignMark, executedVal);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                    throw;
                }
            }
        }

        // 通道控制执行事件。
        private void EOVChnannel_Execute(object sender, ControllerEventArgs e)
        {
            var eovch = sender as FeedbackChannel;
            if (eovch != null)
            {
                var eovexe = eovch.Controller as HS_EOVPIDExecuter;
                if (eovexe != null)
                {
                    eovexe.TargetVal = eovch.AOValue;
                    eovexe.ExecuteBegin();
                }
            }
        }

        #endregion

        #region 流量控制事件

        private void Flow_StopExecute(object sender, ControllerEventArgs e)
        {
            var flowCh = sender as FeedbackChannel;
            System.Diagnostics.Debug.Assert(flowCh != null && flowCh.Controller != null);

            var exe = flowCh.Controller as Executer;
            
            exe.ExecuteOver();
            // 重置 以免数据过冲。
            exe.Reset();
        }

        private void Flow_Execute(object sender, ControllerEventArgs e)
        {
            var flowCh = sender as FeedbackChannel;
            System.Diagnostics.Debug.Assert(flowCh != null && flowCh.Controller != null);

            var exe = flowCh.Controller as Executer;

            exe.TargetVal = flowCh.AOValue;

            exe.ExecuteBegin();
        }

        #endregion

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
            HS_ElectricHeaterExecuter ehExe = sender as HS_ElectricHeaterExecuter;
            bool be = true;
            if (ehExe != null)
            {
                if (ehExe.Heater.Caption.Contains("RL"))
                {
                    //ehExe.Heater.HeaterTrueRequirMinFlow; 
                    be &= ehExe.Heater.HeaterTrueRequirMinFlow <= _rlFlowChannel.MeasureValue;
                }
                else if(ehExe.Heater.Caption.Contains("EL"))
                {
                    be &= ehExe.Heater.HeaterTrueRequirMinFlow <= _elFlowChannel.MeasureValue;
                }
                else
                {
                    be = false;
                }
            }
            return be;
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
            // 4倍时间从OPC读取数据。
            if (_updatedCount % 4 == 0)
            {
                DIGroup.Read();
                SwitchEOVHMIGroup.Read();
                FanHMIGroup.Read();
                HeaterHMIGroup.Read();
                HeaterHMISetGroup.Read();
                DOHMISetGroup.Read();
            }

            HeaterHMIGroup.Read();
            EOVHMIGroup.Read();
            
            // 从采集设备读取所有通道数据

            double[] mvalues = ADDeviceInteract?.AllChannelsValue ?? new double[48];
 
            // 对数据进行校准。
            if (ADBoxDemarcaters != null && ADBoxDemarcaters.Count == 48)
            {
                for (int i = 0; i < 48; i++)
                {
                    mvalues[i] = ADBoxDemarcaters[i].Demarcate(mvalues[i]);
                }
            }

            if (AnalogyValues == null)
            {
                AnalogyValues = new double[mvalues.Length];
            }
            Array.Copy(mvalues, AnalogyValues, mvalues.Length);

            //
            var chs = Device.Channels;
            int count = chs.Count;

            for (int i = 0; i < count; i++)
            {
                var ch = chs[i];

                int index = 0;
                if (ch.Prompt != null && _devPromptIndexDic.TryGetValue(ch.Prompt, out index))
                {
                    // 数采箱通道更新数据。
                    var amc = ch as IAnalogueMeasure;
                    if (amc != null)
                    {
                        var sensor = amc.Collector as LinerSensor;
                        if (sensor != null)
                        {
                            RangeDemarcater rd = new RangeDemarcater(
                                new mcLogic.SafeRange(sensor.ElectricSignalRange.Low, sensor.ElectricSignalRange.Height),
                                new mcLogic.SafeRange(sensor.Range.Low, sensor.Range.Height)
                                );
                            double dv = 0;
                            if (mvalues[index] > 3)
                            {
                                if (mvalues[index] < 4)
                                {
                                    mvalues[index] = 4;
                                }
                                dv = rd.Demarcate(mvalues[index]);
                            }
                            else
                            {
                                //dv = double.MinValue;
                                dv = -99999;                                
                            }
                            amc.MeasureValue = dv;
                            //amc.MeasureValue = mvalues[index];
                        }
                        else
                        {
                            amc.MeasureValue = mvalues[index];
                        }
                    }

                }
                else
                {
                    #region 读取 除数采箱与OPC AI之外的数据 .

                    // 如果通道的collecter属性是组合通道转换则赋值。
                    var multiCh = ch as AnalogueMeasureChannel;
                    if (multiCh != null)
                    {
                        var cov = multiCh.Collector as MultipelChannelConverter;
                        if (cov != null)
                        {
                            multiCh.MeasureValue = cov.Converte();
                        }
                    }
                    
                    #endregion

                }
            }

            // 2 秒向PLC写入一次测量数据。
            if (_updatedCount % 4 == 0)
            {
                WriteToPLCGetChannels();
                if (_bTest)
                {
                    sb.Append(DateTime.Now.ToLongTimeString());
                    sb.Append(',');
                    IAnalogueMeasure am = Device["FT0101"] as IAnalogueMeasure;
                    sb.Append(am.MeasureValue);
                    sb.Append(',');
                    am = Device["FT0101A"] as IAnalogueMeasure;
                    sb.Append(am.MeasureValue);
                    sb.Append(',');
                    am = Device["PT0103"] as IAnalogueMeasure;
                    sb.Append(am.MeasureValue);
                    sb.Append(',');
                    am = Device["PT0105"] as IAnalogueMeasure;
                    sb.Append(am.MeasureValue);
                    sb.AppendLine();
                }
            }

            _updatedCount++;
        }


        /// <summary>
        /// 将二冷路流量设置到目标流量值，执行PID控制逻辑。
        /// </summary>
        /// <param name="targetFlow">目标流量值。</param>
        public void SetFT0101To(double targetFlow)
        {
            var executer = _executerMap["FT0101"];
            executer.TargetVal = targetFlow;
            executer.ExecuteBegin();
        }

        /// <summary>
        /// 将热路流量设置到目标流量值，执行PID控制逻辑。
        /// </summary>
        /// <param name="targetFlow">目标流量值。</param>
        public void SetFT0102To(double targetFlow)
        {
            var executer = _executerMap["FT0102"];
            executer.TargetVal = targetFlow;
            executer.ExecuteBegin();
        }

        /// <summary>
        /// 调用以显示FT0101的PID调节监控器。
        /// </summary>
        public void ShowFT0101_PIDWatcher()
        {
            mcLogic.Execute.Watcher.ExecuteWatcher watcher = new mcLogic.Execute.Watcher.ExecuteWatcher(_executerMap["FT0101"]);
            watcher.ShowWatcherDialog();
        }

        /// <summary>
        /// 调用以显示FT0102的PID调节监控器。
        /// </summary>
        public void ShowFT0102_PIDWatcher()
        {
            mcLogic.Execute.Watcher.ExecuteWatcher watcher = new mcLogic.Execute.Watcher.ExecuteWatcher(_executerMap["FT0102"]);
            watcher.ShowWatcherDialog();
        }

        public void ShowTT0105PIDWatcher()
        {
            mcLogic.Execute.Watcher.ExecuteWatcher watcher = new mcLogic.Execute.Watcher.ExecuteWatcher(_executerMap["TT0105"]);
            watcher.ShowWatcherDialog();
        }

        public void ShowTT0106PIDWatcher()
        {
            mcLogic.Execute.Watcher.ExecuteWatcher watcher = new mcLogic.Execute.Watcher.ExecuteWatcher(_executerMap["TT0106"]);
            watcher.ShowWatcherDialog();
        }

        public void ShowCablibrationForm()
        {
            var cForm = new Calibration.Calibration();
            cForm.HS_Device = this;
            cForm.Show();
        }

        public void SaveCalibrations()
        {
            UpdateADBoxDemarcaters();
            SaveSamplePoints();
        }

        StringBuilder sb = new StringBuilder();
        private bool _bTest = false;
        public void BTest(bool bstart)
        {
            _bTest = bstart;
            if (!_bTest)
            {
                using (StreamWriter sw = new StreamWriter(@"D:\Test.csv", true, Encoding.UTF8))
                {
                    sw.Write(sb.ToString());
                }
                sb.Clear();
            }
        }

        /// <summary>
        /// 初始化样点集合。
        /// </summary>
        private void InitialSamplePoints()
        {
            SamplePoints = new List<SamplePointCollection>(48);
            for (int i = 0; i < 48; i++)
            {
                var spc = new SamplePointCollection();
                spc.Add(new SamplePoint(4, 4));
                spc.Add(new SamplePoint(15, 15));
                SamplePoints.Add(spc);
            }
        }

        /// <summary>
        /// 更新数采箱标定器。
        /// </summary>
        private void UpdateADBoxDemarcaters()
        {
            for (int i = 0; i < 48; i++)
            {
                ADBoxDemarcaters.Add(DemarcateFactory.LinerDemFromSamplePoints(SamplePoints[i]));
            }
        }
        /// <summary>
        /// 将样点集合保存至文件。
        /// </summary>
        private void SaveSamplePoints()
        {
            string path = Environment.CurrentDirectory;
            path += @"\SamplePoints.xml";
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<SamplePointCollection>));
                using (FileStream fs = new FileStream(path,FileMode.Create,FileAccess.Write))
                {
                    xs.Serialize(fs, SamplePoints);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"{ex.Message} \n 保存样点文件错误。");
                throw;
            }
        }
        
        public void SaveElements()
        {
            string eleSetPath = Environment.CurrentDirectory + @"\ElementsSet.xml";
            ChannelsDataset.WriteXml(eleSetPath);
        }
        /// <summary>
        /// 从数据集更新通道信息。
        /// </summary>
        public void UpdateMeasureChannels()
        {
            ChannelsDataset.Channels.UpdateToChannels(Device.Channels);
        }

        public void AddAMeasureChannel(AnalogueMeasureChannel mch)
        {
            ChannelsDataset.Channels.AddChannel(mch);
            CustomChannels.Add(mch);
        }

        public void UpdateAMeasureChannel(AnalogueMeasureChannel mch)
        {
            ChannelsDataset.Channels.UpdateFromChannel(mch);
        }

        /// <summary>
        /// 将测量通道更新到数据集。
        /// </summary>
        public void UpdateAMeasureChannel()
        {
            ChannelsDataset.Channels.UpdateFromChannels(Device.Channels);
        }
        /// <summary>
        /// 删除自定义测量通道。
        /// </summary>
        /// <param name="mch"></param>
        /// <returns>true删除成功。</returns>
        public bool DeleteAMeasureChannel(AnalogueMeasureChannel mch)
        {
            if (CustomChannels.Contains(mch))
            {
                ChannelsDataset.Channels.DeleteChannel(mch);
                Device.RemoveElement(mch);
                CustomChannels.Remove(mch);
            }
            else
            {
                return false;
            }
            return true;
        }
        #endregion

        #region Base class Override

        protected override void OnClosed()
        {
            ADDeviceInteract?.Close();
            
            _updateTimer.Dispose();

            ReleaseHeaters();

            System.Threading.Thread.Sleep(500);

            foreach (var exe in _executerMap)
            {
                System.Diagnostics.Debug.Assert(exe.Value != null);

                exe.Value.ExecuteOver();

                exe.Value.Reset();

                (exe.Value as IDisposable)?.Dispose();
            }

            CloseOpcInteractGroup();

            SaveSamplePoints();
        }

        protected override bool OnClosing()
        {
            _updateTimer.Close();
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
            ADDeviceInteract?.StartAD();
            _updateTimer.Start();
            return true;
        }

        protected override void OnRunning()
        {

        }

        protected override void OnStopped()
        {
            ADDeviceInteract?.StopAD();
            foreach (var heaters in Heaters)
            {
                heaters.Value.Stop();
            }
        }

        protected override bool OnStopping()
        {
            _updateTimer.Stop();
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

                switch (chSetter.Channel.Style)
                {
                    case ExperimentStyle.Measure:
                        {
                            // 在此实现各个测量通道的PID逻辑控制。

                        }
                        break;
                    case ExperimentStyle.Control:
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

                    default:
                        break;
                }
            }
        }

        #endregion
    }
}
