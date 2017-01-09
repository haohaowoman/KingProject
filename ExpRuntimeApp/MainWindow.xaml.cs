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
using LabMCESystem.LabElement;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LabMCESystem;
using MahApps.Metro.Controls;

namespace ExpRuntimeApp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //for (int i = 0; i < 1000; i++)
            //{
            //    DeviceUnit dev = new DeviceUnit();
            //    dev.UnitNumber = (uint)i;
            //    dev.Title = i.ToString("X");
            //    for (int j = 0; j < 100; j++)
            //    {
            //        dev.Channels.Add(new Channel() { Frequence = 1000000, Lable = string.Format($"Chanel:{i + j}")});
            //    }
            //    LabDevService.SamplingDevices.Add(dev);
            //}
            //XmlSerializer xlms = new XmlSerializer(typeof(List<DeviceUnit>));
            //FileStream fs = new FileStream(@".\aaaa.xml", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            ////xlms.Serialize(fs, LabDevService.SamplingDevices);
            //List<DeviceUnit> ddv = xlms.Deserialize(fs) as List<DeviceUnit>;
            //listBox.ItemsSource = ddv;
            //fs.Dispose();
            
        }

        private void UniformGrid_Click(object sender, RoutedEventArgs e)
        {            
            Button btn = e.Source as Button;
            if (btn != null)
            {
                if (btn.Content.ToString() == "准备/查看")
                {
                    ExpRuntimeApp.ExpWindows.PrepareOrLookWnd preLookWnd = new ExpWindows.PrepareOrLookWnd();
                    preLookWnd.Show();
                }

                if (btn.Content.ToString() == "测控试验")
                {
                    ExpRuntimeApp.ExpWindows.MeasureAndControlWnd mWnd = new ExpWindows.MeasureAndControlWnd();
                    mWnd.Show();
                }
            }
        }
    }
}
