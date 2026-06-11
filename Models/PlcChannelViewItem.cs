using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dreamine.PLC.Abstractions.Connections;

namespace Dreamine.PLC.Wpf.Models;

/// <summary>
/// Represents a PLC channel row displayed by monitor UI surfaces.
/// </summary>
public sealed class PlcChannelViewItem : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private PlcConnectionState _state = PlcConnectionState.Disconnected;
    private string _description = string.Empty;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the channel display name.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    /// <summary>
    /// Gets or sets the current channel state.
    /// </summary>
    public PlcConnectionState State
    {
        get => _state;
        set => SetField(ref _state, value);
    }

    /// <summary>
    /// Gets or sets the channel description.
    /// </summary>
    public string Description
    {
        get => _description;
        set => SetField(ref _description, value);
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
