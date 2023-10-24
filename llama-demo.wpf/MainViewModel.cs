using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace llama_demo.wpf
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _prompt = "";
        private string _input;

        public MainViewModel()
        {
            SetPromptCommand = new RelayCommand(SetPrompt);
        }

        public ICommand SetPromptCommand { get; set; }

        public void SetPrompt()
        {
        }

        public string Prompt
        {
            get => _prompt;
            set => SetField(ref _prompt, value);
        }

        public string Input
        {
            get => _input;
            set => SetField(ref _input, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
