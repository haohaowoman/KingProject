using LabMCESystem.Servers.HS;
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

namespace ExpRuntimeApp.UserControls
{
    /// <summary>
    /// HS_ExpPointSet.xaml 的交互逻辑
    /// </summary>
    public partial class HS_ExpPointSet : UserControl
    {
        public HS_ExpPointSet()
        {
            InitializeComponent();
        }

        private void RL_FlowControlBtn_Click(object sender, RoutedEventArgs e)
        {
            HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
            if (srvRes != null && RL_Flow_ControlValue.Value != null)
            {
                srvRes.HS_Device.SetFT0102To((double)RL_Flow_ControlValue.Value);
            }
        }

        private void ELL_FlowControlBtn_Click(object sender, RoutedEventArgs e)
        {
            HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
            if (srvRes != null && ELL_Flow_ControlValue.Value != null)
            {
                srvRes.HS_Device.SetFT0101To((double)ELL_Flow_ControlValue.Value);
            }
        }
    }
}
