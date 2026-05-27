namespace Dreamine.PLC.Wpf.Models;

/// <summary>
/// \brief Represents a PLC monitor operation log item.
/// </summary>
public sealed class PlcOperationLogItem
{
    /// <summary>
    /// \brief Gets or sets the operation timestamp.
    /// </summary>
    public DateTime Time { get; set; } = DateTime.Now;

    /// <summary>
    /// \brief Gets or sets the operation name.
    /// </summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// \brief Gets or sets the PLC address text.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// \brief Gets or sets the operation values.
    /// </summary>
    public string Values { get; set; } = string.Empty;

    /// <summary>
    /// \brief Gets or sets whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// \brief Gets or sets the result message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
