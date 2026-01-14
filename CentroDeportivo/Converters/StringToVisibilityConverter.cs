using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CentroDeportivo.Converters
{
    /// <summary>
    /// Converter que convierte un string en Visibility.
    /// Si el string está vacío o null, devuelve Visible (para mostrar el placeholder).
    /// Si el string tiene contenido, devuelve Collapsed (para ocultar el placeholder).
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            return string.IsNullOrWhiteSpace(str) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

