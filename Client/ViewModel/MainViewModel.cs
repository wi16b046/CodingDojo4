using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.ObjectModel;
using System;
using System.Windows.Input;

namespace Client.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        ClientObj client;
        private bool isConnected = false;

        public string ChatName { get; set; }
        public string Message { get; set; }
        public ObservableCollection<string> ReceivedMessages { get; set; }
        public RelayCommand ConnectBtnClicked { get; set; }
        public RelayCommand SendBtnClicked { get; set; }
        


        public MainViewModel()
        {
            Message = "";
            ReceivedMessages = new ObservableCollection<string>();
            ConnectBtnClicked = new RelayCommand(ExecuteConnect, CanExecuteConnect);
            SendBtnClicked = new RelayCommand(ExecuteSend, CanExecuteSend);
        }

        private void ExecuteSend()
        {
            
            client.Send(ChatName + ": " + Message);
            if (Message != "@quit")
                ReceivedMessages.Add("ME: " + Message);
            else
            {
                Disconnect();
                ReceivedMessages.Add("You disconnected");
            }
                
        }

        private bool CanExecuteSend()
        {
            return (isConnected && Message.Length >= 1);
        }

        private void ExecuteConnect()
        {
            //Client connects here 
            isConnected = true;
            client = new ClientObj("127.0.0.1", 10100, new Action<string>(ReceivedNewMessage), Disconnect);
        }

        private bool CanExecuteConnect()
        {
            return !isConnected;
        }

        private void ReceivedNewMessage(string message)
        {
            App.Current.Dispatcher.Invoke(()=>
            {
                ReceivedMessages.Add(message);
            });
        }

        private void Disconnect()
        {
            isConnected = false;
            
            CommandManager.InvalidateRequerySuggested();
        }

        
    }
}