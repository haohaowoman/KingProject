using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// FirstColdFan.xaml 的交互逻辑
    /// </summary>
    public partial class FirstColdFan : UserControl
    {
        public FirstColdFan()
        {
            InitializeComponent();
        }
        
        public bool FanIsPowerOn
        {
            get { return (bool)GetValue(FanIsPowerOnProperty); }
            set { SetValue(FanIsPowerOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FanIsPowerOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FanIsPowerOnProperty =
            DependencyProperty.Register("FanIsPowerOn", typeof(bool), typeof(FirstColdFan), new PropertyMetadata(false));

        public String FunCaption { get; set; } = "Fun";

        private void FanPowerOnBtn_Click(object sender, RoutedEventArgs e)
        {

        }

    }

    public class TextHeaderHelper
    {

        [Category("NUIs")]
        [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
        public static object GetTextHeader(DependencyObject obj)
        {
            return (object)obj.GetValue(TextHeaderProperty);
        }

        public static void SetTextHeader(DependencyObject obj, object value)
        {
            obj.SetValue(TextHeaderProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextHeaderProperty =
            DependencyProperty.RegisterAttached("TextHeader", typeof(object), typeof(TextHeaderHelper), new PropertyMetadata(default(string)));


    }
}
