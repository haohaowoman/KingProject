using System;
using System.Collections.Generic;
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
using ExpRuntimeApp.Modules;
using LabMCESystem.Servers.HS;

namespace ExpRuntimeApp.UserControls
{
    /// <summary>
    /// ExpDeviceControlCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class ExpDeviceControlCtrl : UserControl
    {
        public ExpDeviceControlCtrl()
        {
            InitializeComponent();
        }

        private void UniformGrid_Click(object sender, RoutedEventArgs e)
        {
            //Button btn = e.OriginalSource as Button;

            //if (btn?.Tag != null)
            //{
            //    var mdChannel = btn.Tag as MdChannel;
            //    if (mdChannel != null)
            //    {
            //        mdChannel.ControllerExecute();
            //    } 
            //}
        }
        //bool bt = false;
        private void FT0101Watcher_BtnClick(object sender, RoutedEventArgs e)
        {
            HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
            if (srvRes != null)
            {
                srvRes.HS_Device.ShowFT0101_PIDWatcher();
                //srvRes.HS_Device.BTest(bt = !bt);
            }
        }

        private void FT0102Watcher_BtnClick(object sender, RoutedEventArgs e)
        {
            HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
            if (srvRes != null)
            {
                srvRes.HS_Device.ShowFT0102_PIDWatcher();
            }
        }

        private void TT0105Watcher_BtnClick(object sender, RoutedEventArgs e)
        {
            HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
            if (srvRes != null)
            {
                srvRes.HS_Device.ShowTT0105PIDWatcher();
            }
        }

        private void TT0106Watcher_BtnClick(object sender, RoutedEventArgs e)
        {
            HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
            if (srvRes != null)
            {
                srvRes.HS_Device.ShowTT0106PIDWatcher();
            }
        }

        private void JiLiang_BtnClick(object sender, RoutedEventArgs e)
        {
            var gbWnd = new ExpWindows.AnalogyValuesWnd();
            gbWnd.Show();
        }

        private void Calibration_BtnClick(object sender, RoutedEventArgs e)
        {
            HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
            if (srvRes != null)
            {
                srvRes.HS_Device.ShowCablibrationForm();
            }
        }
    }
}
