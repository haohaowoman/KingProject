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
using System.Windows.Shapes;

namespace ExpRuntimeApp.ExpWindows
{
    /// <summary>
    /// RLTempSetWarnningWnd.xaml 的交互逻辑
    /// </summary>
    public partial class RLTempSetWarnningWnd
    {
        public RLTempSetWarnningWnd(string msg)
        {
            InitializeComponent();
            MessageString = msg;
        }
        

        public string MessageString
        {
            get { return (string)GetValue(MessageStringProperty); }
            set { SetValue(MessageStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageStringProperty =
            DependencyProperty.Register("MessageString", typeof(string), typeof(RLTempSetWarnningWnd), new PropertyMetadata(string.Empty));



        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
