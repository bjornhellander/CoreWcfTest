using System;
using System.ServiceModel;
using System.Windows;

namespace WcfTest.Wcf.Client
{
    public partial class App : Application
    {
        private async void HandleApplicationStartup(object sender, StartupEventArgs eventArgs)
        {
            try
            {
                var chatProxy = new ChatServiceProxy();
                await chatProxy.ConnectAsync();

                var timeProxy = new TimeServiceProxy();
                await timeProxy.ConnectAsync();

                var mainViewModel = new MainViewModel(chatProxy, timeProxy);

                var mainWindow = new MainWindow();
                mainWindow.DataContext = mainViewModel;
                mainWindow.Show();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine($"Caught exception: {e}");
            }
        }
    }
}
