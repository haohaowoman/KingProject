using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.Servers;
using System.Timers;
using ExpRuntimeApp.Modules;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using LabMCESystem.BaseService.ExperimentDataExchange;
using LabMCESystem.Servers.HS;
using System.Windows.Data;
using LabMCESystem.LabElement;
using System.ComponentModel;
using System.Windows.Input;
using ExpRuntimeApp.DataReport;
using Microsoft.Win32;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ExpRuntimeApp.ExpTask;
using Visifire.Charts;
namespace ExpRuntimeApp.ViewModules
{
    class MCommand : ICommand
    {
        public MCommand()
        {

        }

        public MCommand(EventHandler<object> executed, Predicate<object> canExecute)
        {
            Executed = executed;
            CanExecuteHandler = canExecute;
        }

        public event EventHandler<object> Executed;

        public Predicate<object> CanExecuteHandler { set; get; }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteHandler?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            Executed?.Invoke(this, parameter);
        }
    }

    class ChannelDataPoints
    {
        public string XAxiseLabel { get; set; }

        public DataPointCollection FlowResistance { get; set; }

        public DataPointCollection MissHeatEffic { get; set; }

        public void Clear()
        {
            FlowResistance?.Clear();
            MissHeatEffic?.Clear();
        }

    }

    class ExperimentViewModule : IDisposable, INotifyPropertyChanged
    {

        public ExperimentViewModule()
        {
            // 每100ms从服务读取一次数据
            CurRutimeReport.ExperimentInfo.AddExperimentInfoRow(
                DateTime.Now,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

            _readValueTimer = new Timer(150);
            _readValueTimer.Elapsed += _readValueTimer_Elapsed;

            _cmdCanExeTimer.Interval = TimeSpan.FromMinutes(1);
            _cmdCanExeTimer.Tick += _cmdCanExeTimer_Tick;

            WriteInterval = Properties.Settings.Default.WriteInterval;
            AutoWrite = Properties.Settings.Default.AutoWrite;

            UserName = Properties.Settings.Default.LastUserName;
            UserID = Properties.Settings.Default.LastUserID;
            ExpType = Properties.Settings.Default.LastExpType;
            ProductID = Properties.Settings.Default.LastProductID;
            ProductType = Properties.Settings.Default.LastProductType;
            // 命令

            ExpPointOutputCommand = new MCommand();
            ExpPointOutputCommand.Executed += ExpPointOutputCommand_Executed;
            ExpPointOutputCommand.CanExecuteHandler = (o) =>
            {
                // 风机停止之后需要6分钟才能再次控制。
                bool bEn = !Taskers.IsRunning;
                MdChannel mc = o as MdChannel;
                if (mc != null && mc.Label == "FirstColdFan")
                {
                    bEn = _fanCanExeTime == TimeSpan.Zero;
                }
                return bEn;
            };

            ExpPointOutputStopCommand = new MCommand();
            ExpPointOutputStopCommand.Executed += ExpPointOutputStopCommand_Executed;

            AddDataReportNow = new MCommand();
            AddDataReportNow.Executed += AddDataReportNow_Executed;

            SaveReport = new MCommand();
            SaveReport.Executed += SaveReport_Executed;

            LoadReport = new MCommand();
            LoadReport.Executed += LoadReport_Executed;

            ReportToExcel = new MCommand();
            ReportToExcel.Executed += ReportToExcel_Executed;

            SaveAsReport = new MCommand();
            SaveAsReport.Executed += SaveAsReport_Executed;

            SaveSettings = new MCommand();
            SaveSettings.Executed += (sender, e) =>
            {
                Properties.Settings.Default.Save();
            };

            Taskers.TaskerRun += Taskers_TaskerRun;

            ChannelPrompts = new List<string>();
            for (int i = 0; i < 48; i++)
            {
                ChannelPrompts.Add($"{i / 6 + 1:D2}_Ch{i % 6 + 1}");
            }

            ChartSource = new ObservableCollection<ChannelDataPoints>();
            ChartSource.Add(new ChannelDataPoints()
            {
                XAxiseLabel = "二冷流量",
                FlowResistance = new DataPointCollection(),
                MissHeatEffic = new DataPointCollection()
            });

            ChartSource.Add(new ChannelDataPoints()
            {
                XAxiseLabel = "热路流量",
                FlowResistance = new DataPointCollection(),
                MissHeatEffic = new DataPointCollection()
            });
        }

        public ExperimentViewModule(HS_Server service) : this()
        {
            Service = service;
        }

        #region Properties

        public RoutedCommand TestCommand { get; set; }

        private Timer _readValueTimer;

        private DispatcherTimer _cmdCanExeTimer = new DispatcherTimer();

        private TimeSpan _fanCanExeTime = TimeSpan.Zero;

        public MdChannelsCollection Test { get; set; } = new MdChannelsCollection();

        private HS_Server _service;

        public HS_Server Service
        {
            get
            {
                return _service;
            }
            set
            {
                _service = value;
                if (_service != null)
                {
                    // 在此进行各项初始化

                    // 通道当前数据初始化
                    _mdChannels = new MdChannelsCollection();

                    var chs = _service.ElementManager.Devices[0].Channels;

                    foreach (var ch in chs)
                    {
                        _mdChannels.Add(new MdChannel(ch));
                        Test.Add(new MdChannel(ch));
                    }

                    // 为集合设备Group CollectionView
                    // 通道以工作方式分类
                    CollectionView cCView = (CollectionView)CollectionViewSource.GetDefaultView(_mdChannels);
                    if (cCView.CanGroup)
                    {
                        cCView.GroupDescriptions.Add(new PropertyGroupDescription("Style"));
                    }

                    // 试验点当前数据初始化
                    _mdExperPoints = new MdChannelsCollection();

                    var eps = _service.ElementManager.AllExperimentPoints;

                    foreach (var ep in eps)
                    {
                        var epv = new MdExperPoint(ep);
                        _mdExperPoints.Add(epv);
                    }

                    cCView = (CollectionView)CollectionViewSource.GetDefaultView(_mdExperPoints);
                    if (cCView.CanGroup)
                    {
                        cCView.GroupDescriptions.Add(new PropertyGroupDescription("Area"));
                    }

                    // 通道添加删除事件。
                    _service.HS_Device.Device.GroupChanged += Device_GroupChanged;

                    _service.ElementManager.ExperimentAreaesChanged += ElementManager_ExperimentAreaesChanged;

                    _service.ElementManager.ExperimentPointsChanged += ElementManager_ExperimentPointsChanged;

                    // 异常管理事件。
                    _service.ExcepManager.ActivatedEException += ExcepManager_ActivatedEException;
                    _service.ExcepManager.HandledEException += ExcepManager_HandledEException;

                    InitialDataTabelChannels();
                }
            }
        }

        // 当前所有通道的数据值集合
        private MdChannelsCollection _mdChannels;
        /// <summary>
        /// 获取当前通道的数据值集合
        /// </summary>
        public MdChannelsCollection MdChannels
        {
            get { return _mdChannels; }
        }

        // 当前所有测试点的数据值集合
        private MdChannelsCollection _mdExperPoints;

        /// <summary>
        /// 获取当前试验测试点的数据值集合
        /// </summary>
        public MdChannelsCollection MdExperPoints
        {
            get { return _mdExperPoints; }
        }

        private string _expInformation;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 获取/设置当前试验信息。
        /// </summary>
        public string ExpInformation
        {
            get { return _expInformation; }
            set
            {
                _expInformation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExpInformation)));
            }
        }

        private ObservableCollection<MdChannel> _hassChs;
        /// <summary>
        /// 获取包含传感器的测量通道集合。
        /// </summary>
        public ObservableCollection<MdChannel> HasSensorChannels
        {
            get
            {
                if (_hassChs == null)
                {
                    ObservableCollection<MdChannel> schs = null;
                    if (_mdChannels != null)
                    {
                        schs = new ObservableCollection<MdChannel>();
                        foreach (var ch in _mdChannels)
                        {
                            if (ch.Collector as MdLinserSensor != null)
                            {
                                schs.Add(ch);
                            }
                        }
                    }
                    _hassChs = schs;
                }
                return _hassChs;
            }
        }

        /// <summary>
        /// 获取当前所激活的异常。
        /// </summary>
        public ExceptionActionModule CurrentException { get; private set; } = new ExceptionActionModule();

        public ObservableCollection<ExceptionActionModule> Exceptions
        {
            get
            {
                if (_service.ExcepManager != null)
                {
                    var es = new ObservableCollection<ExceptionActionModule>();
                    foreach (var item in _service.ExcepManager.AppearedEExceptions)
                    {
                        es.Add(new ExceptionActionModule() { Action = item });
                    }
                    return es;
                }
                else
                {
                    return null;
                }

            }
        }

        public HS_ExpDataReport CurRutimeReport { get; set; } = new HS_ExpDataReport();
        /// <summary>
        /// 获取/设置任务集合。
        /// </summary>
        public TaskerCollection Taskers { get; private set; } = new TaskerCollection();

        /// <summary>
        /// 获取基础任务。
        /// </summary>
        public Tasker BaseTasker { get; private set; }

        private bool _autoWrite;
        /// <summary>
        /// 自动写入数据报表。
        /// </summary>
        public bool AutoWrite
        {
            get { return _autoWrite; }
            set
            {
                _autoWrite = value;
                Properties.Settings.Default.AutoWrite = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AutoWrite)));
            }
        }

        private int _writeInterval = 10000;
        /// <summary>
        /// 获取/设置自动写入的时间间隔。
        /// </summary>
        public int WriteInterval
        {
            get { return _writeInterval; }
            set
            {
                _writeInterval = value;
                _readValueTimer.Interval = value * 1000.0;
                Properties.Settings.Default.WriteInterval = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteInterval)));
            }
        }

        private bool _isExpRunning;
        /// <summary>
        /// 设置试验是否运行。
        /// </summary>
        public bool IsExpRunning
        {
            get { return _isExpRunning; }
            set
            {
                _isExpRunning = value;
                if (_isExpRunning)
                {
                    _readValueTimer.Start();
                    ExpTime = DateTime.Now;
                }
                else
                {
                    _readValueTimer.Stop();
                    if (CurRutimeReport.HS_DataReport.Rows.Count > 0 && System.Windows.MessageBox.Show("停止当前试验，是否保存当前实验数据？", "注意", System.Windows.MessageBoxButton.OKCancel, System.Windows.MessageBoxImage.Warning) == System.Windows.MessageBoxResult.OK)
                    {
                        SaveReport.Execute(CurRutimeReport);
                    }

                    if (System.Windows.MessageBox.Show("是否停止当前的所有输出控制？\n\n注意：停止试验之后请等待加热器温度下降至室温再关闭阀门和气源！"
                        , "提示", System.Windows.MessageBoxButton.YesNo,
                        System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.OK)
                    {
                        foreach (var item in _mdChannels)
                        {
                            item?.StopControllerExecute();
                        }
                    }
                }

                var rsc = _mdChannels["ExpRunState"];
                Debug.Assert(rsc != null);

                rsc.NextStatus = value;
                rsc.ControllerExecute();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpRunning)));
            }
        }
        /// <summary>
        /// 获取/设置默认报表文件夹路径。
        /// </summary>
        public string DefualtReportDicPath
        {
            get { return Properties.Settings.Default.DefualtSaveFileDic; }
            set
            {
                DirectoryInfo dinfo = new DirectoryInfo(value);
                if (!dinfo.Exists)
                {
                    try
                    {
                        dinfo.Create();
                        Properties.Settings.Default.DefualtSaveFileDic = dinfo.FullName;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"{ex.Message}\n输入文件夹路径无效，请检查！", "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
                else
                {
                    Properties.Settings.Default.DefualtSaveFileDic = dinfo.FullName;
                }
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(DefualtReportDicPath)));
            }
        }

        public List<string> ChannelPrompts { get; private set; }

        private bool _isShowExpChart;
        /// <summary>
        /// 获取/设置是否绘制试验特性曲线。
        /// </summary>
        public bool IsShowExpChart
        {
            get { return _isShowExpChart; }
            set
            {
                _isShowExpChart = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsShowYlFlowResChart"));
                if (!value)
                {
                    foreach (var item in ChartSource)
                    {
                        item?.Clear();
                    }
                }
            }
        }
        /// <summary>
        /// 曲线数据源集合。
        /// </summary>
        public ObservableCollection<ChannelDataPoints> ChartSource { get; private set; } = new ObservableCollection<ChannelDataPoints>();

        #region ExpInfor

        public DateTime ExpTime
        {
            get
            {
                var t = CurRutimeReport.ExperimentInfo.Rows[0]["_expTime"];
                DateTime rt;
                if (t == null)
                {
                    rt = DateTime.Now;
                }
                else
                {
                    rt = (DateTime)t;
                }
                return rt;
            }
            set
            {
                CurRutimeReport.ExperimentInfo.Rows[0]["_expTime"] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpTime"));
            }
        }

        public string ExpType
        {
            get
            {
                var t = CurRutimeReport.ExperimentInfo.Rows[0]["_expType"];

                return t as string;
            }
            set
            {
                CurRutimeReport.ExperimentInfo.Rows[0]["_expType"] = value;
                Properties.Settings.Default.LastExpType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ExpType"));
            }
        }

        public string UserName
        {
            get
            {
                var t = CurRutimeReport.ExperimentInfo.Rows[0]["_userName"];

                return t as string;
            }
            set
            {
                CurRutimeReport.ExperimentInfo.Rows[0]["_userName"] = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UserName"));
            }
        }

        public string UserID
        {
            get
            {
                var t = CurRutimeReport.ExperimentInfo.Rows[0]["_userID"];

                return t as string;
            }
            set
            {
                CurRutimeReport.ExperimentInfo.Rows[0]["_userID"] = value;
                Properties.Settings.Default.LastUserID = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UserID"));
            }
        }

        public string ProductType
        {
            get
            {
                var t = CurRutimeReport.ExperimentInfo.Rows[0]["_productType"];

                return t as string;
            }
            set
            {
                CurRutimeReport.ExperimentInfo.Rows[0]["_productType"] = value;
                Properties.Settings.Default.LastProductType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProductType"));
            }
        }

        public string ProductID
        {
            get
            {
                var t = CurRutimeReport.ExperimentInfo.Rows[0]["_productID"];

                return t as string;
            }
            set
            {
                CurRutimeReport.ExperimentInfo.Rows[0]["_productID"] = value;
                Properties.Settings.Default.LastProductID = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProductID"));
            }
        }

        #endregion

        #region Commands
        /// <summary>
        /// 实验电控制输出命令。
        /// </summary>
        public MCommand ExpPointOutputCommand { get; private set; }
        /// <summary>
        /// 实验电控制输出停止命令。
        /// </summary>
        public MCommand ExpPointOutputStopCommand { get; private set; }
        /// <summary>
        /// 立即进行数据记录命令。
        /// </summary>
        public MCommand AddDataReportNow { get; set; }
        /// <summary>
        /// 保存报表至XML文件。
        /// </summary>
        public MCommand SaveReport { get; set; }
        /// <summary>
        /// 报表另存为xml文件。
        /// </summary>
        public MCommand SaveAsReport { get; set; }
        /// <summary>
        /// 从XML文件导入报表。
        /// </summary>
        public MCommand LoadReport { get; set; }
        /// <summary>
        /// 将报表导入至Excel表。
        /// </summary>
        public MCommand ReportToExcel { get; set; }
        /// <summary>
        /// 保存程序设置命令。
        /// </summary>
        public MCommand SaveSettings { set; get; }
        #endregion

        #endregion

        private void _readValueTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (AutoWrite)
            {
                App.Current.Dispatcher.Invoke(() => { AddDataReportNow.Execute(CurRutimeReport); });
            }
        }

        private void _cmdCanExeTimer_Tick(object sender, EventArgs e)
        {
            _fanCanExeTime += TimeSpan.FromMinutes(1);
            var lt = (TimeSpan.FromMinutes(7) - _fanCanExeTime);
            if (lt == TimeSpan.Zero)
            {
                _fanCanExeTime = TimeSpan.Zero;
                ExpInformation = "风机已经可以再次启动。";
                _cmdCanExeTimer.Stop();
            }
            else
            {
                ExpInformation = $"风机还剩{lt.TotalMinutes}分钟可以再次启动。";
            }            
        }

        /// <summary>
        /// 环散设备的通道集合改变事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Device_GroupChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems != null)
                        {
                            foreach (var nch in e.NewItems)
                            {
                                var ch = nch as Channel;
                                if (ch != null)
                                {
                                    var nmdch = new MdChannel(ch);
                                    _mdChannels.Add(nmdch);
                                    if (nmdch.AsAIChannel != null)
                                    {
                                        HasSensorChannels.Add(nmdch);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        if (e.OldItems != null)
                        {
                            foreach (var och in e.OldItems)
                            {
                                var ch = och as Channel;
                                if (ch != null)
                                {
                                    var mdch = _mdChannels[ch.Label];
                                    if (mdch != null)
                                    {
                                        _mdChannels.Remove(mdch);
                                        if (mdch.AsAIChannel != null)
                                        {
                                            HasSensorChannels.Remove(mdch);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        // 异常已处理事件。
        private void ExcepManager_HandledEException(object sender, LabMCESystem.EException.HandledEExceptionEventArgs e)
        {
            ExpInformation = "异常已处理";
        }

        // 异常激活事件。
        private void ExcepManager_ActivatedEException(object sender, LabMCESystem.EException.ActivatedEExceptionEventArgs e)
        {
            CurrentException.Action = _service.ExcepManager.GetActiveExcepAction(e.ActivatedEException) as LabMCESystem.EException.EExcepAction;
            ExpInformation = $"异常{e.ActivatedEException}触发。";
        }

        private void ElementManager_ExperimentPointsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ElementManager_ExperimentAreaesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExpPointOutputCommand_Executed(object sender, object e)
        {
            // Paramter is MdChannel?
            MdChannel mCh = e as MdChannel;
            if (mCh != null)
            {
                // 如果控制的是热边与二冷的入口温度的测试点需要对所进流量进行判断选择
                // 并提示用户进行加热器与电炉的选择
                switch (mCh.Label)
                {
                    case "热边空气进口温度":

                        var prpWnd = new ExpWindows.RLTempSetPropWnd();
                        prpWnd.ShowActivated = true;
                        prpWnd.ShowDialogsOverTitleBar = true;
                        if (prpWnd.ShowDialog() == true)
                        {
                            mCh.ControllerExecute();
                        }
                        break;
                    case "二冷空气进口温度":
                        mCh.ControllerExecute();
                        break;
                    default:
                        mCh.ControllerExecute();
                        break;
                }
            }
        }

        private void ExpPointOutputStopCommand_Executed(object sender, object e)
        {
            // Paramter is MdChannel?
            MdChannel mCh = e as MdChannel;
            if (mCh != null)
            {
                mCh.StopControllerExecute();
                if (mCh.Label == "FirstColdFan")
                {
                    var rsc = _mdChannels["风机变频器运行"];
                    if (rsc?.Status == true)
                    {
                        _fanCanExeTime += TimeSpan.FromMinutes(1);
                        _cmdCanExeTimer.Start();
                        ExpInformation = $"风机还剩6分钟可以再次启动。";
                    }
                }
            }
        }

        private void InitialDataTabelChannels()
        {
            //向DataReprot绑定通道。
            var tabel = CurRutimeReport.HS_DataReport;
            tabel.RlFlowChannel = MdExperPoints["热边空气流量"];
            tabel.YlFlowChannel = MdExperPoints["一冷空气流量"];
            tabel.YlInTempChannel = MdExperPoints["一冷空气进口温度"];
            tabel.ElFlowChannel = MdExperPoints["二冷空气流量"];
            tabel.ElInTempChannel = MdExperPoints["二冷空气进口温度"];
            tabel.ElOutTempChannel = MdExperPoints["二冷空气出口温度"];
            tabel.RlOutpressChannel = MdExperPoints["热边空气出口压力"];
            tabel.RlInPressChannel = MdExperPoints["热边空气进口压力"];
            tabel.RlInTempChannel = MdExperPoints["热边空气进口温度"];
            tabel.RlOutTempChannel = MdExperPoints["热边空气出口温度"];
            tabel.ElInPressChannel = MdExperPoints["二冷空气进口压力"];
            tabel.ElOutPressChannel = MdExperPoints["二冷空气出口压力"];
            tabel.RlPressDiffChannel = MdExperPoints["热边压差"];
            tabel.ElPressDiffChannel = MdExperPoints["二冷压差"];
            tabel.HeatEmissEffecChannel = MdExperPoints["散热效率"];

            Tasker t = new Tasker();
            TaskElement te = new TaskElement(MdExperPoints["一冷空气流量"]);
            te.TargetValue = null;
            te.ExecuteCommand = ExpPointOutputCommand;
            te.StopExecuteCommand = ExpPointOutputStopCommand;
            te.AllowTolerance = new Tolerance(100);
            t.Elements.Add(te);

            te = new TaskElement(MdExperPoints["二冷空气流量"]);
            te.TargetValue = 500;
            te.ExecuteCommand = ExpPointOutputCommand;
            te.StopExecuteCommand = ExpPointOutputStopCommand;
            te.AllowTolerance = new Tolerance(50);
            t.Elements.Add(te);

            te = new TaskElement(MdExperPoints["二冷空气进口温度"]);
            te.ExecuteCommand = ExpPointOutputCommand;
            te.StopExecuteCommand = ExpPointOutputStopCommand;
            te.AllowTolerance = new Tolerance(1.5);
            t.Elements.Add(te);

            te = new TaskElement(MdExperPoints["热边空气流量"]);
            te.TargetValue = 500;
            te.ExecuteCommand = ExpPointOutputCommand;
            te.StopExecuteCommand = ExpPointOutputStopCommand;
            te.AllowTolerance = new Tolerance(50);
            t.Elements.Add(te);

            te = new TaskElement(MdExperPoints["热边空气进口温度"]);
            te.ExecuteCommand = ExpPointOutputCommand;
            te.StopExecuteCommand = ExpPointOutputStopCommand;
            te.AllowTolerance = new Tolerance(1.5);
            t.Elements.Add(te);

            BaseTasker = t;
            Taskers.Add(t);
        }

        private void AddDataReportNow_Executed(object sender, object e)
        {
            var nr = CurRutimeReport.HS_DataReport.AddRowNow();

            if (IsShowExpChart)
            {
                ChartSource[0].FlowResistance.Add(new LightDataPoint() { XValue = nr._elFlow, YValue = nr._elPressDiff });
                ChartSource[0].MissHeatEffic.Add(new LightDataPoint() { XValue = nr._elFlow, YValue = nr._heatEmissEffec });

                ChartSource[1].FlowResistance.Add(new LightDataPoint() { XValue = nr._rlFlow, YValue = nr._rlPressDiff });
                ChartSource[1].MissHeatEffic.Add(new LightDataPoint() { XValue = nr._rlFlow, YValue = nr._heatEmissEffec });
            }
        }

        private void LoadReport_Executed(object sender, object e)
        {
            HS_ExpDataReport dataSet = e as HS_ExpDataReport;

            OpenFileDialog sfd = new OpenFileDialog();
            sfd.DefaultExt = "XML 数据文件 (*.xml)|*.xml";
            sfd.Filter = "XML 数据文件 (*.xml)|*.xml";

            sfd.Title = "打开实验数据文件";
            if (sfd.ShowDialog() == true)
            {
                if (dataSet == null)
                {
                    dataSet = new HS_ExpDataReport();
                }
                try
                {
                    dataSet.Clear();
                    dataSet.ReadXml(sfd.FileName);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"{ex.Message}\n打开数据文件{sfd.FileName}失败。", "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }

        }

        private void SaveReport_Executed(object sender, object e)
        {
            HS_ExpDataReport dataSet = e as HS_ExpDataReport;
            if (dataSet != null)
            {
                string dicPath = Properties.Settings.Default.DefualtSaveFileDic;

                var dicInfo = new DirectoryInfo(dicPath);
                if (!dicInfo.Exists)
                {
                    try
                    {
                        dicInfo = Directory.CreateDirectory(dicPath);
                    }
                    catch (Exception)
                    {

                        System.Windows.MessageBox.Show($"创建文件夹{dicPath}失败。", "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }

                }
                dicPath = dicInfo.FullName;
                string filePath = dicPath + $"{ExpTime.ToLongDateString()}{ExpTime:HH mm ss}_{ExpType}.xml";
                dataSet.WriteXml(filePath);
            }

        }

        private void SaveAsReport_Executed(object sender, object e)
        {
            HS_ExpDataReport dataSet = e as HS_ExpDataReport;
            if (dataSet != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "XML 数据文件 (*.xml)|*.xml";
                sfd.Filter = "XML 数据文件 (*.xml)|*.xml";

                sfd.OverwritePrompt = true;
                sfd.Title = "保存数据报表";
                if (sfd.ShowDialog() == true)
                {
                    try
                    {
                        dataSet.WriteXml(sfd.FileName);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"{ex.Message}\n保存数据文件{sfd.FileName}失败。", "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ReportToExcel_Executed(object sender, object e)
        {
            HS_ExpDataReport dataSet = e as HS_ExpDataReport;
            if (dataSet != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "Microsoft Excel 工作表 (*.xlsx)|*.xlsx";
                sfd.Filter = "Microsoft Excel 工作表 (*.xlsx)|*.xlsx|所有(*.*)|*.*";
                sfd.FileName = $"{ExpTime.ToLongDateString()} {ExpTime:HH mm ss} {ExpType}";
                sfd.OverwritePrompt = true;
                sfd.Title = "选择导出Excel文件路径";
                if (sfd.ShowDialog() == true)
                {
                    //dataSet.WriteXml(sfd.FileName);

                    System.Action facMeth = () =>
                    {
                        try
                        {
                            Excel.Application ap = new Application();
                            ap.WindowState = XlWindowState.xlMaximized;
                            ap.DisplayAlerts = false;
                            ap.Visible = false;
                            string tempPath = Environment.CurrentDirectory + @"\ReportTemplate\ExperimentReport.xltx";

                            var workBook = ap.Workbooks.Open(tempPath, System.Type.Missing);

                            Excel.Worksheet wSheet = workBook.Sheets["环散系统实验报表"];

                            int colunmsCount = dataSet.HS_DataReport.Columns.Count;
                            int rowCount = dataSet.HS_DataReport.Rows.Count;
                            wSheet.Cells[1, 1] = dataSet.ExperimentInfo.Rows[0]["_expType"];
                            for (int i = 0; i < rowCount; i++)
                            {
                                for (int j = 1; j < colunmsCount; j++)
                                {
                                    wSheet.Cells[3 + i, j].Value = dataSet.HS_DataReport.Rows[i][j];
                                }
                            }

                            workBook.SaveAs(sfd.FileName);
                            workBook.Close();

                            try
                            {
                                int apPreId = 0;

                                GetWindowThreadProcessId((IntPtr)ap.Hwnd, out apPreId);
                                var p = Process.GetProcessById(apPreId);
                                p.CloseMainWindow();
                                p.Close();
                            }
                            catch (Exception excep)
                            {
                                System.Windows.MessageBox.Show($"{excep.Message}\n关闭{ap.Name}失败", "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                            }

                        }
                        catch (Exception excep)
                        {
                            System.Windows.MessageBox.Show($"{excep.Message}\n 保存Excel失败。", "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        }
                    };

                    Task.Factory.StartNew(facMeth);
                }
            }

        }

        private void Taskers_TaskerRun(object sender, TaskerRunEventArgs e)
        {
            switch (e.State)
            {
                case TaskerState.Wait:
                    break;
                case TaskerState.Running:
                    {
                        ExpInformation = "工步正在运行";
                        if (e.ThroughTick > 0)
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                AddDataReportNow.Execute(this);
                            });
                        }
                    }
                    break;
                case TaskerState.Overred:
                    {
                        ExpInformation = "工步已运行完成";
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            if (
                            System.Windows.MessageBox.Show(
                                "当前工步已运行完成，是否继续运行下一个工步?",
                                "提示",
                                System.Windows.MessageBoxButton.YesNo,
                                System.Windows.MessageBoxImage.Question
                                ) == System.Windows.MessageBoxResult.Yes
                                )
                            {
                                Taskers.RunNextTasker();
                            }
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        public void Dispose()
        {
            _readValueTimer?.Stop();
            _readValueTimer?.Dispose();
            Taskers.Dispose();
            Taskers.Clear();
            Properties.Settings.Default.Save();
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int id);
    }
}
