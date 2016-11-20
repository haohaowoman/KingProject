using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LabMCESystem;
using LabMCESystem.LabElement;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using LabMCESystem.BaseService;
using LabMCESystem.Servers;
using LabMCESystem.Servers.HS;

namespace ExpRuntimeApp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
       {
            HS_Server srvRes = TryFindResource("SingleService") as HS_Server;

            //if (srvRes != null)
            //{
            //    // Tyr load server from bin data file.
            //    try
            //    {
            //        BinaryFormatter bf = new BinaryFormatter();
            //        using (FileStream fs = new FileStream(@".\ElementManagement.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //        {
            //            LabElementManageBase mb = bf.Deserialize(fs) as LabElementManageBase;
            //            (srvRes as HS_Server).ElementManager = mb;

            //            mb.ExperimnetAreas[0].SubElements[0].PairedChannel = mb.Devices[0].SubElements[0];
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //        //Initilaize service.
            //        LabElementManageBase mb = (srvRes as HS_Server)?.ElementManager;
            //        Modules.Initialization.InitiManagement(mb);

            //        BinaryFormatter bf = new BinaryFormatter();
            //        using (FileStream fs = new FileStream(@".\ElementManagement.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //        {
            //            bf.Serialize(fs, mb);
            //        }
            //        //Initilaize devices.

            //    }
            //}
            //else
            //{
            //    MessageBox.Show("打开服务资源失败，将不能进行试验。", "错误", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            //}
        }
    }
}
