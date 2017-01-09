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
namespace ExpRuntimeApp.UserControls
{
    /// <summary>
    /// SwtichEOVControl.xaml 的交互逻辑
    /// </summary>
    public partial class SwtichEOVControl : UserControl
    {
        public SwtichEOVControl()
        {
            InitializeComponent();
        }
        
        public MdChannel SwitchEOVChannel
        {
            get { return (MdChannel)GetValue(SwitchEOVChannelProperty); }
            set { SetValue(SwitchEOVChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SwitchEOVChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SwitchEOVChannelProperty =
            DependencyProperty.Register("SwitchEOVChannel", typeof(MdChannel), typeof(SwtichEOVControl), new PropertyMetadata(default(MdChannel)));

        private void toggleSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SwitchEOVChannel != null && SwitchEOVChannel.AsStatusController != null)
            {
                var sc = SwitchEOVChannel.AsStatusController;
                sc.NextStatus = (bool)toggleSwitchButton.IsChecked;

                sc.ControllerExecute();
            }
        }
    }
}
