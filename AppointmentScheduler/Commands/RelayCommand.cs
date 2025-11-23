using System;
using System.Windows.Input;

namespace AppointmentScheduler.Commands
{
    /// <summary>
    /// A generic command implementation used in MVVM to bind UI actions
    /// (such as button clicks) to ViewModel methods.
    /// </summary>
    /// <remarks>
    /// RelayCommand allows you to pass an execute delegate and an optional
    /// canExecute delegate. WPF automatically reevaluates CanExecute via
    /// CommandManager.RequerySuggested.
    /// </remarks>
    public class RelayCommand : ICommand
    {
        //Delegate to execute when the command is invoked
        private readonly Action<object> _execute;

        //Delegate to determine if the command can execute
        private readonly Predicate<object> _canExecute;

        //Creates a new RelayCommand with the specified execute and canExecute delegates
        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="execute">Action executed when the command runs.</param>
        /// <param name="canExecute">Determines the command availability</param>
        /// <exception cref="ArgumentNullException"></exception>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
