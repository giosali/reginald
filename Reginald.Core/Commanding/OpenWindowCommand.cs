namespace Reginald.Commanding
{
    using System;
    using System.Windows.Input;

    public class OpenWindowCommand : ICommand
    {
        public OpenWindowCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            ExecuteMethod = executeMethod;
            CanExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add { } remove { }
        }

        public Action<object> ExecuteMethod { get; set; }

        public Func<object, bool> CanExecuteMethod { get; set; }

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
