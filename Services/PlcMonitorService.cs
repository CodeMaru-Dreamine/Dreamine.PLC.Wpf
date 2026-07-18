using Dreamine.PLC.Abstractions.Clients;
using Dreamine.PLC.Wpf.ViewModels;

namespace Dreamine.PLC.Wpf.Services;

/// <summary>
/// \if KO
/// <para>기본 PLC 모니터 서비스 구현을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides the default PLC monitor service implementation.</para>
/// \endif
/// </summary>
public sealed class PlcMonitorService : IPlcMonitorService, IAsyncDisposable
{
    /// <summary>
    /// \if KO
    /// <para>새 ViewModel을 사용해 <see cref="T:Dreamine.PLC.Wpf.Services.PlcMonitorService" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.PLC.Wpf.Services.PlcMonitorService" /> using a new view model.</para>
    /// \endif
    /// </summary>
    public PlcMonitorService()
        : this(new PlcMonitorViewModel())
    {
    }

    /// <summary>
    /// \if KO
    /// <para>지정한 ViewModel을 사용해 <see cref="T:Dreamine.PLC.Wpf.Services.PlcMonitorService" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.PLC.Wpf.Services.PlcMonitorService" /> using the specified view model.</para>
    /// \endif
    /// </summary>
    /// <param name="viewModel">
    /// \if KO
    /// <para>서비스에서 노출할 모니터 ViewModel입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The monitor view model exposed by the service.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="viewModel"/>이 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="viewModel"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public PlcMonitorService(PlcMonitorViewModel viewModel)
    {
        ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
    }

    /// <summary>
    /// \if KO
    /// <para>뷰에 노출되는 PLC 모니터 ViewModel을 가져옵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Gets the PLC monitor view model exposed to the view.</para>
    /// \endif
    /// </summary>
    public PlcMonitorViewModel ViewModel { get; }

    /// <summary>
    /// \if KO
    /// <para>ViewModel이 모니터링할 PLC 클라이언트를 교체합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Replaces the PLC client monitored by the view model.</para>
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
    /// <para>표시할 채널 이름입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The channel name to display.</para>
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
    public void SetClient(IPlcClient client, string channelName)
    {
        ViewModel.SetClient(client, channelName);
    }

    /// <summary>
    /// \if KO
    /// <para>ViewModel의 작업 로그에 진단 항목을 추가합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Adds a diagnostic entry to the view model's operation log.</para>
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
    /// <para>성공 여부입니다.</para>
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
        ViewModel.AppendLog(operation, address, values, isSuccess, message);
    }

    /// <summary>
    /// \if KO
    /// <para>소유한 ViewModel을 비동기로 정리합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Asynchronously disposes the owned view model.</para>
    /// \endif
    /// </summary>
    /// <returns>
    /// \if KO
    /// <para>ViewModel의 비동기 정리 작업입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The view model's asynchronous disposal operation.</para>
    /// \endif
    /// </returns>
    public ValueTask DisposeAsync()
    {
        return ViewModel.DisposeAsync();
    }
}
