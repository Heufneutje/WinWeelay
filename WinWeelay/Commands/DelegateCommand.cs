using System;
using System.Windows.Input;

namespace WinWeelay
{
    /// <summary>
    /// Command that can by used by passing delegates.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;

        /// <summary>
        /// Event which fires if the state of the command should be rechecked.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Create a command that is always active.
        /// </summary>
        /// <param name="execute">Method to run when executed.</param>
        public DelegateCommand(Action<object> execute) : this(execute, null)
        {
        }

        /// <summary>
        /// Create a command that is only active when a condition is met.
        /// </summary>
        /// <param name="execute">Method to run when executed.</param>
        /// <param name="canExecute">Method to check whether the command should be active.</param>
        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Method to check whether the command should be active.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;

            return _canExecute(parameter);
        }

        /// <summary>
        /// Method to run when executed.
        /// </summary>
        /// <param name="parameter">Command parameter.</param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Notify UI that the active state of the command should change.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
