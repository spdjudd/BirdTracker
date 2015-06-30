using System;
using System.Windows.Data;
using System.Windows.Media;

namespace BirdTracker.Converter
{
    public class SignedValueBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush PositiveBrush = Brushes.ForestGreen;
        private static readonly SolidColorBrush NegativeBrush = Brushes.Red;
        private static readonly SolidColorBrush ZeroBrush = Brushes.Transparent;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Brush)) throw new ArgumentException("Expecting targetType Brush");
            if (value is double)
            {
                return (double) value > 0 ? PositiveBrush : (double) value < 0 ? NegativeBrush : ZeroBrush;
            }
            if (value is decimal)
            {
                return (decimal)value > 0 ? PositiveBrush : (decimal)value < 0 ? NegativeBrush : ZeroBrush;                
            }
            if (value is int)
            {
                return (int)value > 0 ? PositiveBrush : (int)value < 0 ? NegativeBrush : ZeroBrush;                
            }
            return ZeroBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
