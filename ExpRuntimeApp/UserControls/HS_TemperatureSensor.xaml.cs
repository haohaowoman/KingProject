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
    /// HS_TemperatureSensor.xaml 的交互逻辑
    /// </summary>
    public partial class HS_TemperatureSensor : UserControl
    {
        public HS_TemperatureSensor()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 获取/设置温度通道。
        /// </summary>
        public MdChannel TemperatureChannel
        {
            get { return (MdChannel)GetValue(TemperatureChannelProperty); }
            set { SetValue(TemperatureChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TemperatureChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemperatureChannelProperty =
            DependencyProperty.Register("TemperatureChannel", typeof(MdChannel), typeof(HS_TemperatureSensor), new PropertyMetadata(null));


    }
}
