using ExpRuntimeApp.Modules;
using LabMCESystem.LabElement;
using LabMCESystem.Servers.HS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExpRuntimeApp.Pages.ElementPages
{
    /// <summary>
    /// AllHasSensorChannelsInfoPage.xaml 的交互逻辑
    /// </summary>
    public partial class AllHasSensorChannelsInfoPage : Page
    {
        public AllHasSensorChannelsInfoPage()
        {
            InitializeComponent();
            AddChannelInfo = new NewChannelInfo();
            
        }
               

        public NewChannelInfo AddChannelInfo
        {
            get { return (NewChannelInfo)GetValue(AddChannelInfoProperty); }
            set { SetValue(AddChannelInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddChannelInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddChannelInfoProperty =
            DependencyProperty.Register("AddChannelInfo", typeof(NewChannelInfo), typeof(AllHasSensorChannelsInfoPage), new PropertyMetadata(default(NewChannelInfo)));

        private void AddChannelBtn_Click(object sender, RoutedEventArgs e)
        {
            HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
            if (srvRes != null)
            {
                var dev = srvRes.HS_Device;
                var nch = LabDevice.CreateChannel(dev.Device, AddChannelInfo.ChannelLabel, ExperimentStyle.Measure) as AnalogueMeasureChannel;
                if (nch == null)
                {
                    MessageBox.Show("创建通道失败，请检查通道名称是否已经占用！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    nch.Summary = AddChannelInfo.ChannelSummary;
                    nch.Unit = AddChannelInfo.SensorUnit;
                    nch.Prompt = AddChannelInfo.ChannelPrompt;
                    nch.Range = new LabMCESystem.QRange(AddChannelInfo.SensorQRangeLow, AddChannelInfo.SensorQRangeHigh);

                    var ssr = new LinerSensor(
                        new LabMCESystem.QRange(AddChannelInfo.SensorElecRangeLow, AddChannelInfo.SensorElecRangHigh),
                        nch.Range);
                    ssr.Label = AddChannelInfo.SensorLabel;
                    ssr.SensorNumber = AddChannelInfo.SensorNumber;
                    ssr.Summary = AddChannelInfo.ChannelSummary;
                    ssr.Unit = AddChannelInfo.SensorUnit;
                    nch.Collector = ssr;
                    bool ba = dev.Device.AddElement(nch);
                    if (ba)
                    {
                        dev.AddAMeasureChannel(nch);
                    }
                    else
                    {
                        MessageBox.Show("创建通道失败，请检查通道名称是否已经占用！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void MakeSureSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
            if (srvRes != null)
            {
                var dev = srvRes.HS_Device;
                dev.UpdateAMeasureChannel();
                dev.SaveElements();
            }
        }

        private void DeleteChannelBtn_Click(object sender, RoutedEventArgs e)
        {
            var mch = ChannelDataGrid.SelectedItem as MdChannel;
            if (mch != null)
            {
                var sch = mch.Channel as AnalogueMeasureChannel;
                HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
                bool bd = false;
                if (srvRes != null && sch != null)
                {
                    var dev = srvRes.HS_Device;
                    bd = dev.DeleteAMeasureChannel(sch);
                    
                }

                if (!bd)
                {
                    MessageBox.Show("删除通道失败，可能删除用户自定义添加的通道，系统固有通道不能被删除。",
                        "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class NewChannelInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _chLabel = "exp1";

        public string ChannelLabel
        {
            get { return _chLabel; }
            set
            {
                _chLabel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChannelLabel"));
            }
        }

        private string _chPrompt = "08_Ch1";

        public string ChannelPrompt
        {
            get { return _chPrompt; }
            set
            {
                _chPrompt = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChannelPrompt"));
            }
        }

        private string _chSummary = "自定义通道";

        public string ChannelSummary
        {
            get { return _chSummary; }
            set
            {
                _chSummary = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ChannelSummary"));
            }
        }

        private string _ssrLabel = "新增传感器";

        public string SensorLabel
        {
            get { return _ssrLabel; }
            set
            {
                _ssrLabel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SensorLabel"));
            }
        }

        private string _ssrNum = "自定义传感器";

        public string SensorNumber
        {
            get { return _ssrNum; }
            set
            {
                _ssrNum = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SensorNumber"));
            }
        }

        private string _ssrUnit = "Kg/h";

        public string SensorUnit
        {
            get { return _ssrUnit; }
            set
            {
                _ssrUnit = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SensorUnit"));
            }
        }

        private double _ssrQRangLow = 0;

        public double SensorQRangeLow
        {
            get { return _ssrQRangLow; }
            set
            {
                _ssrQRangLow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SensorQRangeLow"));
            }
        }

        private double _ssrQRangHigh = 3200;

        public double SensorQRangeHigh
        {
            get { return _ssrQRangHigh; }
            set
            {
                _ssrQRangHigh = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SensorQRangeHigh"));
            }
        }

        private double _ssrElcRangeLow = 4;

        public double SensorElecRangeLow
        {
            get { return _ssrElcRangeLow; }
            set
            {
                _ssrElcRangeLow = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SensorElecRangeLow"));
            }
        }

        private double _ssrElecRangeHigh = 20;

        public double SensorElecRangHigh
        {
            get { return _ssrElecRangeHigh; }
            set
            {
                _ssrElecRangeHigh = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SensorElecRangHigh"));
            }
        }

    }
}
