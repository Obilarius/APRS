
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;

namespace ARPS
{
    /// <summary>
    /// Konvertiert einen DirectoryItemType zu einem bestimmten Icon
    /// </summary>
    [ValueConversion(typeof(DirectoryItemType), typeof(BitmapImage))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Standartmäßiges Bild
            var image = Application.Current.FindResource("Img_Folder");

            switch ((DirectoryItemType)value)
            {
                case DirectoryItemType.Server:
                    image = Application.Current.FindResource("Img_Server");
                    break;
                case DirectoryItemType.SharedFolder:
                    image = Application.Current.FindResource("Img_SharedFolder");
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
