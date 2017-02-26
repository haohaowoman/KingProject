using System;
using System.Collections.Generic;
using System.Data;
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
using ExpRuntimeApp.ViewModules;
namespace ExpRuntimeApp.ExpWindows
{
    /// <summary>
    /// DataReportWnd.xaml 的交互逻辑
    /// </summary>
    public partial class DataReportWnd : Window
    {
        public DataReportWnd()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewMd = this.DataContext as ExperimentViewModule;
            var dataView = viewMd.CurRutimeReport.HS_DataReport.DefaultView;
            
            ReportGrid.ItemsSource = viewMd.CurRutimeReport.HS_DataReport.DefaultView;
            
        }

        private void ReportGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
