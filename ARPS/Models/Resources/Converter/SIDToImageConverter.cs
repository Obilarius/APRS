
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ARPS
{
    /// <summary>
    /// Konvertiert eine SID zu einem bestimmten Icon
    /// </summary>
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class SIDToImageConverter : IValueConverter
    {
        public static SIDToImageConverter Instance = new SIDToImageConverter();


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var adElement = ADStructure.GetADElement(value.ToString());

            // Standartmäßiges Bild
            var image = Application.Current.FindResource("Img_User");

            switch ((ADElementType)adElement.Type)
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
