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
            LabDevice dev = new LabDevice("模拟量采集", 1);
            for (int i = 0; i < 48; i++)
            {
                dev.AddElement(new LabChannel($"Ch:{i + 1:D2}", ExperimentWorkStyle.Measure) { Frequence = 1000 });
            }

            LabDevice.ReBuildDeviceID(dev);

            bool? br = lm?.RegistNewDevice(dev);
            if (br != true)
            {
                System.Windows.MessageBox.Show("注册设备失败");
            }
            // 一冷
            ExperimentArea area = new ExperimentArea("一冷");

            area.AddElement(new ExperimentPoint("空气进口流量", ExperimentWorkStyle.Measure) { Units="L/min" });
            area.AddElement(new ExperimentPoint("空气出口流量", ExperimentWorkStyle.Measure) { Units = "L/min" });
            area.AddElement(new ExperimentPoint("空气进口温度", ExperimentWorkStyle.Measure) { Units = "℃" });
            area.AddElement(new ExperimentPoint("空气出口温度", ExperimentWorkStyle.Measure) { Units = "℃" });
            area.AddElement(new ExperimentPoint("空气进口压力", ExperimentWorkStyle.Measure) { Units = "KPa" });
            area.AddElement(new ExperimentPoint("空气出口压力", ExperimentWorkStyle.Measure) { Units = "KPa" });

            lm?.RegistNewExpArea(area);

            // 二冷
            area = new ExperimentArea("二冷");

            area.AddElement(new ExperimentPoint("空气进口流量", ExperimentWorkStyle.Measure) { Units = "L/min" });
            area.AddElement(new ExperimentPoint("空气出口流量", ExperimentWorkStyle.Measure) { Units = "L/min" });
            area.AddElement(new ExperimentPoint("空气进口温度", ExperimentWorkStyle.Measure) { Units = "℃" });
            area.AddElement(new ExperimentPoint("空气出口温度", ExperimentWorkStyle.Measure) { Units = "℃" });
            area.AddElement(new ExperimentPoint("空气进口压力", ExperimentWorkStyle.Measure) { Units = "KPa" });
            area.AddElement(new ExperimentPoint("空气出口压力", ExperimentWorkStyle.Measure) { Units = "KPa" });

            lm?.RegistNewExpArea(area);

            //热边
            area = new ExperimentArea("二冷");

            area.AddElement(new ExperimentPoint("空气进口流量", ExperimentWorkStyle.Measure) { Units = "L/min" });
            area.AddElement(new ExperimentPoint("空气出口流量", ExperimentWorkStyle.Measure) { Units = "L/min" });
            area.AddElement(new ExperimentPoint("空气进口温度", ExperimentWorkStyle.Measure) { Units = "℃" });
            area.AddElement(new ExperimentPoint("空气出口温度", ExperimentWorkStyle.Measure) { Units = "℃" });
            area.AddElement(new ExperimentPoint("空气进口压力", ExperimentWorkStyle.Measure) { Units = "KPa" });
            area.AddElement(new ExperimentPoint("空气出口压力", ExperimentWorkStyle.Measure) { Units = "KPa" });

            lm?.RegistNewExpArea(area);

        }
    }
}
