﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Famoser.ExpenseMonitor.Presentation.WindowsUniversal.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var val = (bool) value;
            if (val)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
