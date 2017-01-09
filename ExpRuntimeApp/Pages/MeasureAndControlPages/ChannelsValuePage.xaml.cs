using ExpRuntimeApp.Modules;
using ExpRuntimeApp.ViewModules;
using LabMCESystem.LabElement;
using LabMCESystem.ETask;
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

namespace ExpRuntimeApp.Pages.MeasureAndControlPages
{
    /// <summary>
    /// ChannelsValuePage.xaml 的交互逻辑
    /// </summary>
    public partial class ChannelsValuePage : Page
    {
        public ChannelsValuePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 控制产生
            ExperimentViewModule vm = DataContext as ExperimentViewModule;
            Button btn = e.OriginalSource as Button;
            if (btn != null && btn.Tag != null)
            {
                // 通道行
                MdChannel cv = btn.Tag as MdChannel;

                if (cv != null && (cv.Channel.Style & ExperimentStyle.Control) == ExperimentStyle.Control)
                {
                    cv.ControllerExecute();
                }

            }
        }
    }
}
