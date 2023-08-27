using Godot;
using System;
using System.IO;
using System.Net.Sockets;


namespace FFA.Empty.Empty.Network.Client
{
    public class BaseClient
    {
        public BaseClient() { }
        private bool connected;
        private NetworkStream stream;

        private System.Threading.Mutex sendMustex = new System.Threading.Mutex();

        //Events
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public delegate void DataRecieved(object sender, byte[] data, NetworkStream stream);
        public event DataRecieved DataRecievedEvent = delegate { };

        public delegate void ClientConnected(object sender, NetworkStream stream);
        public event ClientConnected ClientConnectedEvent = delegate { };

        public delegate void ClientDisconnected(object sender);
        public event ClientDisconnected ClientDisconnectedEvent = delegate { };
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Events
        public void ConnectToServer(string host, int port)
        {
            if (connected) return;

            try
            {
                TcpClient client = new TcpClient();
                client.Connect(host, port);
                stream = client.GetStream();
                connected = true;
                new System.Threading.Thread(InputRecievingThread).Start();
            }
            catch (IOException) { }
        }//Connexion method

        public void Disconnect()
        {
            if (!connected) return;

            this.stream.Close();
        }

        private void InputRecievingThread()
        {
            ClientConnectedEvent(this, stream);
            while (connected)
            {

                byte[] buffer = new byte[8_192];
                //try
                {
                    stream.Read(buffer, 0, buffer.Length);
                    DataRecievedEvent(this, buffer, stream);
                }
                /*catch (Exception e)
                {
                    GD.Print("[BaseClient] Disconnecting, Exeption caught : " + e);
                    connected = false;
                }*/

            }
            ClientDisconnectedEvent(this);
        }//Listening thread

        public void SendDataToServer(byte[] data)
        {
            if (!connected) return;
            if (sendMustex.WaitOne(100))
            {
                if (data.Length > 8_192)
                {
                    //Split message
                    GD.Print("[BaseClient] Err : Message too long : " + data.Length);
                }
                else
                {
                    stream.Write(data, 0, data.Length);
                }
                sendMustex.ReleaseMutex();
            }
            else { GD.Print("[BaseClient] ERR : Failed to send data : Mutex taken"); }
        }

    }
}
