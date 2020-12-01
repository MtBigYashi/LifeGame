using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LifeGameViewModel
{
    public class RelayCommand : ICommand
    {
        private Predicate<object> _canExecute = null;
        private Action<object> _execute = null;

        public RelayCommand(Action<object> execute)
        {
            this._execute = execute;
        }

        public RelayCommand(Predicate<object> canExecute, Action<object> execute)
        {
            this._canExecute = canExecute;
            this._execute = execute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => this._canExecute == null || this._canExecute.Invoke(parameter);

        public void Execute(object parameter) => this._execute.Invoke(parameter);
    }
}
