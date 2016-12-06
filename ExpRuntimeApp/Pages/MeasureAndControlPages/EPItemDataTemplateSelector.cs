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
    class EPItemDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EPPressureDataTemplate { get; set; }

        public DataTemplate EPFlowDataTemplate { get; set; }

        public DataTemplate EPTemperatureDateTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ExpPointValue epv = item as ExpPointValue;
            if (epv != null)
            {
                // 根据通道的单位判断类型而选择模板
                string unit = epv.ExpPoint.Unit;
                DataTemplate tdt = null;
                switch (unit)
                {
                    case "Kg/h":
                        tdt = EPFlowDataTemplate;
                        break;
                    case "℃":
                        tdt = EPTemperatureDateTemplate;
                        break;
                    case "KPa":
                        tdt = EPPressureDataTemplate;
                        break;
                    default:
                        break;
                }

                return tdt;
            }
            
            return base.SelectTemplate(item, container);
        }
    }
}
