
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;

namespace ARPS
{
    /// <summary>
    /// Konvertiert einen UserType zu einem bestimmten Icon
    /// </summary>
    [ValueConversion(typeof(ADElementType), typeof(BitmapImage))]
    public class UserTypeToImageConverter : IValueConverter
    {
        public static UserTypeToImageConverter Instance = new UserTypeToImageConverter();


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Standartmäßiges Bild
            var image = Application.Current.FindResource("Img_User");

            switch ((ADElementType)value)
            {
                case ADElementType.Group:
                    image = Application.Current.FindResource("Img_Group");
                    break;
                case ADElementType.Administrator:
                    image = Application.Current.FindResource("Img_Administrator");
                    break;
                case ADElementType.Computer:
                    image = Application.Current.FindResource("Img_Computer");
                    break;
                default:
                    break;
            }

            return image;
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
