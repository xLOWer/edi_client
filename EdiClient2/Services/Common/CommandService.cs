using System;
using System.Windows.Input;

namespace EdiClient.Services
{
    public class Command : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Command(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public bool CanExecute(object parameter) {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter) {
            this.execute(parameter);
        }

    }


    public class Command<T> : ICommand
    {
        private Action<T> execute1;
        private Func<T, bool> canExecute1;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Command(Action<T> execute, Func<T, bool> canExecute = null)
        {
            this.execute1 = execute;
            this.canExecute1 = canExecute;
        }

        public bool CanExecute(object parameter) {
            return this.canExecute1 == null || this.canExecute1((T)parameter);
        }

        public void Execute(object parameter) {
            this.execute1((T)parameter);
        }
    }


}

