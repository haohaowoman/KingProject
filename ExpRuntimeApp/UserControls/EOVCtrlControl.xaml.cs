using ExpRuntimeApp.Modules;
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
    /// EOVCtrlControl.xaml 的交互逻辑
    /// </summary>
    public partial class EOVCtrlControl : UserControl
    {
        public EOVCtrlControl()
        {
            InitializeComponent();
        }

        public MdChannel EOVChannel
        {
            get { return (MdChannel)GetValue(EOVProperty); }
            set { SetValue(EOVProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EOVChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EOVProperty =
            DependencyProperty.Register("EOVChannel", typeof(MdChannel), typeof(EOVCtrlControl), new PropertyMetadata());

        private void EovAOBtn_Click(object sender, RoutedEventArgs e)
        {
            EOVChannel?.ControllerExecute();
        }
    }
}
