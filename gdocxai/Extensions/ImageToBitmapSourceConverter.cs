using System;
using System.Drawing;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Indexai
{
    [ValueConversion(typeof(Image), typeof(BitmapSource))]
    public class ImageToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Image myImage = (Image)value;
            return myImage.ToImageSource();
            /*BitmapSource bitmapSource = myImage.ToImageSource();
            myImage.Dispose();
            return bitmapSource;*/

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}