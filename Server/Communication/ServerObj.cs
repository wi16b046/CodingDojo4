using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Communication
{
    class ServerObj
    {
        Socket serverSocket;
        List<ClientHandler> clients = new List<ClientHandler>();
        Action<string> GUIInformer;
        Thread acceptingThread;


        public ServerObj(int port, string ip, Action<string> guiInformer)
        {
            GUIInformer = guiInformer;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip),port));
            serverSocket.Listen(10);
        }

        public void StartAccepting()
        {
            acceptingThread = new Thread(new ThreadStart(Accept));
            acceptingThread.IsBackground = true;
            acceptingThread.Start();
        }

        private void Accept()
        {
            while (acceptingThread.IsAlive)
            {
                clients.Add(new ClientHandler(serverSocket.Accept(), new Action<string, Socket>(NewMassage)));
            }
        }

        private void NewMassage(string message, Socket senderSocket)
        {
            GUIInformer(message);

            foreach (var item in clients)
            {
                if (item.ClientSocket != senderSocket)
                    item.Send(message);
            }
        }

        public void StopAccepting()
        {
            serverSocket.Close();
            acceptingThread.Abort();
            foreach(var item in clients)
            {
                item.Close();
            }
            clients.Clear();
        }

        public void dropUser(string name)
        {
            foreach (var item in clients)
            {
                if (item.Name.Equals(name))
                {
                    item.Close();
                    clients.Remove(item);
                    break;
                }
                    

            }
        }
    }
}
