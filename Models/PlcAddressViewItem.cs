using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dreamine.PLC.Abstractions.Devices;

namespace Dreamine.PLC.Wpf.Models;

/// <summary>
/// Represents an address row displayed by PLC monitor UI surfaces.
/// </summary>
public sealed class PlcAddressViewItem : INotifyPropertyChanged
{
    private PlcDeviceType _deviceType;
    private int _offset;
    private int? _bitOffset;
    private string _displayName = string.Empty;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the PLC device type.
    /// </summary>
    public PlcDeviceType DeviceType
    {
        get => _deviceType;
        set => SetField(ref _deviceType, value);
    }

    /// <summary>
    /// Gets or sets the PLC address offset.
    /// </summary>
    public int Offset
    {
        get => _offset;
        set => SetField(ref _offset, value);
    }

    /// <summary>
    /// Gets or sets the optional bit offset. A null value represents a word address.
    /// </summary>
    public int? BitOffset
    {
        get => _bitOffset;
        set => SetField(ref _bitOffset, value);
    }

    /// <summary>
    /// Gets or sets the user-facing address label.
    /// </summary>
    public string DisplayName
    {
        get => _displayName;
        set => SetField(ref _displayName, value);
    }

    /// <summary>
    /// Converts this row to a PLC address.
    /// </summary>
    /// <returns>The PLC address.</returns>
    public PlcAddress ToAddress()
    {
        return new PlcAddress(DeviceType, Offset, BitOffset);
    }

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
