using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dreamine.PLC.Abstractions.Connections;

namespace Dreamine.PLC.Wpf.Models;

/// <summary>
/// \if KO
/// <para>PLC 모니터 UI에 표시되는 채널 행을 나타냅니다.</para>
/// \endif
/// \if EN
/// <para>Represents a PLC channel row displayed by monitor UI surfaces.</para>
/// \endif
/// </summary>
public sealed class PlcChannelViewItem : INotifyPropertyChanged
{
    /// <summary>
    /// \if KO
    /// <para>name 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the name value.</para>
    /// \endif
    /// </summary>
    private string _name = string.Empty;
    /// <summary>
    /// \if KO
    /// <para>state 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the state value.</para>
    /// \endif
    /// </summary>
    private PlcConnectionState _state = PlcConnectionState.Disconnected;
    /// <summary>
    /// \if KO
    /// <para>description 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the description value.</para>
    /// \endif
    /// </summary>
    private string _description = string.Empty;

    /// <summary>
    /// \if KO
    /// <para>속성 값이 변경될 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when a property value changes.</para>
    /// \endif
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// \if KO
    /// <para>채널 표시 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the channel display name.</para>
    /// \endif
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    /// <summary>
    /// \if KO
    /// <para>현재 채널 연결 상태를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the current channel connection state.</para>
    /// \endif
    /// </summary>
    public PlcConnectionState State
    {
        get => _state;
        set => SetField(ref _state, value);
    }

    /// <summary>
    /// \if KO
    /// <para>채널 설명을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the channel description.</para>
    /// \endif
    /// </summary>
    public string Description
    {
        get => _description;
        set => SetField(ref _description, value);
    }

    /// <summary>
    /// \if KO
    /// <para>필드 값이 달라진 경우 값을 갱신하고 속성 변경 이벤트를 발생시킵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Updates a field and raises the property-change event when the value differs.</para>
    /// \endif
    /// </summary>
    /// <typeparam name="T">
    /// \if KO
    /// <para>필드 값의 형식입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The field-value type.</para>
    /// \endif
    /// </typeparam>
    /// <param name="field">
    /// \if KO
    /// <para>갱신할 필드 참조입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A reference to the field to update.</para>
    /// \endif
    /// </param>
    /// <param name="value">
    /// \if KO
    /// <para>새 값입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The new value.</para>
    /// \endif
    /// </param>
    /// <param name="propertyName">
    /// \if KO
    /// <para>변경 이벤트에 사용할 속성 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The property name used for the change event.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>값이 변경되었으면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the value changed.</para>
    /// \endif
    /// </returns>
    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
