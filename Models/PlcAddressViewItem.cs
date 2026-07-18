using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dreamine.PLC.Abstractions.Devices;

namespace Dreamine.PLC.Wpf.Models;

/// <summary>
/// \if KO
/// <para>PLC 모니터 UI에 표시되는 주소 행을 나타냅니다.</para>
/// \endif
/// \if EN
/// <para>Represents an address row displayed by PLC monitor UI surfaces.</para>
/// \endif
/// </summary>
public sealed class PlcAddressViewItem : INotifyPropertyChanged
{
    /// <summary>
    /// \if KO
    /// <para>device Type 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the device type value.</para>
    /// \endif
    /// </summary>
    private PlcDeviceType _deviceType;
    /// <summary>
    /// \if KO
    /// <para>offset 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the offset value.</para>
    /// \endif
    /// </summary>
    private int _offset;
    /// <summary>
    /// \if KO
    /// <para>bit Offset 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the bit offset value.</para>
    /// \endif
    /// </summary>
    private int? _bitOffset;
    /// <summary>
    /// \if KO
    /// <para>display Name 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the display name value.</para>
    /// \endif
    /// </summary>
    private string _displayName = string.Empty;

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
    /// <para>PLC 장치 형식을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the PLC device type.</para>
    /// \endif
    /// </summary>
    public PlcDeviceType DeviceType
    {
        get => _deviceType;
        set => SetField(ref _deviceType, value);
    }

    /// <summary>
    /// \if KO
    /// <para>PLC 주소 오프셋을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the PLC address offset.</para>
    /// \endif
    /// </summary>
    public int Offset
    {
        get => _offset;
        set => SetField(ref _offset, value);
    }

    /// <summary>
    /// \if KO
    /// <para>선택적 비트 오프셋을 가져오거나 설정합니다. <see langword="null" />은 워드 주소를 나타냅니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the optional bit offset. <see langword="null" /> represents a word address.</para>
    /// \endif
    /// </summary>
    public int? BitOffset
    {
        get => _bitOffset;
        set => SetField(ref _bitOffset, value);
    }

    /// <summary>
    /// \if KO
    /// <para>사용자에게 표시할 주소 레이블을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the user-facing address label.</para>
    /// \endif
    /// </summary>
    public string DisplayName
    {
        get => _displayName;
        set => SetField(ref _displayName, value);
    }

    /// <summary>
    /// \if KO
    /// <para>현재 행 값을 PLC 주소로 변환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Converts the current row values to a PLC address.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>장치 형식과 오프셋을 포함한 PLC 주소입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The PLC address containing the device type and offsets.</para>
    /// \endif
    /// </returns>
    public PlcAddress ToAddress()
    {
        return new PlcAddress(DeviceType, Offset, BitOffset);
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
