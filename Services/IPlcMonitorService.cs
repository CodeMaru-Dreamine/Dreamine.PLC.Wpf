using Dreamine.PLC.Abstractions.Clients;
using Dreamine.PLC.Wpf.ViewModels;

namespace Dreamine.PLC.Wpf.Services;

/// <summary>
/// \if KO
/// <para>PLC 모니터 ViewModel을 애플리케이션에 노출하는 서비스 계약을 정의합니다.</para>
/// \endif
/// \if EN
/// <para>Defines an application-facing service contract for a PLC monitor view model.</para>
/// \endif
/// </summary>
public interface IPlcMonitorService
{
    /// <summary>
    /// \if KO
    /// <para>뷰에 노출되는 모니터 ViewModel을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the monitor view model exposed to the view.</para>
    /// \endif
    /// </summary>
    PlcMonitorViewModel ViewModel { get; }

    /// <summary>
    /// \if KO
    /// <para>모니터링할 PLC 클라이언트와 채널 이름을 교체합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Replaces the monitored PLC client and channel name.</para>
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
    /// <para><paramref name="client"/>가 <see langword="null"/>이면 구현체가 발생시킬 수 있습니다.</para>
    /// \endif
    /// \if EN
    /// <para>May be thrown by an implementation when <paramref name="client"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    void SetClient(IPlcClient client, string channelName);

    /// <summary>
    /// \if KO
    /// <para>PLC 모니터에 진단 로그 항목을 추가합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Adds a diagnostic log entry to the PLC monitor.</para>
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
    void AppendLog(string operation, string address, string values, bool isSuccess, string message);
}
