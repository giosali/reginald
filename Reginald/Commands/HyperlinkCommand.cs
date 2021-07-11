using System;
using System.Windows.Input;

namespace Reginald.Commands
{
    public class HyperlinkCommand : ICommand
    {
        Action<object> executeMethod;
        Func<object, bool> canExecuteMethod;

        public HyperlinkCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            executeMethod(parameter);
        }
    }
}
