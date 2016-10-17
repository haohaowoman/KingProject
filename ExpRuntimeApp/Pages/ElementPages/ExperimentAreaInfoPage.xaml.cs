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

namespace ExpRuntimeApp.Pages.ElementPages
{
    /// <summary>
    /// ExperimentAreaInfoPage.xaml 的交互逻辑
    /// </summary>
    public partial class ExperimentAreaInfoPage : Page
    {
        public ExperimentAreaInfoPage()
        {
            InitializeComponent();
            
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        //private void epGrid_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    CollectionView v = (CollectionView)CollectionViewSource.GetDefaultView(epGrid.ItemsSource);

        //    if (v.CanGroup)
        //    {
        //        v.GroupDescriptions.Add(new PropertyGroupDescription("LabGroup"));
        //    }
        //}
    }
}
