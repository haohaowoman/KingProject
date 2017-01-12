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

namespace ExpRuntimeApp.ViewModules
{    
    class ExperimentViewModule : IDisposable
    {

        public ExperimentViewModule()
        {
            // 每100ms从服务读取一次数据
            _readValueTimer = new Timer(150);
            _readValueTimer.Elapsed += _readValueTimer_Elapsed;
        }

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

                    // 开始定义读取数据    
                    //_readValueTimer_Elapsed(_readValueTimer, null);

                    //_readValueTimer.Start();

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
                        if (ch.Channel.Style==ExperimentStyle.Measure)
                        {
                            schs.Add(ch);
                        }
                        
                    }
                }
                return schs;
            }
        }

        private void _readValueTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_service != null)
            {
                // 读取数据
                var eReader = _service.ExpDataExchange.SingleDataReader;

                Random r = new Random();
                // 更新通道数据
                foreach (var item in _mdChannels)
                {
                    IAnalogueMeasure iam = item.Channel as IAnalogueMeasure;

                    if (iam != null)
                    {
                        iam.MeasureValue = r.Next(0, 100) / 1.0;
                    }
                    else
                    {
                        IStatusExpress ise = item.Channel as IStatusExpress;
                        if (ise != null)
                        {
                            ise.Status = !ise.Status;
                        }
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
