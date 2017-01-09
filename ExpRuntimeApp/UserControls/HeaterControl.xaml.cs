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
    /// HeaterControl.xaml 的交互逻辑
    /// </summary>
    public partial class HeaterControl : UserControl
    {
        public HeaterControl()
        {
            InitializeComponent();
        }
        

        public MdChannel TemperatureChannel
        {
            get { return (MdChannel)GetValue(TemperatureChannelProperty); }
            set { SetValue(TemperatureChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TemperatureChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemperatureChannelProperty =
            DependencyProperty.Register("TemperatureChannel", typeof(MdChannel), typeof(HeaterControl), new PropertyMetadata());



        public MdChannel RemoteStatusChannel
        {
            get { return (MdChannel)GetValue(RemoteStatusChannelProperty); }
            set { SetValue(RemoteStatusChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoteStatusChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoteStatusChannelProperty =
            DependencyProperty.Register("RemoteStatusChannel", typeof(MdChannel), typeof(HeaterControl), new PropertyMetadata());



    }
}
