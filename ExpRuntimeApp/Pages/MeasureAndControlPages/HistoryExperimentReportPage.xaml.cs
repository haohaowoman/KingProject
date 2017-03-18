using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO;
using ExpRuntimeApp;
using ExpRuntimeApp.DataReport;
using System.Windows.Threading;

namespace ExpRuntimeApp.Pages.MeasureAndControlPages
{
    /// <summary>
    /// HistoryExperimentReportPage.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryExperimentReportPage : Page
    {
        public HistoryExperimentReportPage()
        {
            InitializeComponent();
            ReportFiles = new ObservableCollection<FileInfo>();
            DataReport = new HS_ExpDataReport();
            _updataTime = new DispatcherTimer();
            _updataTime.Interval = TimeSpan.FromSeconds(2);
            _updataTime.Tick += _updataTime_Tick;
        }

        DispatcherTimer _updataTime;

        public ObservableCollection<FileInfo> ReportFiles
        {
            get { return (ObservableCollection<FileInfo>)GetValue(ReportFilesProperty); }
            set { SetValue(ReportFilesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReportFiles.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReportFilesProperty =
            DependencyProperty.Register("ReportFiles", typeof(ObservableCollection<FileInfo>), typeof(HistoryDataAnalysePage), new PropertyMetadata(null));
        
        public HS_ExpDataReport DataReport
        {
            get { return (HS_ExpDataReport)GetValue(DataReportProperty); }
            set { SetValue(DataReportProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataReport.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataReportProperty =
            DependencyProperty.Register("DataReport", typeof(HS_ExpDataReport), typeof(HistoryDataAnalysePage), new PropertyMetadata(null));
        
        public string ExpType
        {
            get { return (string)GetValue(ExpTypeProperty); }
            set { SetValue(ExpTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpTypeProperty =
            DependencyProperty.Register("ExpType", typeof(string), typeof(HistoryExperimentReportPage), new PropertyMetadata(string.Empty));
        
        public string ProductType
        {
            get { return (string)GetValue(ProductTypeProperty); }
            set { SetValue(ProductTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProductType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProductTypeProperty =
            DependencyProperty.Register("ProductType", typeof(string), typeof(HistoryExperimentReportPage), new PropertyMetadata(string.Empty));
        
        public string ProductID
        {
            get { return (string)GetValue(ProductIDProperty); }
            set { SetValue(ProductIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProductID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProductIDProperty =
            DependencyProperty.Register("ProductID", typeof(string), typeof(HistoryExperimentReportPage), new PropertyMetadata(string.Empty));

        
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            string dpath = ExpRuntimeApp.Properties.Settings.Default.DefualtSaveFileDic;
            DirectoryInfo dinfo = new DirectoryInfo(dpath);
            if (dinfo.Exists)
            {
                var files = dinfo.GetFiles();
                foreach (var file in files)
                {                    
                    if (file.Extension == ".xml")
                    {
                        ReportFiles.Add(file);
                    }
                }
            }
            _updataTime.Start();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var fInfo = e.AddedItems[0] as FileInfo;
                if (fInfo != null)
                {                    
                    try
                    {
                        DataReport.Clear();
                        DataReport.ReadXml(fInfo.FullName);
                        ReportGrid.ItemsSource = DataReport.HS_DataReport.DefaultView;
                        UpdataExperimentInfo();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}\n数据文件错误，请确定数据文件是否有效？", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        
                    }
                }
            }
        }

        private void _updataTime_Tick(object sender, EventArgs e)
        {
            UpdataExperimentInfo();
        }

        private void UpdataExperimentInfo()
        {
            if (DataReport != null && DataReport.ExperimentInfo.Rows.Count > 0)
            {
                ExpType = DataReport.ExperimentInfo.Rows[0]["_expType"] as string;
                ProductType = DataReport.ExperimentInfo.Rows[0]["_productType"] as string;
                ProductID = DataReport.ExperimentInfo.Rows[0]["_productID"] as string;
            }
            else
            {
                ExpType = ProductID = ProductType = string.Empty;
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataReport == null)
            {
                DataReport = new HS_ExpDataReport();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _updataTime.Stop();            
        }
    }
}
