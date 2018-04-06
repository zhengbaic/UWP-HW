using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Todos2.converter
{
    public class BoolToValue : IValueConverter
    {
        public object Convert(object value, Type targetType,
           object parameter, string language)
        {
            Boolean t = (Boolean)value;
            return t;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            Boolean t = (Boolean)value;
            return t;
        }
    }
}
