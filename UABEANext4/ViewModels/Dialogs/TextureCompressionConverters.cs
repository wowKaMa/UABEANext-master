using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace UABEANext4.ViewModels.Dialogs.Converters
{
    public class TextureCompressionBrushConverter : IValueConverter
    {
        public IBrush? TrueBrush { get; set; }
        public IBrush? FalseBrush { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b && b) return TrueBrush;
            return FalseBrush;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? FontWeight.Bold : FontWeight.Normal;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class CountToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (value is int count && count > 0);
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
