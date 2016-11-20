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

        public ChannelValue EOV
        {
            get { return (ChannelValue)GetValue(EOVProperty); }
            set { SetValue(EOVProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EOV.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EOVProperty =
            DependencyProperty.Register("EOV", typeof(ChannelValue), typeof(EOVCtrlControl), new PropertyMetadata());


        public string Mark
        {
            get { return (string)GetValue(MarkProperty); }
            set { SetValue(MarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarkProperty =
            DependencyProperty.Register("Mark", typeof(string), typeof(EOVCtrlControl), new PropertyMetadata("haha"));



    }
}
