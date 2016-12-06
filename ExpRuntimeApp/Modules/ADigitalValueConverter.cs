using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ExpRuntimeApp.Modules
{
    /// <summary>
    /// 模拟量到离散量的转换
    /// </summary>
    class ADigitalValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double av = (double)value;
            bool dv = false;
            if (av != 0)
            {
                dv = true;
            }
            return dv;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool dv = (bool)value;
            double av = dv ? 1.0 : 0;
            return av;
        }
    }
}
