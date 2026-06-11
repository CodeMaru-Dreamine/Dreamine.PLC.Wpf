using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Dreamine.PLC.Abstractions.Clients;
using Dreamine.PLC.Abstractions.Connections;
using Dreamine.PLC.Core.Clients;
using Dreamine.PLC.Core.Devices;
using Dreamine.PLC.Wpf.Commands;
using Dreamine.PLC.Wpf.Models;

namespace Dreamine.PLC.Wpf.ViewModels;

/// <summary>
/// Provides a vendor-neutral PLC monitor ViewModel.
/// </summary>
public sealed class PlcMonitorViewModel : INotifyPropertyChanged, IAsyncDisposable
{
    private readonly DefaultPlcAddressParser _addressParser = new();
    private IPlcClient _client;
    private string _channelName = "InMemory PLC";
    private string _addressText = "D100";
    private string _countText = "4";
    private string _bitValuesText = "1,0,1,0";
    private string _wordValuesText = "100,200,300,400";
    private string _statusMessage = "Ready.";
    private PlcConnectionState _state = PlcConnectionState.Disconnected;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcMonitorViewModel"/> class.
    /// </summary>
    public PlcMonitorViewModel()
        : this(new InMemoryPlcClient(), "InMemory PLC")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcMonitorViewModel"/> class.
    /// </summary>
    /// <param name="client">The PLC client.</param>
    /// <param name="channelName">The channel display name.</param>
    public PlcMonitorViewModel(IPlcClient client, string channelName = "PLC")
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _client.StateChanged += OnClientStateChanged;
        _state = _client.State;
        _channelName = channelName;

        ConnectCommand = new AsyncRelayCommand(ConnectAsync);
        DisconnectCommand = new AsyncRelayCommand(DisconnectAsync);
        ReadBitsCommand = new AsyncRelayCommand(ReadBitsAsync);
        ReadWordsCommand = new AsyncRelayCommand(ReadWordsAsync);
        WriteBitsCommand = new AsyncRelayCommand(WriteBitsAsync);
        WriteWordsCommand = new AsyncRelayCommand(WriteWordsAsync);
        ClearLogCommand = new RelayCommand(ClearLog);
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the PLC operation logs.
    /// </summary>
    public ObservableCollection<PlcOperationLogItem> Logs { get; } = [];

    /// <summary>
    /// Gets the connect command.
    /// </summary>
    public ICommand ConnectCommand { get; }

    /// <summary>
    /// Gets the disconnect command.
    /// </summary>
    public ICommand DisconnectCommand { get; }

    /// <summary>
    /// Gets the read bits command.
    /// </summary>
    public ICommand ReadBitsCommand { get; }

    /// <summary>
    /// Gets the read words command.
    /// </summary>
    public ICommand ReadWordsCommand { get; }

    /// <summary>
    /// Gets the write bits command.
    /// </summary>
    public ICommand WriteBitsCommand { get; }

    /// <summary>
    /// Gets the write words command.
    /// </summary>
    public ICommand WriteWordsCommand { get; }

    /// <summary>
    /// Gets the clear log command.
    /// </summary>
    public ICommand ClearLogCommand { get; }

    /// <summary>
    /// Gets or sets the channel display name.
    /// </summary>
    public string ChannelName
    {
        get => _channelName;
        set => SetField(ref _channelName, value);
    }

    /// <summary>
    /// Gets or sets the PLC address text.
    /// </summary>
    public string AddressText
    {
        get => _addressText;
        set => SetField(ref _addressText, value);
    }

    /// <summary>
    /// Gets or sets the read count text.
    /// </summary>
    public string CountText
    {
        get => _countText;
        set => SetField(ref _countText, value);
    }

    /// <summary>
    /// Gets or sets the write bit values text.
    /// </summary>
    public string BitValuesText
    {
        get => _bitValuesText;
        set => SetField(ref _bitValuesText, value);
    }

    /// <summary>
    /// Gets or sets the write word values text.
    /// </summary>
    public string WordValuesText
    {
        get => _wordValuesText;
        set => SetField(ref _wordValuesText, value);
    }

    /// <summary>
    /// Gets the current PLC connection state.
    /// </summary>
    public PlcConnectionState State
    {
        get => _state;
        private set => SetField(ref _state, value);
    }

    /// <summary>
    /// Gets or sets the status message.
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetField(ref _statusMessage, value);
    }

    /// <summary>
    /// Replaces the current PLC client.
    /// </summary>
    /// <param name="client">The new PLC client.</param>
    /// <param name="channelName">The channel display name.</param>
    public void SetClient(IPlcClient client, string channelName)
    {
        ArgumentNullException.ThrowIfNull(client);

        _client.StateChanged -= OnClientStateChanged;
        _client = client;
        _client.StateChanged += OnClientStateChanged;
        ChannelName = channelName;
        State = _client.State;
        AddLog("Client", string.Empty, string.Empty, true, $"Client changed: {channelName}");
    }

    /// <summary>
    /// Adds an external diagnostic log entry to the PLC monitor.
    /// </summary>
    /// <param name="operation">The operation name.</param>
    /// <param name="address">The PLC address.</param>
    /// <param name="values">The operation values.</param>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="message">The diagnostic message.</param>
    public void AppendLog(string operation, string address, string values, bool isSuccess, string message)
    {
        AddLog(operation, address, values, isSuccess, message);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _client.StateChanged -= OnClientStateChanged;
        await _client.DisposeAsync().ConfigureAwait(false);
    }

    private async Task ConnectAsync()
    {
        var result = await _client.ConnectAsync().ConfigureAwait(true);
        StatusMessage = result.IsSuccess ? "Connected." : result.Message ?? "Connect failed.";
        AddLog("Connect", string.Empty, string.Empty, result.IsSuccess, StatusMessage);
    }

    private async Task DisconnectAsync()
    {
        var result = await _client.DisconnectAsync().ConfigureAwait(true);
        StatusMessage = result.IsSuccess ? "Disconnected." : result.Message ?? "Disconnect failed.";
        AddLog("Disconnect", string.Empty, string.Empty, result.IsSuccess, StatusMessage);
    }

    private async Task ReadBitsAsync()
    {
        if (!TryCreateReadRequest(out var address, out var count))
        {
            return;
        }

        var result = await _client.ReadBitsAsync(address, count).ConfigureAwait(true);
        var values = result.Value is null ? string.Empty : string.Join(',', result.Value.Select(x => x ? "1" : "0"));
        StatusMessage = result.IsSuccess ? $"Read bits: {values}" : result.Message ?? "Read bits failed.";
        AddLog("ReadBits", address.ToString(), values, result.IsSuccess, StatusMessage);
    }

    private async Task ReadWordsAsync()
    {
        if (!TryCreateReadRequest(out var address, out var count))
        {
            return;
        }

        var result = await _client.ReadWordsAsync(address, count).ConfigureAwait(true);
        var values = result.Value is null ? string.Empty : string.Join(',', result.Value.Select(x => x.ToString(CultureInfo.InvariantCulture)));
        StatusMessage = result.IsSuccess ? $"Read words: {values}" : result.Message ?? "Read words failed.";
        AddLog("ReadWords", address.ToString(), values, result.IsSuccess, StatusMessage);
    }

    private async Task WriteBitsAsync()
    {
        if (!TryParseAddress(out var address))
        {
            return;
        }

        if (!TryParseBits(BitValuesText, out var values))
        {
            return;
        }

        var result = await _client.WriteBitsAsync(address, values).ConfigureAwait(true);
        var valuesText = string.Join(',', values.Select(x => x ? "1" : "0"));
        StatusMessage = result.IsSuccess ? $"Write bits: {valuesText}" : result.Message ?? "Write bits failed.";
        AddLog("WriteBits", address.ToString(), valuesText, result.IsSuccess, StatusMessage);
    }

    private async Task WriteWordsAsync()
    {
        if (!TryParseAddress(out var address))
        {
            return;
        }

        if (!TryParseWords(WordValuesText, out var values))
        {
            return;
        }

        var result = await _client.WriteWordsAsync(address, values).ConfigureAwait(true);
        var valuesText = string.Join(',', values.Select(x => x.ToString(CultureInfo.InvariantCulture)));
        StatusMessage = result.IsSuccess ? $"Write words: {valuesText}" : result.Message ?? "Write words failed.";
        AddLog("WriteWords", address.ToString(), valuesText, result.IsSuccess, StatusMessage);
    }

    private void ClearLog()
    {
        Logs.Clear();
        StatusMessage = "Log cleared.";
    }

    private bool TryCreateReadRequest(out Abstractions.Devices.PlcAddress address, out int count)
    {
        address = default;
        count = 0;

        if (!TryParseAddress(out address))
        {
            return false;
        }

        if (!int.TryParse(CountText, NumberStyles.Integer, CultureInfo.InvariantCulture, out count) || count <= 0)
        {
            StatusMessage = "Count must be greater than zero.";
            AddLog("Validate", AddressText, CountText, false, StatusMessage);
            return false;
        }

        return true;
    }

    private bool TryParseAddress(out Abstractions.Devices.PlcAddress address)
    {
        var result = _addressParser.Parse(AddressText);
        if (!result.IsSuccess)
        {
            address = default;
            StatusMessage = result.Message ?? "Invalid PLC address.";
            AddLog("Validate", AddressText, string.Empty, false, StatusMessage);
            return false;
        }

        address = result.Value;
        return true;
    }

    private bool TryParseBits(string text, out bool[] values)
    {
        values = [];
        try
        {
            values = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(ParseBit)
                .ToArray();
            return values.Length > 0;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            AddLog("Validate", AddressText, text, false, StatusMessage);
            return false;
        }
    }

    private bool TryParseWords(string text, out short[] values)
    {
        values = [];
        try
        {
            values = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => short.Parse(x, CultureInfo.InvariantCulture))
                .ToArray();
            return values.Length > 0;
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
            AddLog("Validate", AddressText, text, false, StatusMessage);
            return false;
        }
    }

    private static bool ParseBit(string text)
    {
        return text switch
        {
            "1" => true,
            "0" => false,
            _ when bool.TryParse(text, out var value) => value,
            _ => throw new FormatException($"Invalid bit value: {text}")
        };
    }

    private void AddLog(string operation, string address, string values, bool isSuccess, string message)
    {
        Logs.Insert(0, new PlcOperationLogItem
        {
            Time = DateTime.Now,
            Operation = operation,
            Address = address,
            Values = values,
            IsSuccess = isSuccess,
            Message = message
        });

        while (Logs.Count > 500)
        {
            Logs.RemoveAt(Logs.Count - 1);
        }
    }

    private void OnClientStateChanged(object? sender, PlcConnectionState e)
    {
        State = e;
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
