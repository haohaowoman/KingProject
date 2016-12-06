using ExpRuntimeApp.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ExpRuntimeApp.Pages.MeasureAndControlPages
{
    class ChValueRowDetailsTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// 获取/设置电动调节阀的数据模板。
        /// </summary>
        public DataTemplate EOVChannelTemplate { get; set; }
        /// <summary>
        /// 获取/设置测试信号的数据模板。
        /// </summary>
        public DataTemplate TemperatureTemplate { get; set; }

        public DataTemplate FlowTemplate { get; set; }

        public DataTemplate DigitalTemplate { get; set; }

        public DataTemplate DefualtMeasureDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ChannelValue cv = item as ChannelValue;
            if (cv != null)
            {

                if (cv.Channel.Unit == null && cv.Channel.Label.Contains("EV"))
                {
                    return DigitalTemplate;
                }

                switch (cv.Channel.Unit)
                {
                    case "%":
                        return EOVChannelTemplate;

                    case "℃":
                        return TemperatureTemplate;
                    case "Kg/h":
                        return FlowTemplate;
                    default:
                        return DefualtMeasureDataTemplate;
                        //break;
                }

            }
            return base.SelectTemplate(item, container);
        }
    }
}
