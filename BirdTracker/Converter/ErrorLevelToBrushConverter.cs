using System;
using System.Windows.Data;
using System.Windows.Media;
using log4net.Core;

namespace BirdTracker.Converter
{
    public class ErrorLevelToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush ErrorBrush = new SolidColorBrush(Colors.Red);
        private static readonly SolidColorBrush WarningBrush = new SolidColorBrush(Colors.Orange);
        private static readonly SolidColorBrush InfoBrush = new SolidColorBrush(Colors.White);

        static ErrorLevelToBrushConverter()
        {
            ErrorBrush.Freeze();
            WarningBrush.Freeze();
            InfoBrush.Freeze();
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Brush)) throw new ArgumentException("Expecting targetType Brush");
            if (!(value is Level)) return Binding.DoNothing;
            var errorLevel = (Level)value;
            if (errorLevel >= Level.Error) return ErrorBrush;
            if (errorLevel == Level.Warn) return WarningBrush;
            return InfoBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
