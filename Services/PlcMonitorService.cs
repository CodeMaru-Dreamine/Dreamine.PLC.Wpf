using Dreamine.PLC.Abstractions.Clients;
using Dreamine.PLC.Wpf.ViewModels;

namespace Dreamine.PLC.Wpf.Services;

/// <summary>
/// Default PLC monitor service implementation.
/// </summary>
public sealed class PlcMonitorService : IPlcMonitorService, IAsyncDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlcMonitorService"/> class.
    /// </summary>
    public PlcMonitorService()
        : this(new PlcMonitorViewModel())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlcMonitorService"/> class.
    /// </summary>
    /// <param name="viewModel">The monitor view model.</param>
    public PlcMonitorService(PlcMonitorViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }

    /// <inheritdoc />
    public PlcMonitorViewModel ViewModel { get; }

    /// <inheritdoc />
    public void SetClient(IPlcClient client, string channelName)
    {
        ViewModel.SetClient(client, channelName);
    }

    /// <inheritdoc />
    public void AppendLog(string operation, string address, string values, bool isSuccess, string message)
    {
        ViewModel.AppendLog(operation, address, values, isSuccess, message);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return ViewModel.DisposeAsync();
    }
}
