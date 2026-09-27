using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace UABEANext4.Converters
{
    public class TextureCompressionBoolToBrushConverter : IValueConverter
    {
        public IBrush? TrueBrush { get; set; }
        public IBrush? FalseBrush { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return TrueBrush;
            return FalseBrush;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToFontWeightConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return FontWeight.Bold;
            return FontWeight.Normal;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
