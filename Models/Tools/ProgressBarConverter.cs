using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace PaintWPF.Models.Tools
{
    public class ProgressBarConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            const int maxBarValue = 100;
            if (value is double doubleValue && parameter is double actualWidth)
            {
                return (doubleValue / maxBarValue) * actualWidth;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
