using System.Windows;
using System.Windows.Controls;
using Dreamine.PLC.Wpf.ViewModels;

namespace Dreamine.PLC.Wpf.Views;

/// <summary>
/// \brief Interaction logic for PlcMonitorView.
/// </summary>
public partial class PlcMonitorView : UserControl
{
    /// <summary>
    /// \brief Initializes a new instance of the <see cref="PlcMonitorView"/> class.
    /// </summary>
    public PlcMonitorView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is null)
        {
            DataContext = new PlcMonitorViewModel();
        }
    }
}
