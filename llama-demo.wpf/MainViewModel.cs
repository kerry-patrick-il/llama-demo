using LLama.Common;
using LLama;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace llama_demo.wpf
{
    public class MainViewModel : ObservableViewModel
    {
        private LLamaWeights? _model;
        private LLamaContext? _context;
        private ChatSession? _session;

        #region initialization
        public MainViewModel()
        {
            _chat = new ObservableCollection<string>();
            LoadPromptCommand = new RelayCommand(LoadPrompt);
            SendInputCommand = new RelayCommand(SendInput);
            ClearChatCommand = new RelayCommand(ClearOldChat);
        }

        public static MainViewModel Create()
        {
            return new MainViewModel();
        }
        #endregion

        public ICommand LoadPromptCommand { get; set; }

        public void LoadPrompt()
        {
            if (string.IsNullOrEmpty(SelectedModel))
            {
                MessageBox.Show("Please select a model", "No Model Selected");
            }
            else if (string.IsNullOrEmpty(Prompt))
            {
                MessageBox.Show("Please enter a prompt", "Prompt Missing");
            }
            else
            {
                StartChat();
            }
        }

        public ICommand SendInputCommand { get; set; }

        public void SendInput()
        {
            if (string.IsNullOrEmpty(Input))
            {
                MessageBox.Show("Please enter some input", "Missing Input");
            }
            else
            {
                Chat.Add(Input);
                SendChatPrompt(Input);
                Input = string.Empty;
            }
        }

        public ICommand ClearChatCommand { get; set; }
        private void ClearOldChat()
        {
            Chat.Clear();

            _model?.Dispose();
            _context?.Dispose();
        }

        private void StartChat()
        {
            ClearOldChat();
            Chat.Add(Prompt);

            var parameters = new ModelParams(SelectedModel!)
            {
                ContextSize = 2048
            };
            _model = LLamaWeights.LoadFromFile(parameters);
            // Initialize a chat session
            _context = _model.CreateContext(parameters);
            _session = new ChatSession(new InteractiveExecutor(_context));

            var prompt = Prompt;
            SendChatPrompt(prompt);
        }

        private void SendChatPrompt(string prompt)
        {
            var response = new StringBuilder();
            foreach (var text in _session!.Chat(prompt,
                         new InferenceParams() { Temperature = 0.1f, AntiPrompts = new List<string> { "User:" } }))
            {
                response.Append(text);
            }

            Chat.Add(response.ToString());
        }

        #region properties
        private string _prompt = "";
        public string Prompt
        {
            get => _prompt;
            set => SetField(ref _prompt, value);
        }

        private string _input = "";
        public string Input
        {
            get => _input;
            set => SetField(ref _input, value);
        }

        private ObservableCollection<string> _chat;
        public ObservableCollection<string> Chat
        {
            get => _chat;
            set => SetField(ref _chat, value);
        }

        private string? _selectedModel;
        public string? SelectedModel
        {
            get => _selectedModel;
            set => SetField(ref _selectedModel, value);
        }

        public Dictionary<string, string> Models { get; } = new()
        {
            { "C:\\git\\edu\\ai\\models\\codellama-7b-instruct.Q4_K_M.gguf", "Code Llama" },
            { "C:\\git\\edu\\ai\\models\\llama-2-7b-guanaco-qlora.Q6_K.gguf", "Llama 2" }
        };

        public Dictionary<string, string> PromptStarts { get; } = new()
        {
            { "Simple Conversation", """
                                     Transcript of a dialog, where the User interacts with an Assistant named Bob. 
                                     Bob is helpful, kind, honest, good at writing, 
                                     and never fails to answer the User's requests immediately and with precision.
                                     
                                     User: Hello, Bob.
                                     Bob: Hello. How may I help you today?
                                     User: Please tell me the largest city in Europe.
                                     Bob: Sure. The largest city in Europe is Moscow, the capital of Russia.
                                     User:
                                     """},
            { "Code Generation", """
                                 You are a polite and helpful pair programming assistant.
                                 You MUST reply in a polite and helpful manner.
                                 When asked for your name, you MUST reply that your name is 'LocalLLM'.
                                 You MUST use Markdown formatting in your replies when the content is a block of code.
                                 You MUST include the programming language name in any Markdown code blocks.
                                 Your code responses MUST be using C# language syntax.

                                 User:
                                 """ }
        };

        public int SelectedPromptStartIndex
        {
            get => -1;
            set
            {
                if (value > -1 && PromptStarts.Count > value)
                {
                    var prompt = PromptStarts.ToList()[value].Value;
                    Prompt = prompt;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

    }
}
