using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Calendar
{
    class EventTextConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() != string.Empty)
            {
                return value;
            }
            else if ((string)parameter == "title")
            {
                return String.Format($"({Properties.Resources.ConverterNoTitle})");
            }
            else if ((string)parameter == "desc")
            {
                return String.Format($"({Properties.Resources.ConverterNoDescription})");
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
