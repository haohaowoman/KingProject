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
    /// HS_PressureSensorCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class HS_PressureSensorCtrl : UserControl
    {
        public HS_PressureSensorCtrl()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// 获取/设置压力通道。
        /// </summary>
        public MdChannel PressureChannel
        {
            get { return (MdChannel)GetValue(PresentChannelProperty); }
            set { SetValue(PresentChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PressureChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PresentChannelProperty =
            DependencyProperty.Register("PressureChannel", typeof(MdChannel), typeof(HS_PressureSensorCtrl), new PropertyMetadata(default(MdChannel)));


    }
}
