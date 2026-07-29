using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using Dreamine.PLC.Abstractions.Connections;
using Dreamine.PLC.Abstractions.Devices;
using Dreamine.PLC.Core.Clients;
using Dreamine.PLC.Wpf.Commands;
using Dreamine.PLC.Wpf.Converters;
using Dreamine.PLC.Wpf.Models;
using Dreamine.PLC.Wpf.Services;
using Dreamine.PLC.Wpf.ViewModels;
using Xunit;

namespace Dreamine.PLC.Wpf.Tests;

public sealed class PlcWpfBehaviorTests
{
    [Fact]
    public void DelegateCommand_ExecutesOnlyWhenAllowed_AndRaisesNotification()
    {
        object? received = null;
        var raised = 0;
        var command = new DelegateCommand(value => received = value, value => Equals(value, "go"));
        command.CanExecuteChanged += (_, _) => raised++;

        command.Execute("stop");
        Assert.Null(received);
        Assert.False(command.CanExecute("stop"));

        command.Execute("go");
        command.RaiseCanExecuteChanged();

        Assert.Equal("go", received);
        Assert.Equal(1, raised);
        Assert.Throws<ArgumentNullException>(() => new DelegateCommand(null!));
    }

    [Fact]
    public void Models_RaisePropertyChanges_AndCreateAddress()
    {
        var changed = new List<string?>();
        var address = new PlcAddressViewItem();
        address.PropertyChanged += (_, args) => changed.Add(args.PropertyName);

        address.DeviceType = PlcDeviceType.D;
        address.Offset = 120;
        address.BitOffset = 3;
        address.DisplayName = "Motor";
        address.DisplayName = "Motor";

        Assert.Equal(new PlcAddress(PlcDeviceType.D, 120, 3), address.ToAddress());
        Assert.Equal(4, changed.Count);

        changed.Clear();
        var channel = new PlcChannelViewItem();
        channel.PropertyChanged += (_, args) => changed.Add(args.PropertyName);
        channel.Name = "Line 1";
        channel.State = PlcConnectionState.Connected;
        channel.Description = "Packaging";

        Assert.Equal(3, changed.Count);
        Assert.Equal("Line 1", channel.Name);
        Assert.Equal(PlcConnectionState.Connected, channel.State);
        Assert.Equal("Packaging", channel.Description);
    }

    [Theory]
    [InlineData(PlcConnectionState.Connected)]
    [InlineData(PlcConnectionState.Connecting)]
    [InlineData(PlcConnectionState.Disconnecting)]
    [InlineData(PlcConnectionState.Faulted)]
    [InlineData(PlcConnectionState.Disconnected)]
    public void ConnectionStateConverter_ReturnsExpectedBrush(PlcConnectionState state)
    {
        var converter = new PlcConnectionStateBrushConverter();
        var expected = state switch
        {
            PlcConnectionState.Connected => Brushes.ForestGreen,
            PlcConnectionState.Connecting or PlcConnectionState.Disconnecting => Brushes.DarkOrange,
            PlcConnectionState.Faulted => Brushes.Firebrick,
            PlcConnectionState.Disconnected => Brushes.Gray,
            _ => throw new ArgumentOutOfRangeException(nameof(state)),
        };

        var brush = converter.Convert(state, typeof(Brush), null!, CultureInfo.InvariantCulture);

        Assert.Same(expected, brush);
        Assert.Same(
            Binding.DoNothing,
            converter.ConvertBack(brush, typeof(PlcConnectionState), null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public async Task ViewModel_ConnectWriteReadDisconnect_CompletesRoundTrip()
    {
        var client = new InMemoryPlcClient();
        await using var viewModel = new PlcMonitorViewModel(client, "Test PLC");

        await InvokeAsync(viewModel, "ConnectAsync");
        Assert.Equal(PlcConnectionState.Connected, viewModel.State);
        Assert.Equal("Connected.", viewModel.StatusMessage);

        viewModel.AddressText = "D100";
        viewModel.WordValuesText = "10, -20, 30";
        await InvokeAsync(viewModel, "WriteWordsAsync");
        viewModel.CountText = "3";
        await InvokeAsync(viewModel, "ReadWordsAsync");
        Assert.Equal("Read words: 10,-20,30", viewModel.StatusMessage);

        viewModel.AddressText = "M10";
        viewModel.BitValuesText = "1, false, true, 0";
        await InvokeAsync(viewModel, "WriteBitsAsync");
        viewModel.CountText = "4";
        await InvokeAsync(viewModel, "ReadBitsAsync");
        Assert.Equal("Read bits: 1,0,1,0", viewModel.StatusMessage);

        await InvokeAsync(viewModel, "DisconnectAsync");
        Assert.Equal(PlcConnectionState.Disconnected, viewModel.State);
        Assert.Equal("Disconnected.", viewModel.StatusMessage);
        Assert.Contains(viewModel.Logs, entry => entry.Operation == "WriteWords" && entry.IsSuccess);
        Assert.Contains(viewModel.Logs, entry => entry.Operation == "ReadBits" && entry.IsSuccess);
    }

    [Fact]
    public async Task ViewModel_ValidationFailures_AreLoggedWithoutCallingClient()
    {
        await using var viewModel = new PlcMonitorViewModel();

        viewModel.AddressText = "invalid";
        await InvokeAsync(viewModel, "ReadWordsAsync");
        Assert.Equal("Validate", viewModel.Logs[0].Operation);
        Assert.False(viewModel.Logs[0].IsSuccess);

        viewModel.AddressText = "D1";
        viewModel.CountText = "0";
        await InvokeAsync(viewModel, "ReadBitsAsync");
        Assert.Equal("Validate", viewModel.Logs[0].Operation);

        viewModel.BitValuesText = "1,maybe";
        await InvokeAsync(viewModel, "WriteBitsAsync");
        Assert.Contains("Invalid bit value", viewModel.StatusMessage);

        viewModel.WordValuesText = "1,not-a-number";
        await InvokeAsync(viewModel, "WriteWordsAsync");
        Assert.False(viewModel.Logs[0].IsSuccess);
    }

    [Fact]
    public async Task ViewModel_SetClient_ClearLog_AndPropertiesNotify()
    {
        var firstClient = new InMemoryPlcClient();
        await using var viewModel = new PlcMonitorViewModel(firstClient, "First");
        var changed = new List<string?>();
        viewModel.PropertyChanged += (_, args) => changed.Add(args.PropertyName);

        var secondClient = new InMemoryPlcClient();
        viewModel.SetClient(secondClient, "Second");
        viewModel.AddressText = "D200";
        viewModel.CountText = "2";
        viewModel.BitValuesText = "1";
        viewModel.WordValuesText = "2";
        viewModel.StatusMessage = "Updated";

        Assert.Equal("Second", viewModel.ChannelName);
        Assert.Contains(nameof(viewModel.ChannelName), changed);
        Assert.Throws<ArgumentNullException>(() => viewModel.SetClient(null!, "Invalid"));

        viewModel.AppendLog("External", "D1", "10", true, "OK");
        Assert.NotEmpty(viewModel.Logs);
        viewModel.ClearLogCommand.Execute(null);
        Assert.Empty(viewModel.Logs);
        Assert.Equal("Log cleared.", viewModel.StatusMessage);
    }

    [Fact]
    public async Task ViewModel_LogIsBoundedToFiveHundredEntries()
    {
        await using var viewModel = new PlcMonitorViewModel();

        for (var index = 0; index < 510; index++)
        {
            viewModel.AppendLog("Test", $"D{index}", index.ToString(CultureInfo.InvariantCulture), true, "OK");
        }

        Assert.Equal(500, viewModel.Logs.Count);
        Assert.Equal("D509", viewModel.Logs[0].Address);
        Assert.Equal("D10", viewModel.Logs[^1].Address);
    }

    [Fact]
    public async Task MonitorService_ForwardsOperationsAndDisposesViewModel()
    {
        var viewModel = new PlcMonitorViewModel();
        await using var service = new PlcMonitorService(viewModel);

        Assert.Same(viewModel, service.ViewModel);
        service.AppendLog("Audit", "D1", "42", true, "Recorded");
        Assert.Equal("Audit", viewModel.Logs[0].Operation);

        service.SetClient(new InMemoryPlcClient(), "Replacement");
        Assert.Equal("Replacement", viewModel.ChannelName);
        Assert.Throws<ArgumentNullException>(() => new PlcMonitorService(null!));
    }

    [Fact]
    public void OperationLogItem_StoresValues()
    {
        var time = new DateTime(2026, 7, 29, 12, 0, 0, DateTimeKind.Local);
        var item = new PlcOperationLogItem
        {
            Time = time,
            Operation = "Read",
            Address = "D1",
            Values = "7",
            IsSuccess = true,
            Message = "OK"
        };

        Assert.Equal(time, item.Time);
        Assert.Equal("Read", item.Operation);
        Assert.Equal("D1", item.Address);
        Assert.Equal("7", item.Values);
        Assert.True(item.IsSuccess);
        Assert.Equal("OK", item.Message);
    }

    private static async Task InvokeAsync(PlcMonitorViewModel viewModel, string methodName)
    {
        var method = typeof(PlcMonitorViewModel).GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(method);

        var task = Assert.IsAssignableFrom<Task>(method.Invoke(viewModel, null));
        await task;
    }
}
