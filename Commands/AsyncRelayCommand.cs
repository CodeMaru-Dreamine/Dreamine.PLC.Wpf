using System.Windows.Input;

namespace Dreamine.PLC.Wpf.Commands;

/// <summary>
/// Provides an asynchronous ICommand implementation.
/// </summary>
public sealed class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;
    private Exception? _lastException;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
    /// </summary>
    /// <param name="execute">The asynchronous execute delegate.</param>
    /// <param name="canExecute">The optional can-execute delegate.</param>
    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Occurs when the asynchronous delegate throws an exception.
    /// </summary>
    public event EventHandler<Exception>? ExecutionFailed;

    /// <summary>
    /// Gets the last exception thrown by the asynchronous delegate.
    /// </summary>
    public Exception? LastException => _lastException;

    /// <inheritdoc />
    public bool CanExecute(object? parameter)
    {
        return !_isExecuting && (_canExecute?.Invoke() ?? true);
    }

    /// <inheritdoc />
    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }

        try
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            await _execute().ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            _lastException = ex;
            ExecutionFailed?.Invoke(this, ex);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    /// <summary>
    /// Raises the CanExecuteChanged event.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
