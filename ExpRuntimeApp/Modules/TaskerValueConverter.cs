using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ExpRuntimeApp.ExpTask;
namespace ExpRuntimeApp.Modules
{
    [ValueConversion(typeof(TaskerState), typeof(string))]
    class TaskerStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = string.Empty;
            TaskerState st = (TaskerState)value;
            switch (st)
            {
                case TaskerState.Wait:
                    str = "等待";
                    break;
                case TaskerState.Running:
                    str = "运行中";
                    break;
                case TaskerState.Overred:
                    str = "完成";
                    break;
                default:
                    break;
            }
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            TaskerState st = TaskerState.Wait;
            switch (str)
            {
                case "等待":
                    st = TaskerState.Wait;
                    break;
                case "运行中":
                    st = TaskerState.Running;
                    break;
                case "完成":
                    st = TaskerState.Overred;
                    break;
                default:
                    break;
            }
            return st;
        }
    }

    [ValueConversion(typeof(Tolerance), typeof(double))]
    class ToleranceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Tolerance)value).Positive;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Tolerance((double)value);
        }
    }

    [ValueConversion(typeof(TaskerState), typeof(bool))]
    class TaskerCanEditConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TaskerState st = (TaskerState)value;
            bool canEdit = st != TaskerState.Running;
            return canEdit;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TaskerState.Wait;
        }
    }

    [ValueConversion(typeof(double?), typeof(bool?))]
    class DoubleNullToBoolenNull : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? dn = value as double?;
            return (bool?)(dn != null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? bm = value as bool?;
            double? dn = null;
            if (bm == true)
            {
                return 0;
            }
            return dn;
        }
    }
}
