using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
namespace ExpRuntimeApp.ExpWindows
{
    /// <summary>
    /// TestMainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestMainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public TestMainWindow()
        {
            InitializeComponent();
        }

        private void ExpSetBtn_Click(object sender, RoutedEventArgs e)
        {
            ExperimentSetWnd esWnd = new ExperimentSetWnd();
            esWnd.Show();
        }

        private void ExitExpBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenHistoryAnalyseWndBtn_Click(object sender, RoutedEventArgs e)
        {
            HistoryAnalyseWnd hAWnd = new HistoryAnalyseWnd();
            hAWnd.Show();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("是否退出当前试验，并退出应用程序？\n在退出前请做好试验结束工作，关闭相关设备", "退出注意", MessageBoxButton.OKCancel, MessageBoxImage.Hand) == MessageBoxResult.OK)
            {
                e.Cancel = false;                
            }
            else
            {
                e.Cancel = true;                
            }
        }

        private async void BegainExpTgBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            if (tb != null)
            {
                if (tb.IsChecked == true)
                {
                    tb.Content = "停止试验";
                    //var mySettings = new MetroDialogSettings()
                    //{
                    //    NegativeButtonText = "Close now",
                    //    AnimateShow = false,
                    //    AnimateHide = false
                    //};
                    //var edlg = this.ShowProgressAsync(
                    //"退出试验，请等待...", "正在退出试验，执行关键退出步骤，关闭应用程序", true, mySettings
                    //);
                    
                }
                else
                {
                    tb.Content = "开始试验";
                }
            }
        }

        private void OpenGKUIsBtn_Click(object sender, RoutedEventArgs e)
        {
            var gkWnd = new MeasureAndControlWnd();
            gkWnd.Show();
        }

        private void ShowAllChannelsBtn_Click(object sender, RoutedEventArgs e)
        {
            AllChannelsPointsWnd mWnd = new AllChannelsPointsWnd();
            mWnd.Show();
        }

        private void ShowFaultListButton_Click(object sender, RoutedEventArgs e)
        {
            var flWnd = new FualtListWnd();

            flWnd.Show();
        }

        /// <summary>
        /// 异步执行试验关闭逻辑。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MetroWindow_Closed(object sender, EventArgs e)
        {
            var edlg = await this.ShowProgressAsync(
                "退出试验，请等待...", "正在退出试验，执行关键退出步骤，关闭应用程序"
                );
            System.Threading.Thread.Sleep(3000);
            // await edlg
        }
    }
}
