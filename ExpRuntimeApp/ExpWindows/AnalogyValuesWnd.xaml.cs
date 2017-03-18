using LabMCESystem.Servers.HS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace ExpRuntimeApp.ExpWindows
{
    class AValue : INotifyPropertyChanged
    {
        public string Prompt { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private double _avalue;

        public double AnalogyValue
        {
            get { return _avalue; }
            set { _avalue = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AnalogyValue")); }
        }

        public override string ToString()
        {
            return $"{AnalogyValue:F2}";
        }
    }
    /// <summary>
    /// AnalogyValuesWnd.xaml 的交互逻辑
    /// </summary>
    public partial class AnalogyValuesWnd
    {
        public AnalogyValuesWnd()
        {
            InitializeComponent();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += _timer_Tick;
            _timer.Start();
            for (int i = 0; i < 48; i++)
            {                
                _avs.Add(new AValue() { Prompt = $"{i / 6 + 1:D2}_Ch{i % 6 + 1}" });
            }
            ValueGrid.ItemsSource = _avs;
        }

        DispatcherTimer _timer;
        
        ObservableCollection<AValue> _avs = new ObservableCollection<AValue>();

        private void _timer_Tick(object sender, EventArgs e)
        {
            HS_Server srvRes = App.Current.TryFindResource("SingleService") as HS_Server;
            if (srvRes != null)
            {                
                for (int i = 0; i < 48; i++)
                {
                    _avs[i].AnalogyValue = srvRes.HS_Device.AnalogyValues[i];
                }

            }
        }

    }
}
