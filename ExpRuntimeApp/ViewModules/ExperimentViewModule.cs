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

        #region Commands
        /// <summary>
        /// 实验电控制输出命令。
        /// </summary>
        public MCommand ExpPointOutputCommand { get; private set; }
        /// <summary>
        /// 实验电控制输出停止命令。
        /// </summary>
        public MCommand ExpPointOutputStopCommand { get; private set; }

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

                mCh.ControllerExecute();

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


        public void Dispose()
        {
            _readValueTimer?.Stop();
            _readValueTimer?.Dispose();
        }
    }
}
