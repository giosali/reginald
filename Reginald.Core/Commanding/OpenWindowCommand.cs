using System;
using System.Windows.Input;

namespace Reginald.Commanding
{
    public class OpenWindowCommand : ICommand
    {
        public Action<object> ExecuteMethod { get; set; }

        public Func<object, bool> CanExecuteMethod { get; set; }

        public OpenWindowCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            ExecuteMethod = executeMethod;
            CanExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteMethod(parameter);
        }
    }
}
