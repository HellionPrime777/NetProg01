using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DesktopClient.Commands
{
    public class BaceCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly Func<object, bool> _canExecute;
        public BaceCommand(Action<object> action)
        {
            _canExecute = new Func<object, bool>(x => true);
            _action = action;
        }
        public BaceCommand(Func<object, bool> canExecute, Action<object> action)
        {
            _canExecute = canExecute;
            _action = action;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? false;
        }
        public void Execute(object parameter)
        {
            _action.Invoke(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
