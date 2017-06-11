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
    /// HS_FlowSensorCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class HS_FlowSensorCtrl : UserControl
    {
        public HS_FlowSensorCtrl()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// 获取设置传感器通道。
        /// </summary>
        public MdChannel FlowChannel
        {
            get { return (MdChannel)GetValue(FlowChannelProperty); }
            set { SetValue(FlowChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FlowChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlowChannelProperty =
            DependencyProperty.Register("FlowChannel", typeof(MdChannel), typeof(HS_FlowSensorCtrl), new PropertyMetadata(null));


    }
}
