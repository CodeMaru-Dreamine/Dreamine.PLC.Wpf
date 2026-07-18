using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Dreamine.PLC.Abstractions.Clients;
using Dreamine.PLC.Abstractions.Connections;
using Dreamine.PLC.Core.Clients;
using Dreamine.PLC.Core.Devices;
using Dreamine.MVVM.ViewModels;
using Dreamine.PLC.Wpf.Commands;
using Dreamine.PLC.Wpf.Models;

namespace Dreamine.PLC.Wpf.ViewModels;

/// <summary>
/// \if KO
/// <para>제조사에 독립적인 PLC 모니터 ViewModel을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides a vendor-neutral PLC monitor view model.</para>
/// \endif
/// </summary>
public sealed class PlcMonitorViewModel : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    /// \if KO
    /// <para>address Parser 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the address parser value.</para>
    /// \endif
    /// </summary>
    private readonly DefaultPlcAddressParser _addressParser = new();
    /// <summary>
    /// \if KO
    /// <para>client 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the client value.</para>
    /// \endif
    /// </summary>
    private IPlcClient _client;
    /// <summary>
    /// \if KO
    /// <para>channel Name 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the channel name value.</para>
    /// \endif
    /// </summary>
    private string _channelName = "InMemory PLC";
    /// <summary>
    /// \if KO
    /// <para>address Text 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the address text value.</para>
    /// \endif
    /// </summary>
    private string _addressText = "D100";
    /// <summary>
    /// \if KO
    /// <para>count Text 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the count text value.</para>
    /// \endif
    /// </summary>
    private string _countText = "4";
    /// <summary>
    /// \if KO
    /// <para>bit Values Text 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the bit values text value.</para>
    /// \endif
    /// </summary>
    private string _bitValuesText = "1,0,1,0";
    /// <summary>
    /// \if KO
    /// <para>word Values Text 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the word values text value.</para>
    /// \endif
    /// </summary>
    private string _wordValuesText = "100,200,300,400";
    /// <summary>
    /// \if KO
    /// <para>status Message 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the status message value.</para>
    /// \endif
    /// </summary>
    private string _statusMessage = "Ready.";
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
    /// <para>메모리 PLC 클라이언트를 사용해 <see cref="T:Dreamine.PLC.Wpf.ViewModels.PlcMonitorViewModel" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.PLC.Wpf.ViewModels.PlcMonitorViewModel" /> using an in-memory PLC client.</para>
    /// \endif
    /// </summary>
    public PlcMonitorViewModel()
        : this(new InMemoryPlcClient(), "InMemory PLC")
    {
    }

    /// <summary>
    /// \if KO
    /// <para>지정한 PLC 클라이언트를 사용해 <see cref="T:Dreamine.PLC.Wpf.ViewModels.PlcMonitorViewModel" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.PLC.Wpf.ViewModels.PlcMonitorViewModel" /> using the specified PLC client.</para>
    /// \endif
    /// </summary>
    /// <param name="client">
    /// \if KO
    /// <para>모니터링할 PLC 클라이언트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The PLC client to monitor.</para>
    /// \endif
    /// </param>
    /// <param name="channelName">
    /// \if KO
    /// <para>UI에 표시할 채널 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The channel name displayed in the UI.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="client"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="client"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>ViewModel 속성 값이 변경될 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when a view-model property value changes.</para>
    /// \endif
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// \if KO
    /// <para>최신 항목이 앞에 위치하는 PLC 작업 로그를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the PLC operation log with newest entries first.</para>
    /// \endif
    /// </summary>
    public ObservableCollection<PlcOperationLogItem> Logs { get; } = [];

    /// <summary>
    /// \if KO
    /// <para>PLC 연결 명령을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the PLC connection command.</para>
    /// \endif
    /// </summary>
    public ICommand ConnectCommand { get; }

    /// <summary>
    /// \if KO
    /// <para>PLC 연결 해제 명령을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the PLC disconnection command.</para>
    /// \endif
    /// </summary>
    public ICommand DisconnectCommand { get; }

    /// <summary>
    /// \if KO
    /// <para>PLC 비트 읽기 명령을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the PLC bit-read command.</para>
    /// \endif
    /// </summary>
    public ICommand ReadBitsCommand { get; }

    /// <summary>
    /// \if KO
    /// <para>PLC 워드 읽기 명령을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the PLC word-read command.</para>
    /// \endif
    /// </summary>
    public ICommand ReadWordsCommand { get; }

    /// <summary>
    /// \if KO
    /// <para>PLC 비트 쓰기 명령을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the PLC bit-write command.</para>
    /// \endif
    /// </summary>
    public ICommand WriteBitsCommand { get; }

    /// <summary>
    /// \if KO
    /// <para>PLC 워드 쓰기 명령을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the PLC word-write command.</para>
    /// \endif
    /// </summary>
    public ICommand WriteWordsCommand { get; }

    /// <summary>
    /// \if KO
    /// <para>작업 로그 지우기 명령을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the operation-log clear command.</para>
    /// \endif
    /// </summary>
    public ICommand ClearLogCommand { get; }

    /// <summary>
    /// \if KO
    /// <para>채널 표시 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the channel display name.</para>
    /// \endif
    /// </summary>
    public string ChannelName
    {
        get => _channelName;
        set => SetField(ref _channelName, value);
    }

    /// <summary>
    /// \if KO
    /// <para>입력된 PLC 주소 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the entered PLC address text.</para>
    /// \endif
    /// </summary>
    public string AddressText
    {
        get => _addressText;
        set => SetField(ref _addressText, value);
    }

    /// <summary>
    /// \if KO
    /// <para>읽기 개수 입력 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the read-count input text.</para>
    /// \endif
    /// </summary>
    public string CountText
    {
        get => _countText;
        set => SetField(ref _countText, value);
    }

    /// <summary>
    /// \if KO
    /// <para>쓸 비트 값의 쉼표 구분 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the comma-separated bit-values text to write.</para>
    /// \endif
    /// </summary>
    public string BitValuesText
    {
        get => _bitValuesText;
        set => SetField(ref _bitValuesText, value);
    }

    /// <summary>
    /// \if KO
    /// <para>쓸 워드 값의 쉼표 구분 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the comma-separated word-values text to write.</para>
    /// \endif
    /// </summary>
    public string WordValuesText
    {
        get => _wordValuesText;
        set => SetField(ref _wordValuesText, value);
    }

    /// <summary>
    /// \if KO
    /// <para>현재 PLC 연결 상태를 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the current PLC connection state.</para>
    /// \endif
    /// </summary>
    public PlcConnectionState State
    {
        get => _state;
        private set => SetField(ref _state, value);
    }

    /// <summary>
    /// \if KO
    /// <para>사용자에게 표시할 상태 메시지를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the status message displayed to the user.</para>
    /// \endif
    /// </summary>
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetField(ref _statusMessage, value);
    }

    /// <summary>
    /// \if KO
    /// <para>현재 PLC 클라이언트와 상태 이벤트 구독을 새 클라이언트로 교체합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Replaces the current PLC client and state-event subscription with a new client.</para>
    /// \endif
    /// </summary>
    /// <param name="client">
    /// \if KO
    /// <para>새로 모니터링할 PLC 클라이언트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The new PLC client to monitor.</para>
    /// \endif
    /// </param>
    /// <param name="channelName">
    /// \if KO
    /// <para>새 채널 표시 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The new channel display name.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="client"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="client"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    /// <remarks>
    /// \if KO
    /// <para>교체 전 클라이언트는 이 메서드에서 정리하지 않습니다.</para>
    /// \endif
    /// \if EN
    /// <para>This method does not dispose the client being replaced.</para>
    /// \endif
    /// </remarks>
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
    /// \if KO
    /// <para>외부 진단 로그 항목을 PLC 모니터에 추가합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Adds an external diagnostic log entry to the PLC monitor.</para>
    /// \endif
    /// </summary>
    /// <param name="operation">
    /// \if KO
    /// <para>작업 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The operation name.</para>
    /// \endif
    /// </param>
    /// <param name="address">
    /// \if KO
    /// <para>PLC 주소 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The PLC address text.</para>
    /// \endif
    /// </param>
    /// <param name="values">
    /// \if KO
    /// <para>작업 값 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The operation-values text.</para>
    /// \endif
    /// </param>
    /// <param name="isSuccess">
    /// \if KO
    /// <para>작업 성공 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether the operation succeeded.</para>
    /// \endif
    /// </param>
    /// <param name="message">
    /// \if KO
    /// <para>진단 메시지입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The diagnostic message.</para>
    /// \endif
    /// </param>
    public void AppendLog(string operation, string address, string values, bool isSuccess, string message)
    {
        AddLog(operation, address, values, isSuccess, message);
    }

    /// <summary>
    /// \if KO
    /// <para>상태 이벤트 구독을 해제하고 현재 PLC 클라이언트를 비동기로 정리합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Unsubscribes from state events and asynchronously disposes the current PLC client.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>클라이언트의 비동기 정리 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The client's asynchronous disposal operation.</para>
    /// \endif
    /// </returns>
    public async ValueTask DisposeAsync()
    {
        _client.StateChanged -= OnClientStateChanged;
        await _client.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// \if KO
    /// <para>현재 PLC 클라이언트를 연결하고 상태와 로그를 갱신합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Connects the current PLC client and updates status and logging.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>비동기 연결 및 UI 갱신 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing asynchronous connection and UI updates.</para>
    /// \endif
    /// </returns>
    private async Task ConnectAsync()
    {
        var result = await _client.ConnectAsync().ConfigureAwait(true);
        StatusMessage = result.IsSuccess ? "Connected." : result.Message ?? "Connect failed.";
        AddLog("Connect", string.Empty, string.Empty, result.IsSuccess, StatusMessage);
    }

    /// <summary>
    /// \if KO
    /// <para>현재 PLC 클라이언트의 연결을 해제하고 상태와 로그를 갱신합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Disconnects the current PLC client and updates status and logging.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>비동기 연결 해제 및 UI 갱신 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing asynchronous disconnection and UI updates.</para>
    /// \endif
    /// </returns>
    private async Task DisconnectAsync()
    {
        var result = await _client.DisconnectAsync().ConfigureAwait(true);
        StatusMessage = result.IsSuccess ? "Disconnected." : result.Message ?? "Disconnect failed.";
        AddLog("Disconnect", string.Empty, string.Empty, result.IsSuccess, StatusMessage);
    }

    /// <summary>
    /// \if KO
    /// <para>UI 입력에서 비트 읽기 요청을 만들고 결과를 상태와 로그에 표시합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Builds a bit-read request from UI input and displays its result in status and logging.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>비동기 비트 읽기 및 UI 갱신 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing asynchronous bit reading and UI updates.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>UI 입력에서 워드 읽기 요청을 만들고 결과를 상태와 로그에 표시합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Builds a word-read request from UI input and displays its result in status and logging.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>비동기 워드 읽기 및 UI 갱신 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing asynchronous word reading and UI updates.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>UI 입력의 비트 값을 구문 분석해 PLC에 쓰고 결과를 표시합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Parses bit values from UI input, writes them to the PLC, and displays the result.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>비동기 비트 쓰기 및 UI 갱신 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing asynchronous bit writing and UI updates.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>UI 입력의 워드 값을 구문 분석해 PLC에 쓰고 결과를 표시합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Parses word values from UI input, writes them to the PLC, and displays the result.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>비동기 워드 쓰기 및 UI 갱신 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A task representing asynchronous word writing and UI updates.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>모든 작업 로그를 지우고 상태 메시지를 갱신합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Clears all operation logs and updates the status message.</para>
    /// \endif
    /// </summary>
    private void ClearLog()
    {
        Logs.Clear();
        StatusMessage = "Log cleared.";
    }

    /// <summary>
    /// \if KO
    /// <para>주소와 양수 개수 텍스트를 읽기 요청 값으로 구문 분석합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Parses the address and positive count text into read-request values.</para>
    /// \endif
    /// </summary>
    /// <param name="address">
    /// \if KO
    /// <para>성공 시 구문 분석된 PLC 주소를 받습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Receives the parsed PLC address on success.</para>
    /// \endif
    /// </param>
    /// <param name="count">
    /// \if KO
    /// <para>성공 시 양수 읽기 개수를 받습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Receives the positive read count on success.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>두 입력이 모두 유효하면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when both inputs are valid.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>현재 주소 텍스트를 PLC 주소로 구문 분석하고 실패 상태를 기록합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Parses the current address text into a PLC address and records validation failure state.</para>
    /// \endif
    /// </summary>
    /// <param name="address">
    /// \if KO
    /// <para>성공 시 구문 분석된 PLC 주소를 받습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Receives the parsed PLC address on success.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>주소가 유효하면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when the address is valid.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>쉼표 구분 비트 텍스트를 부울 배열로 구문 분석하고 오류를 로그에 기록합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Parses comma-separated bit text into a Boolean array and logs validation errors.</para>
    /// \endif
    /// </summary>
    /// <param name="text">
    /// \if KO
    /// <para>구문 분석할 비트 값 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The bit-values text to parse.</para>
    /// \endif
    /// </param>
    /// <param name="values">
    /// \if KO
    /// <para>성공 시 구문 분석된 비트 배열을 받습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Receives the parsed bit array on success.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>하나 이상의 값이 유효하게 구문 분석되면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when one or more values are parsed successfully.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>쉼표 구분 워드 텍스트를 16비트 정수 배열로 구문 분석하고 오류를 로그에 기록합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Parses comma-separated word text into a 16-bit integer array and logs validation errors.</para>
    /// \endif
    /// </summary>
    /// <param name="text">
    /// \if KO
    /// <para>구문 분석할 워드 값 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The word-values text to parse.</para>
    /// \endif
    /// </param>
    /// <param name="values">
    /// \if KO
    /// <para>성공 시 구문 분석된 워드 배열을 받습니다.</para>
    /// \endif
    /// \if EN
    /// <para>Receives the parsed word array on success.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>하나 이상의 값이 유효하게 구문 분석되면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when one or more values are parsed successfully.</para>
    /// \endif
    /// </returns>
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

    /// <summary>
    /// \if KO
    /// <para>단일 비트 텍스트를 부울 값으로 변환합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Converts one bit-value token to a Boolean value.</para>
    /// \endif
    /// </summary>
    /// <param name="text">
    /// \if KO
    /// <para><c>0</c>, <c>1</c> 또는 부울 문자열입니다.</para>
    /// \endif
    /// \if EN
    /// <para>A <c>0</c>, <c>1</c>, or Boolean string.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>구문 분석된 비트 값입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The parsed bit value.</para>
    /// \endif
    /// </returns>
    /// <exception cref="FormatException">
    /// \if KO
    /// <para>텍스트가 지원되는 비트 형식이 아닐 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when the text is not a supported bit representation.</para>
    /// \endif
    /// </exception>
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

    /// <summary>
    /// \if KO
    /// <para>작업 로그의 앞에 항목을 추가하고 최대 500개로 제한합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Inserts an operation-log item at the front and limits the collection to 500 entries.</para>
    /// \endif
    /// </summary>
    /// <param name="operation">
    /// \if KO
    /// <para>작업 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The operation name.</para>
    /// \endif
    /// </param>
    /// <param name="address">
    /// \if KO
    /// <para>PLC 주소 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The PLC address text.</para>
    /// \endif
    /// </param>
    /// <param name="values">
    /// \if KO
    /// <para>값 텍스트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The values text.</para>
    /// \endif
    /// </param>
    /// <param name="isSuccess">
    /// \if KO
    /// <para>작업 성공 여부입니다.</para>
    /// \endif
    /// \if EN
    /// <para>Whether the operation succeeded.</para>
    /// \endif
    /// </param>
    /// <param name="message">
    /// \if KO
    /// <para>결과 메시지입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The result message.</para>
    /// \endif
    /// </param>
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

    /// <summary>
    /// \if KO
    /// <para>PLC 클라이언트 상태 변경을 ViewModel의 상태 속성에 반영합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Reflects a PLC client state change in the view model's state property.</para>
    /// \endif
    /// </summary>
    /// <param name="sender">
    /// \if KO
    /// <para>상태 이벤트를 발생시킨 클라이언트입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The client that raised the state event.</para>
    /// \endif
    /// </param>
    /// <param name="e">
    /// \if KO
    /// <para>새 PLC 연결 상태입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The new PLC connection state.</para>
    /// \endif
    /// </param>
    private void OnClientStateChanged(object? sender, PlcConnectionState e)
    {
        State = e;
    }

    /// <summary>
    /// \if KO
    /// <para>필드 값이 달라진 경우 값을 갱신하고 속성 변경 알림을 발생시킵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Updates a field and raises property-change notification when the value differs.</para>
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
    /// <para>알림에 사용할 속성 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The property name used for notification.</para>
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
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// \if KO
    /// <para>지정한 속성 이름으로 <see cref="PropertyChanged"/> 이벤트를 발생시킵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Raises <see cref="PropertyChanged"/> for the specified property name.</para>
    /// \endif
    /// </summary>
    /// <param name="propertyName">
    /// \if KO
    /// <para>변경된 속성 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The changed property name.</para>
    /// \endif
    /// </param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
