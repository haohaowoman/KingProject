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
using MahApps.Metro.Controls;

namespace ExpRuntimeApp.UserControls
{
    /// <summary>
    /// TesttingMakeSure.xaml 的交互逻辑
    /// 试验之前必须确定的项
    /// </summary>
    public partial class TesttingMakeSure : UserControl
    {


        public bool BeSured
        {
            get { return (bool)GetValue(BeSuredProperty); }
            set { SetValue(BeSuredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BeSured.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeSuredProperty =
            DependencyProperty.Register("BeSured", typeof(bool), typeof(TesttingMakeSure), new PropertyMetadata(default(bool)));

        List<ToggleSwitch> _tss = new List<ToggleSwitch>();
        public TesttingMakeSure()
        {
            InitializeComponent();
            _tss.Add(bSure1);
            _tss.Add(bSure2);
            _tss.Add(bSure3);
            _tss.Add(bSure4);
            _tss.Add(bSure5);
            _tss.Add(bSure6);

        }

        private void DockPanel_Checked(object sender, RoutedEventArgs e)
        {
            var bs = from b in _tss where b.IsChecked == true select b.IsChecked;

            if (bs.Count() == _tss.Count)
            {
                BeSured = true;
            }
            else
            {
                BeSured = false;
            }
        }
    }
}
