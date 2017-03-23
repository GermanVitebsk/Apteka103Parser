using System;
using System.Windows.Input;

namespace Apteka103Parser.Helpers
{
    public class DelegateCommand : ICommand
    {
        private readonly Func<object, bool> canExecute;
        private readonly Action<object> execute;

        // Два конструктора
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public DelegateCommand(Action<object> execute)
        {
            this.execute = execute;
            canExecute = AlwaysCanExecute;
        }

        // Событие, необходимое для ICommand
        public event EventHandler CanExecuteChanged;

        // Методы, необходимые для ICommand
        public void Execute(object param)
        {
            execute(param);
        }

        public bool CanExecute(object param)
        {
            return canExecute(param);
        }

        // Метод, необходимый для IDelegateCommand
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        // Метод CanExecute по умолчанию
        private bool AlwaysCanExecute(object param)
        {
            return true;
        }
    }
}