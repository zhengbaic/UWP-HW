using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Todos2.converter
{
    class ImageToUri : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new BitmapImage(value as Uri);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value as BitmapImage).UriSource;
        }
    }
}
