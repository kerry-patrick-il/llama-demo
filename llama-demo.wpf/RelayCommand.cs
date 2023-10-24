using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace llama_demo.wpf
{
    public class RelayCommand: ICommand
    {
        private readonly Action _implementation;

        public RelayCommand(Action implementation)
        {
            _implementation = implementation;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _implementation?.Invoke();
        }

        public event EventHandler? CanExecuteChanged;
    }
}
