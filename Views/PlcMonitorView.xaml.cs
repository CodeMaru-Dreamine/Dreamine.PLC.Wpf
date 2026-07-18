using System.Windows.Controls;

namespace Dreamine.PLC.Wpf.Views;

/// <summary>
/// \if KO
/// <para><c>PlcMonitorView</c>의 WPF 상호 작용 논리를 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides WPF interaction logic for <c>PlcMonitorView</c>.</para>
/// \endif
/// </summary>
public partial class PlcMonitorView : UserControl
{
    /// <summary>
    /// \if KO
    /// <para>XAML 구성 요소를 로드해 <see cref="T:Dreamine.PLC.Wpf.Views.PlcMonitorView" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.PLC.Wpf.Views.PlcMonitorView" /> by loading its XAML components.</para>
    /// \endif
    /// </summary>
    public PlcMonitorView()
    {
        InitializeComponent();
    }
}
