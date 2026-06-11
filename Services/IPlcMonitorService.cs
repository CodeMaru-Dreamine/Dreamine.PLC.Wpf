using Dreamine.PLC.Abstractions.Clients;
using Dreamine.PLC.Wpf.ViewModels;

namespace Dreamine.PLC.Wpf.Services;

/// <summary>
/// Provides an application-facing service boundary for a PLC monitor view model.
/// </summary>
public interface IPlcMonitorService
{
    /// <summary>
    /// Gets the monitor view model exposed to the view.
    /// </summary>
    PlcMonitorViewModel ViewModel { get; }

    /// <summary>
    /// Replaces the monitored PLC client.
    /// </summary>
    /// <param name="client">The PLC client.</param>
    /// <param name="channelName">The channel display name.</param>
    void SetClient(IPlcClient client, string channelName);

    /// <summary>
    /// Adds a diagnostic log entry to the monitor.
    /// </summary>
    /// <param name="operation">The operation name.</param>
    /// <param name="address">The PLC address.</param>
    /// <param name="values">The operation values.</param>
    /// <param name="isSuccess">Whether the operation succeeded.</param>
    /// <param name="message">The diagnostic message.</param>
    void AppendLog(string operation, string address, string values, bool isSuccess, string message);
}
