using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModel
{
    class ClientObj
    {
        
        Socket clientSocket;

        byte[] buffer = new byte[1024];

        //Delegate for informing about receiving messages
        Action<string> MessageInformer;
        //Delegate for informing about aborting the connection
        Action AbortInformer;

        public ClientObj(string ip, int port, Action<string> messageInformer, Action abortInformer)
        {
            MessageInformer = messageInformer;
            AbortInformer = abortInformer;
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(ip), port);
            clientSocket = client.Client;
            StartReceiving();
        }


        public void Send(string message)
        {
            if (clientSocket != null)
                clientSocket.Send(Encoding.UTF8.GetBytes(message));
        }

        private void StartReceiving()
        {
            Task.Factory.StartNew(Receive);
        }

        private void Receive()
        {
            string message;

            do
            {
                int length = clientSocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, length);

                MessageInformer(message);
            } while (message != "@quit");
            Close();
        }

        private void Close()
        {
            clientSocket.Close();
            AbortInformer();
        }
    }
}