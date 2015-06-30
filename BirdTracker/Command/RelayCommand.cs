using System;
using System.Diagnostics;
using System.Windows.Input;

namespace BirdTracker.Command
{
    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Action _execute;
        private readonly Action<object> _executeObj;
        private readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommand(Action execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _executeObj = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            var canExecuteChanged = CanExecuteChanged;
            if (canExecuteChanged == null) return;
            canExecuteChanged(this, EventArgs.Empty);
        }

        public void Execute(object o)
        {
            if (_execute != null)
            {
                _execute();
            }
            else
            {
                _executeObj(o);
            }
        }

        #endregion // ICommand Members
    }
}
