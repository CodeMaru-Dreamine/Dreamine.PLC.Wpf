using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Dreamine.PLC.Abstractions.Connections;

namespace Dreamine.PLC.Wpf.Converters;

/// <summary>
/// Converts PLC connection state values to brushes.
/// </summary>
public sealed class PlcConnectionStateBrushConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            PlcConnectionState.Connected => Brushes.ForestGreen,
            PlcConnectionState.Connecting => Brushes.DarkOrange,
            PlcConnectionState.Disconnecting => Brushes.DarkOrange,
            PlcConnectionState.Faulted => Brushes.Firebrick,
            _ => Brushes.Gray
        };
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
