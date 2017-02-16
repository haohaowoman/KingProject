using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ExpRuntimeApp.UserControls
{
    class SwitchEOVStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool status = (bool)value;
            return status ? "开" : "关";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool nStatus = false;
            string label = (string)value;
            if (label=="开")
            {
                nStatus = true;
            }
            return nStatus;
        }
    }
}
