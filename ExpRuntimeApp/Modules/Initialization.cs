using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabMCESystem.BaseService;
using LabMCESystem.LabElement;

namespace ExpRuntimeApp.Modules
{
    static class Initialization
    {
        public static void InitiManagement(LabElementManageBase lm)
        {
        //    LabDevice dev = new LabDevice("模拟量采集", 1);
        //    for (int i = 0; i < 48; i++)
        //    {
        //        dev.AddElement(new AnalogueMeasureChannel($"Ch:{i + 1:D2}") { Frequence = 1000 });
        //    }

        //    LabDevice.ReBuildDeviceID(dev);

        //    bool? br = lm?.RegistNewDevice(dev);
        //    if (br != true)
        //    {
        //        System.Windows.MessageBox.Show("注册设备失败");
        //    }
        //    // 一冷
        //    ExperimentalArea area = new ExperimentalArea("一冷");

        //    area.AddElement(new ExperimentalPoint("空气进口流量") { Unit="L/min" });
        //    area.AddElement(new ExperimentalPoint("空气出口流量") { Unit = "L/min" });
        //    area.AddElement(new ExperimentalPoint("空气进口温度") { Unit = "℃" });
        //    area.AddElement(new ExperimentalPoint("空气出口温度") { Unit = "℃" });
        //    area.AddElement(new ExperimentalPoint("空气进口压力") { Unit = "KPa" });
        //    area.AddElement(new ExperimentalPoint("空气出口压力") { Unit = "KPa" });

        //    lm?.RegistNewExpArea(area);

        //    // 二冷
        //    area = new ExperimentalArea("二冷");

        //    area.AddElement(new ExperimentalPoint("空气进口流量") { Unit = "L/min" });
        //    area.AddElement(new ExperimentalPoint("空气出口流量") { Unit = "L/min" });
        //    area.AddElement(new ExperimentalPoint("空气进口温度") { Unit = "℃" });
        //    area.AddElement(new ExperimentalPoint("空气出口温度") { Unit = "℃" });
        //    area.AddElement(new ExperimentalPoint("空气进口压力") { Unit = "KPa" });
        //    area.AddElement(new ExperimentalPoint("空气出口压力") { Unit = "KPa" });

        //    lm?.RegistNewExpArea(area);

        //    //热边
        //    area = new ExperimentalArea("二冷");

        //    area.AddElement(new ExperimentalPoint("空气进口流量") { Unit = "L/min" });
        //    area.AddElement(new ExperimentalPoint("空气出口流量") { Unit = "L/min" });
        //    area.AddElement(new ExperimentalPoint("空气进口温度") { Unit = "℃" });
        //    area.AddElement(new ExperimentalPoint("空气出口温度") { Unit = "℃" });
        //    area.AddElement(new ExperimentalPoint("空气进口压力") { Unit = "KPa" });
        //    area.AddElement(new ExperimentalPoint("空气出口压力") { Unit = "KPa" });

        //    lm?.RegistNewExpArea(area);

        }
    }
}
