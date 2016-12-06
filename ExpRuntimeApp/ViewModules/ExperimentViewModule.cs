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

namespace ExpRuntimeApp.ViewModules
{
    class ExperimentViewModule : IDisposable
    {
        private Timer _readValueTimer;
        
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
                    _curChannelVal = new ObservableCollection<ChannelValue>();

                    var chs = _service.ElementManager.AllChannels;

                    foreach (var ch in chs)
                    {
                        _curChannelVal.Add(new ChannelValue(ch));
                    }

                    // 为集合设备Group CollectionView
                    // 通道以工作方式分类
                    CollectionView cCView = (CollectionView)CollectionViewSource.GetDefaultView(_curChannelVal);
                    if (cCView.CanGroup)
                    {
                        cCView.GroupDescriptions.Add(new PropertyGroupDescription("Channel.Unit"));
                    }

                    // 试验点当前数据初始化
                    _curExpPointVal = new ObservableCollection<ExpPointValue>();

                    var eps = _service.ElementManager.AllExperimentPoints;

                    foreach (var ep in eps)
                    {
                        var epv = new ExpPointValue(ep);
                        foreach (var item in _curChannelVal)
                        {
                            epv.PairedChannelValue = item;
                            if (epv.PairedChannelValue != null)
                            {
                                break;
                            }
                        }
                        _curExpPointVal.Add(epv);
                    }

                    cCView = (CollectionView)CollectionViewSource.GetDefaultView(_curExpPointVal);
                    if (cCView.CanGroup)
                    {
                        cCView.GroupDescriptions.Add(new PropertyGroupDescription("ExpPoint.LabGroup"));
                    }

                    _service.ElementManager.ExperimentAreaesChanged += ElementManager_ExperimentAreaesChanged;

                    _service.ElementManager.ExperimentPointsChanged += ElementManager_ExperimentPointsChanged;

                    // 开始定义读取数据                   
                    _readValueTimer.Start();
                }
            }
        }

        private void ElementManager_ExperimentPointsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ElementManager_ExperimentAreaesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public ExperimentViewModule(HS_Server service) : this()
        {
            Service = service;
        }

        // 当前所有通道的数据值集合
        private ObservableCollection<ChannelValue> _curChannelVal;
        /// <summary>
        /// 获取当前通道的数据值集合
        /// </summary>
        public ObservableCollection<ChannelValue> CurChannelVal
        {
            get { return _curChannelVal; }
        }

        // 当前所有测试点的数据值集合
        private ObservableCollection<ExpPointValue> _curExpPointVal;

        /// <summary>
        /// 获取当前试验测试点的数据值集合
        /// </summary>
        public ObservableCollection<ExpPointValue> CurExpPointVal
        {
            get { return _curExpPointVal; }
        }


        public ExperimentViewModule()
        {
            // 每100ms从服务读取一次数据
            _readValueTimer = new Timer(100);
            _readValueTimer.Elapsed += _readValueTimer_Elapsed;

        }

        private void _readValueTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_service != null)
            {
                // 读取数据
                var eReader = _service.ExpDataExchange.SingleDataReader;

                ExpSingleRTDataArgs args = new ExpSingleRTDataArgs();

                // 更新通道数据
                foreach (var item in _curChannelVal)
                {
                    args.ChKeyCode = item.Channel.KeyCode;
                    //eReader.Read(args);
                    item.Value += 1.1f;
                    if (item.Value > 100.0)
                    {
                        item.Value = 0;
                    }
                    //item.Value = args.RTValue;
                }

                // 更新试验测试点数据
                foreach (var item in _curExpPointVal)
                {
                    if (item.ExpPoint.PairedChannel != null)
                    {
                        args.ChKeyCode = item.ExpPoint.PairedChannel.KeyCode;
                        //eReader.Read(args);
                        //item.Value += 2.1f;
                        
                        if (item.Value > 100.0)
                        {
                            item.Value = 0;
                        }
                        //item.Value = args.RTValue;
                    }
                }
            }
        }

        public void Dispose()
        {
            _readValueTimer?.Stop();
            _readValueTimer?.Dispose();
        }
    }
}
