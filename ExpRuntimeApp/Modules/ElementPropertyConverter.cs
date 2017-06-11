using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
namespace ExpRuntimeApp.Modules
{
    [ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    class EnableToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bshow = (bool)value;
            Visibility v;
            if (bshow)
            {
                v = Visibility.Visible;
            }
            else
            {
                v = Visibility.Collapsed;
            }
            return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility v = (Visibility)value;
            bool enable = false;
            switch (v)
            {
                case Visibility.Visible:
                    enable = true;
                    break;
                case Visibility.Hidden:
                    enable = true;
                    break;
                case Visibility.Collapsed:
                    enable = false;
                    break;
                default:
                    break;
            }
            return enable;
        }
    }
    [ValueConversion(typeof(bool), typeof(bool))]
    class NotBoolenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bshow = (bool)value;
            return !bshow;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bshow = (bool)value;
            return !bshow;
        }
    }
}
