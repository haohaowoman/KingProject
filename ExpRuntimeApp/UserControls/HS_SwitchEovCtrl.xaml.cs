using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// HS_SwitchEovCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class HS_SwitchEovCtrl : UserControl
    {
        public HS_SwitchEovCtrl()
        {
            InitializeComponent();
        }


        public MdChannel SwitchEovChannel
        {
            get { return (MdChannel)GetValue(SwitchEovChannelProperty); }
            set { SetValue(SwitchEovChannelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SwitchEovChannel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SwitchEovChannelProperty =
            DependencyProperty.Register("SwitchEovChannel", typeof(MdChannel), typeof(HS_SwitchEovCtrl), new PropertyMetadata(default(MdChannel)));

        private void CheckSwitchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SwitchEovChannel != null && SwitchEovChannel.AsStatusController != null)
            {
                var sc = SwitchEovChannel.AsStatusController;
                bool ns = SwitchEovChannel.Status;
                string sstr = ns ? "关闭" : "打开";
                
                if (
                    MessageBox.Show($"是否{sstr} {SwitchEovChannel.Label} 开关阀。", "注意", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes
                    )
                {
                    sc.NextStatus = !ns;
                    sc.ControllerExecute();
                }
            }
        }
    }

    [ValueConversion(typeof(bool),typeof(string))]
    public class SwitchStatusStrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bSwitch = (bool)value;
            return bSwitch ? "已打开" : "已关闭";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string sstr = (string)value;
            bool b = false;
            if (sstr == "已打开")
            {
                b = true;
            }
            return b;
        }
    }
    [ValueConversion(typeof(bool),typeof(string))]
    class ElesSwitchCtrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bSwitch = (bool)value;
            return bSwitch ? "关闭" : "打开";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
