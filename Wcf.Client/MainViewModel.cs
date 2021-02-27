using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WcfTest.Interface;

namespace WcfTest.Wcf.Client
{
    internal class MainViewModel : ViewModelBase
    {
        private readonly IChatService chatService;

        private ObservableCollection<string> notifications;
        private ICommand sendCommand;
        private string text;
        private string time;

        [Obsolete("Design-time constructor")]
        public MainViewModel()
        {
            Setup();

            notifications.Add("Text #1");
            notifications.Add("Text #2");
            notifications.Add("Text #3");

            text = "My text";
            time = "?";
        }

        public MainViewModel(ChatServiceProxy chatService, TimeServiceProxy timeService)
        {
            this.chatService = chatService;

            Setup();

            text = "";

            chatService.MessagePosted += HandleMessagePosted;
            timeService.TimeUpdated += HandleTimeUpdated;
        }

        private void Setup()
        {
            notifications = new ObservableCollection<string>();
            sendCommand = new RelayCommand(DoPostAsync, CanPost);
        }

        public ObservableCollection<string> Notifications => notifications;

        public string Text
        {
            set => SetProperty(ref text, value);
            get => text;
        }

        public ICommand SendCommand => sendCommand;

        public string Time
        {
            set => SetProperty(ref time, value);
            get => time;
        }

        private bool CanPost()
        {
            return !string.IsNullOrEmpty(text);
        }

        private async Task DoPostAsync()
        {
            try
            {
                await chatService.PostMessageAsync(Text);
                Text = "";
            }
            catch (CommunicationException e)
            {
                Console.WriteLine($"Caught exception: {e}");
            }
        }

        private void HandleMessagePosted(string message)
        {
            notifications.Add(message);
        }

        private void HandleTimeUpdated(DateTime time)
        {
            Time = time.ToString();
        }
    }
}
