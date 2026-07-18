namespace Dreamine.PLC.Wpf.Models;

/// <summary>
/// \if KO
/// <para>PLC 모니터 작업 로그 항목을 나타냅니다.</para>
/// \endif
/// \if EN
/// <para>Represents a PLC monitor operation-log item.</para>
/// \endif
/// </summary>
public sealed class PlcOperationLogItem
{
    /// <summary>
    /// \if KO
    /// <para>작업 시각을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the operation timestamp.</para>
    /// \endif
    /// </summary>
    public DateTime Time { get; set; } = DateTime.Now;

    /// <summary>
    /// \if KO
    /// <para>작업 이름을 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the operation name.</para>
    /// \endif
    /// </summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// \if KO
    /// <para>PLC 주소 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the PLC address text.</para>
    /// \endif
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// \if KO
    /// <para>작업 값 텍스트를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the operation-values text.</para>
    /// \endif
    /// </summary>
    public string Values { get; set; } = string.Empty;

    /// <summary>
    /// \if KO
    /// <para>작업 성공 여부를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets whether the operation succeeded.</para>
    /// \endif
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// \if KO
    /// <para>결과 메시지를 가져오거나 설정합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets or sets the result message.</para>
    /// \endif
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
