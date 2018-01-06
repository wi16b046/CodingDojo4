using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Server.Communication;
using System.Collections.ObjectModel;
using System;

namespace Server.ViewModel
{
    
    public class MainViewModel : ViewModelBase
    {
        ServerObj server;
        const int port = 10100;
        const string ip = "127.0.0.1";
        bool isConnected = false;
        
        //for ListBoxes
        public ObservableCollection<string> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        //for Buttons
        public RelayCommand StartBtnClicked { get; set; }
        public RelayCommand StopBtnClicked { get; set; }
        public RelayCommand DropBtnClicked { get; set; }
        public RelayCommand SaveToLogBtnClicked { get; set; }

        //for dropping a user
        public string User { get; set; }

        public MainViewModel()
        {
            //Instanziieren der Listen
            Users = new ObservableCollection<string>();
            Messages = new ObservableCollection<string>();

            StartBtnClicked = new RelayCommand(ExecuteStart, () =>
            {
                return !isConnected;
            });

            StopBtnClicked = new RelayCommand(ExecuteStop, () =>
            {
                return isConnected;
            });

            DropBtnClicked = new RelayCommand(ExecuteDrop, () =>
            {
                return (User != null);
            });

            SaveToLogBtnClicked = new RelayCommand(ExecuteSaveToLog, () =>
            {
                return (Messages.Count >= 1);
            });
        }

        private void ExecuteSaveToLog()
        {
            //SAve to log file here
        }

        private void ExecuteDrop()
        {
            server.dropUser(User);
            Users.Remove(User);
        }

        private void ExecuteStop()
        {
            server.StopAccepting();
            isConnected = false;
        }

        private void ExecuteStart()
        {
            server = new ServerObj(port, ip, NewMessage);
            server.StartAccepting();
            isConnected = true;
        }

        private void NewMessage(string message)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                string name = message.Split(':')[0];

                if (!Users.Contains(name))
                {
                    Users.Add(name);
                }

                Messages.Add(message);
                RaisePropertyChanged();
            });
        }
    }
}