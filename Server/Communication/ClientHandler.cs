using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Communication
{
    class ClientHandler
    {
        Action<string, Socket> Action;
        byte[] buffer = new byte[512];
        Thread receiveThreadClient;
        

        public string Name { get; set; }
        public Socket ClientSocket { get; private set; }

        public ClientHandler(Socket socket, Action<string, Socket> action)
        {
            ClientSocket = socket;
            Action = action;
            receiveThreadClient = new Thread(Receive);
            receiveThreadClient.Start();
        }

        private void Receive()
        {
            string message;
            do
            {
                int length = ClientSocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, length);

                if(Name==null && message.Contains(":"))
                {
                    Name = message.Split(':')[0];
                }
                Action(message, ClientSocket);

            } while (!message.Equals("@quit"));
            Close();
        }

        public void Send(string message)
        {
            ClientSocket.Send(Encoding.UTF8.GetBytes(message));
        }

        public void Close()
        {
            Send("@quit");
            ClientSocket.Close();
            receiveThreadClient.Abort();
        }
    }
}
