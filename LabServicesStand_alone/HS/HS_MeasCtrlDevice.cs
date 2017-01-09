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

        /// <summary>
        /// 获取/设置实验室的报警温度上限。
        /// </summary>
        public double LabFaultTemprature { get; set; } = 45;

        #endregion

        #region Operators

        /// <summary>
        /// 初始化本机设备
        /// 在此进行创建 登录 和 注册
        /// </summary>
        private void InitialDevice()
        {
            LabDevice dev = new LabDevice("PLC&加热器控制", 01);

            InitialExceptionWatcher();

            InitialMeasureChannels(dev);

            InitialPLCChannels(dev);

            InitialDianluChannels(dev);

            InitialFanDevice(dev);

            InitialStatusChannel(dev);
            // 单独为FT0101 与 FT0102创建绑定执行器。

            // 二冷
            Channel ch = dev["FT0101"];

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
                    ext.ExecuteChanged += HS_DigitalEOV_ExecuteChanged;
                }
            }

            // 为执行器添加反馈更新事件与执行事件。
            //foreach (var exe in _executerMap)
            //{
            //    // 电磁调节阀 执行器
            //    if (exe.Value is HS_EOVPIDExecuter)
            //    {
            //        HS_EOVPIDExecuter tempe = exe.Value as HS_EOVPIDExecuter;

            //        tempe.UpdateFedback += HS_EOVsUpdateFedback;

            //        tempe.ExecuteChanged += HS_EOVsExecuteChanged;
            //    }

            //    // 电磁开关阀执行器
            //    if (exe.Value is DigitalExecuter)
            //    {
            //        exe.Value.ExecuteChanged += HS_DigitalEOV_ExecuteChanged;
            //    }
            //}

            // 为采集通道映射数据
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 6; j++)
                {
                    _measureValues.Add($"{i:D2}_Ch{j:D}", 0);
                }
            }

            _mvDicKeys = _measureValues.Keys.ToArray();

            // 为通道映射数据集合
            _chsValueLook = new Dictionary<string, ChannelRTData>();
            foreach (var item in Device.Children)
            {
                _chsValueLook.Add(item.Label, new ChannelRTData(item));
            }
        }

        #region 电磁开关阀执行器事件
        // 电磁开关阀控制。
        private void HS_DigitalEOV_ExecuteChanged(object sender, double executedVal)
        {
            var de = sender as DigitalExecuter;
            if (de != null)
            {
                HS_PLCReadWriter.WriteStatus(de.DesignMark, de.Enable);
            }
        }

        // 电磁开关阀控制事件。
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
                    eove.FedbackData = ReadAnaloge(eove.DesignMark);
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
                    WriteAnaloge(eove.DesignMark, executedVal);
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
        //static int iio = 0;
        // 定时器进行数据的读取和更新。
        private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 从采集设备读取所有通道数据

            Random r = new Random();
            double[] mvalues = new double[48];

            for (int i = 0; i < 48; i++)
            {
                mvalues[i] = r.NextDouble() * 100.0;
            }

            int count = _mvDicKeys.Length;
            for (int i = 0; i < count; i++)
            {
                _measureValues[_mvDicKeys[i]] = mvalues[i];
            }
            
            // 获取 PLC 数据 如果有执行器则更新为执行器的反馈值。
            var chs = Device.Channels;
            count = chs.Count;
            
            for(int i = 0; i < count; i++)
            {
                var ch = chs[i];
                double val;
                if (ch.Prompt != null && _measureValues.TryGetValue(ch.Prompt, out val))
                {
                    (ch as IAnalogueMeasure).MeasureValue = val;
                }
                else
                {
                    // 如果没有找到集合_measureValues的键值 则从PLC OPC服务器中读取数据。
                    switch (ch.Style)
                    {
                        case ExperimentStyle.Measure:
                            (ch as IAnalogueMeasure).MeasureValue = ReadAnaloge(ch.Prompt);
                            break;
                        case ExperimentStyle.Control:
                            ch.Value = ReadAnaloge(ch.Prompt);
                            break;
                        case ExperimentStyle.Feedback:
                            (ch as IAnalogueMeasure).MeasureValue = ReadAnaloge(ch.Prompt);
                            break;
                        case ExperimentStyle.Status:
                            (ch as IStatusExpress).Status = ReadStatus(ch.Prompt);
                            break;
                        case ExperimentStyle.StatusControl:
                            (ch as IStatusExpress).Status = ReadStatus(ch.Prompt);
                            break;
                        default:
                            ch.Value = ReadAnaloge(ch.Prompt);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Base class Override

        protected override void OnClosed()
        {
            _updateTimer.Dispose();
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
            _updateTimer.Start();
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
