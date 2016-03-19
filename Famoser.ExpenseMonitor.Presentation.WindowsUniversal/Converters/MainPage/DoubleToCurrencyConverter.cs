using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Converters.MainPage
{
    class DoubleToCurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var val = (double) value;
                return val.ToString("F");
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            double ret;
            if (double.TryParse(value as string, out ret))
                return ret;
            return null;
        }
    }
}
