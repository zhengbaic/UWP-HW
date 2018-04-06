using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Todos2.converter
{
    public class OpacityAndCheckbox : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Boolean check = (Boolean)value;
            if (check) return 1;
            else return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
