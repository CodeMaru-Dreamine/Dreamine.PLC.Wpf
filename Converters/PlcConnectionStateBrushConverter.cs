using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Dreamine.PLC.Abstractions.Connections;

namespace Dreamine.PLC.Wpf.Converters;

/// <summary>
/// \if KO
/// <para>PLC 연결 상태 값을 표시용 브러시로 변환합니다.</para>
/// \endif
/// \if EN
/// <para>Converts PLC connection-state values to display brushes.</para>
/// \endif
/// </summary>
public sealed class PlcConnectionStateBrushConverter : IValueConverter
{
    /// <summary>
    /// \if KO
    /// <para>PLC 연결 상태를 해당 상태 색상의 WPF 브러시로 변환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Converts a PLC connection state to the WPF brush representing that state.</para>
    /// \endif
    /// </summary>
    /// <param name="value">
    /// \if KO
    /// <para>변환할 연결 상태 값입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The connection-state value to convert.</para>
    /// \endif
    /// </param>
    /// <param name="targetType">
    /// \if KO
    /// <para>바인딩 대상 형식이며 이 변환기에서는 사용하지 않습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The binding target type, which this converter does not use.</para>
    /// \endif
    /// </param>
    /// <param name="parameter">
    /// \if KO
    /// <para>선택적 변환 매개변수이며 이 변환기에서는 사용하지 않습니다.</para>
    /// \endif
    /// \if EN
    /// <para>An optional conversion parameter, which this converter does not use.</para>
    /// \endif
    /// </param>
    /// <param name="culture">
    /// \if KO
    /// <para>변환 문화권이며 이 변환기에서는 사용하지 않습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The conversion culture, which this converter does not use.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>상태별 브러시이며 알 수 없는 값은 회색 브러시입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The state-specific brush, or a gray brush for an unrecognized value.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>단방향 변환기이므로 역변환하지 않고 <see cref="F:System.Windows.Data.Binding.DoNothing" />을 반환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>This is a one-way converter; it does not convert back and returns <see cref="F:System.Windows.Data.Binding.DoNothing" />.</para>
    /// \endif
    /// </summary>
    /// <param name="value">
    /// \if KO
    /// <para>역변환할 값이며 사용하지 않습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The value to convert back, which is ignored.</para>
    /// \endif
    /// </param>
    /// <param name="targetType">
    /// \if KO
    /// <para>대상 형식이며 사용하지 않습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The target type, which is ignored.</para>
    /// \endif
    /// </param>
    /// <param name="parameter">
    /// \if KO
    /// <para>변환 매개변수이며 사용하지 않습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The conversion parameter, which is ignored.</para>
    /// \endif
    /// </param>
    /// <param name="culture">
    /// \if KO
    /// <para>변환 문화권이며 사용하지 않습니다.</para>
    /// \endif
    /// \if EN
    /// <para>The conversion culture, which is ignored.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>항상 <see cref="Binding.DoNothing"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Always <see cref="Binding.DoNothing"/>.</para>
    /// \endif
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}
