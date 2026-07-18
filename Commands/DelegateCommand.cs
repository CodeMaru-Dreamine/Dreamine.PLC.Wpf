using System.Windows.Input;

namespace Dreamine.PLC.Wpf.Commands;

/// <summary>
/// \if KO
/// <para>명령 매개변수를 전달받는 <see cref="T:System.Windows.Input.ICommand" /> 구현을 제공합니다.</para>
/// \endif
/// \if EN
/// <para>Provides an <see cref="T:System.Windows.Input.ICommand" /> implementation that accepts the command parameter.</para>
/// \endif
/// </summary>
public sealed class DelegateCommand : ICommand
{
    /// <summary>
    /// \if KO
    /// <para>execute 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the execute value.</para>
    /// \endif
    /// </summary>
    private readonly Action<object?> _execute;
    /// <summary>
    /// \if KO
    /// <para>can Execute 값을 보관합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Stores the can execute value.</para>
    /// \endif
    /// </summary>
    private readonly Predicate<object?>? _canExecute;

    /// <summary>
    /// \if KO
    /// <para>실행 및 선택적 실행 가능 조건을 사용해 <see cref="T:Dreamine.PLC.Wpf.Commands.DelegateCommand" /> 클래스의 새 인스턴스를 초기화합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Initializes a new instance of <see cref="T:Dreamine.PLC.Wpf.Commands.DelegateCommand" /> with execution and optional can-execute delegates.</para>
    /// \endif
    /// </summary>
    /// <param name="execute">
    /// \if KO
    /// <para>명령 실행 시 호출할 대리자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The delegate invoked when the command executes.</para>
    /// \endif
    /// </param>
    /// <param name="canExecute">
    /// \if KO
    /// <para>실행 가능 여부를 평가할 선택적 대리자입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The optional delegate that evaluates whether execution is allowed.</para>
    /// \endif
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// \if KO
    /// <para><paramref name="execute"/>가 <see langword="null"/>일 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Thrown when <paramref name="execute"/> is <see langword="null"/>.</para>
    /// \endif
    /// </exception>
    public DelegateCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    /// <summary>
    /// \if KO
    /// <para>명령의 실행 가능 상태가 변경되었음을 알릴 때 발생합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Occurs when the command's ability to execute may have changed.</para>
    /// \endif
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// \if KO
    /// <para>현재 매개변수로 명령을 실행할 수 있는지 확인합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Determines whether the command can execute with the current parameter.</para>
    /// \endif
    /// </summary>
    /// <param name="parameter">
    /// \if KO
    /// <para>실행 가능 조건에 전달할 명령 매개변수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The command parameter passed to the can-execute predicate.</para>
    /// \endif
    /// </param>
    /// <returns>
    /// \if KO
    /// <para>조건이 없거나 조건이 허용하면 <see langword="true"/>입니다.</para>
    /// \endif
    /// \if EN
    /// <para><see langword="true"/> when no predicate exists or the predicate permits execution.</para>
    /// \endif
    /// </returns>
    public bool CanExecute(object? parameter)
    {
        return _canExecute?.Invoke(parameter) ?? true;
    }

    /// <summary>
    /// \if KO
    /// <para>실행 가능 조건이 허용하는 경우 명령 대리자를 호출합니다.</para>
    /// \endif
    /// \if EN
    /// <para>Invokes the command delegate when the can-execute condition permits it.</para>
    /// \endif
    /// </summary>
    /// <param name="parameter">
    /// \if KO
    /// <para>실행 가능 조건과 실행 대리자에 전달할 매개변수입니다.</para>
    /// \endif
    /// \if EN
    /// <para>The parameter passed to the can-execute predicate and execution delegate.</para>
    /// \endif
    /// </param>
    public void Execute(object? parameter)
    {
        if (CanExecute(parameter))
        {
            _execute(parameter);
        }
    }

    /// <summary>
    /// \if KO
    /// <para><see cref="E:Dreamine.PLC.Wpf.Commands.DelegateCommand.CanExecuteChanged" /> 이벤트를 발생시킵니다.</para>
    /// \endif
    /// \if EN
    /// <para>Raises the <see cref="E:Dreamine.PLC.Wpf.Commands.DelegateCommand.CanExecuteChanged" /> event.</para>
    /// \endif
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
