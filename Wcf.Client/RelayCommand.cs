using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WcfTest.Wcf.Client
{
    public class RelayCommand : ICommand
    {
        private Func<Task> execute;
        private Func< bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute();
        }

        public void Execute(object parameter)
        {
            this.execute();
        }
    }
}
