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
    /// HS_EovCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class HS_EovCtrl : UserControl
    {
        public HS_EovCtrl()
        {
            InitializeComponent();
        }


        public MdChannel EovChannel
        {
            get { return (MdChannel)GetValue(EovChannelProperty); }
            set { SetValue(EovChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EovChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EovChannelProperty =
            DependencyProperty.Register("EovChannel", typeof(MdChannel), typeof(HS_EovCtrl), new PropertyMetadata(default(MdChannel)));

        private void EovAOBtn_Click(object sender, RoutedEventArgs e)
        {
            EovChannel?.ControllerExecute();
        }
    }
}
