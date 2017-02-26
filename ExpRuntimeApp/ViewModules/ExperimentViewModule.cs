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

    class ExperimentViewModule : IDisposable, INotifyPropertyChanged
    {

        public ExperimentViewModule()
        {
            // 每100ms从服务读取一次数据
            _readValueTimer = new Timer(150);
            _readValueTimer.Elapsed += _readValueTimer_Elapsed;

            // 命令

            ExpPointOutputCommand = new MCommand();
            ExpPointOutputCommand.Executed += ExpPointOutputCommand_Executed;
            ExpPointOutputCommand.CanExecuteHandler = (o) => { return true; };

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

            CurRutimeReport.ExperimentInfo.AddExperimentInfoRow(
                DateTime.Now,
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public ExperimentViewModule(HS_Server service) : this()
        {
            Service = service;
        }

        #region Properties

        public RoutedCommand TestCommand { get; set; }

        private Timer _readValueTimer;

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

        /// <summary>
        /// 获取包含传感器的测量通道集合。
        /// </summary>
        public List<MdChannel> HasSensorChannels
        {
            get
            {
                List<MdChannel> schs = null;
                if (_mdChannels != null)
                {
                    schs = new List<MdChannel>();
                    foreach (var ch in _mdChannels)
                    {
                        if (ch.Channel.Style == ExperimentStyle.Measure)
                        {
                            schs.Add(ch);
                        }

                    }
                }
                return schs;
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
        /// 从XML文件导入报表。
        /// </summary>
        public MCommand LoadReport { get; set; }
        /// <summary>
        /// 将报表导入至Excel表。
        /// </summary>
        public MCommand ReportToExcel { get; set; }
        #endregion

        #endregion

        private void _readValueTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

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
        }

        private void AddDataReportNow_Executed(object sender, object e)
        {
            CurRutimeReport.HS_DataReport.AddRowNow();
        }

        private void LoadReport_Executed(object sender, object e)
        {
            HS_ExpDataReport dataSet = e as HS_ExpDataReport;

            OpenFileDialog sfd = new OpenFileDialog();
            sfd.DefaultExt = "XML 数据文件 (*.xml)|*.xlsx";
            sfd.Filter = "XML 数据文件 (*.xml)|*.xlsx";
            
            sfd.Title = "打开实验数据文件";
            if (sfd.ShowDialog() == true)
            {
                if (dataSet == null)
                {
                    dataSet = new HS_ExpDataReport();
                }
                try
                {
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

        private void ReportToExcel_Executed(object sender, object e)
        {
            HS_ExpDataReport dataSet = e as HS_ExpDataReport;
            if (dataSet != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "Microsoft Excel 工作表 (*.xlsx)|*.xlsx";
                sfd.Filter = "Microsoft Excel 工作表 (*.xlsx)|*.xlsx|所有(*.*)|*.*";
                sfd.FileName = $"{ExpTime.ToLongDateString()} {ExpTime.ToLongTimeString():HH mm ss} {ExpType}";
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

        public void Dispose()
        {
            _readValueTimer?.Stop();
            _readValueTimer?.Dispose();
        }
        [DllImport("User32.dll",CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int id);
    }
}
