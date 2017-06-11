using ExpRuntimeApp.Modules;
using LabMCESystem.LabElement;
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

        public DataTemplate StatusOutputTemplate { get; set; }

        public DataTemplate StatusTemplate { get; set; }

        public DataTemplate DefualtMeasureDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            MdChannel cv = item as MdChannel;
            if (cv != null && (container as FrameworkElement) != null)
            {
                DataTemplate rt = null;
                switch (cv.Style)
                {
                    case ExperimentStyle.Measure:
                        {
                            IAnalogueMeasure am = cv.Channel as IAnalogueMeasure;
                            if (am != null)
                            {
                                switch (am.Unit)
                                {
                                    case "%":
                                        rt = EOVChannelTemplate;
                                        break;
                                    case "℃":
                                        rt = DefualtMeasureDataTemplate;
                                        break;
                                    case "Kg/h":
                                        rt = FlowTemplate;
                                        break;
                                    default:
                                        rt = DefualtMeasureDataTemplate;
                                        break;
                                }
                            }
                        }
                        break;
                    case ExperimentStyle.Control:
                        {
                            IController se = cv.Channel as IController;
                            if (se != null)
                            {
                                rt = StatusOutputTemplate;
                            }
                        }
                        break;
                    case ExperimentStyle.Feedback:
                        {
                            IFeedback fb = cv.Channel as IFeedback;
                            if (fb != null)
                            {
                                switch (fb.Unit)
                                {
                                    case "%":
                                        rt = EOVChannelTemplate;
                                        break;
                                    case "℃":
                                        rt = TemperatureTemplate;
                                        break;
                                    case "Kg/h":
                                        rt = FlowTemplate;
                                        break;
                                    default:
                                        rt = FlowTemplate;
                                        break;
                                }
                            }
                        }
                        break;
                    case ExperimentStyle.Status:
                        {
                            IStatusExpress se = cv.Channel as IStatusExpress;
                            if (se != null)
                            {
                                rt = StatusTemplate;
                            }
                        }
                        break;
                    case ExperimentStyle.StatusControl:
                        {
                            IStatusController se = cv.Channel as IStatusController;
                            if (se != null)
                            {
                                rt = StatusOutputTemplate;
                            }
                        }
                        break;
                    default:
                        break;
                }
                return rt;
            }
            return base.SelectTemplate(item, container);
        }
    }

    class ChValueColumnCellTempateSelector : DataTemplateSelector
    {
        public DataTemplate StatusExpress { get; set; }

        public DataTemplate ValueExpress { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var cv = item as MdChannel;
            var el = container as FrameworkElement;
            if (cv != null && el != null)
            {
                if ((cv.Style & ExperimentStyle.Status) == ExperimentStyle.Status)
                {
                    return StatusExpress;
                }
                else
                {
                    return ValueExpress;
                }
            }    

            return base.SelectTemplate(item, container);
        }
    }
}
