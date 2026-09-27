using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace UABEANext4.Converters;

public class BoolToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool b = value is bool v && v;
        string[] colors = (parameter as string ?? "Red:Green").Split(':');
        return Brush.Parse(b ? colors[0] : colors[1]);
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class MeshPresenceColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        int count = value is int c ? c : 0;
        return count > 0 ? Brushes.Cyan : Brushes.White;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class TypeToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string type = value as string ?? "";
        return type switch
        {
            "Menu" => "📂",
            "Parameter" => "⚙️",
            _ => "📄"
        };
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class ErrorColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isError = value is bool b && b;
        return isError ? Brushes.Red : Brushes.White;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}

public class ErrorIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isError = value is bool b && b;
        return isError ? "❌" : "✅";
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => null;
}
