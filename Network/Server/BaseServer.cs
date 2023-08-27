using Godot;
using System;
using System.Net;
using System.Net.Sockets;

namespace FFA.Empty.Empty.Network.Server
{
    public class BaseServer
    {
        private TcpListener listener;
        private StreamListener[] streams = new StreamListener[16];

        //private NetworkStream[] streams = new NetworkStream[16];

        //Events
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public delegate void DataRecieved(object sender, byte[] data, NetworkStream stream);
        public event DataRecieved DataRecievedEvent = delegate { };

        public delegate void ClientConnected(object sender, byte id);
        public event ClientConnected ClientConnectedEvent = delegate { };

        public delegate void ClientDisconnected(object sender, byte clientID);
        public event ClientDisconnected ClientDisconnectedEvent = delegate { };
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Events
        public BaseServer()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    listener = new TcpListener(ip, 1404);
                    break;
                }
            }//Finds local IP address

            new System.Threading.Thread(ListeningThread).Start();
        }
        public void Terminate()
        {
            GD.Print("[BaseServer] terminating");
            for (byte i = 0; i < streams.Length; i++)
            {
                if (streams[i] != null)
                {
                    streams[i].Close();
                    streams[i] = null;
                }
                listener.Stop();

            }
        }
        private void ListeningThread()
        {
            listener.Start(20);
            while (true)
            {
                TcpClient c = listener.AcceptTcpClient();
                for (byte i = 0; i < streams.Length; i++)
                {
                    if (streams[i] == null)
                    {
                        streams[i] = new StreamListener(c.GetStream());
                        streams[i].DisconnectedEvent += Disconnected;
                        streams[i].DataRecievedEvent += DataRecievedByServer;
                        ClientConnectedEvent(this, (byte)(i + 1));
                        break;
                    }//Connect client and wires the corresponding events
                    if (i == streams.Length) c.Dispose();//If server is full, don't
                }
            }
        }

        private void DataRecievedByServer(object sender, byte[] data, NetworkStream stream)
        {
            this.DataRecievedEvent(this, data, stream);
        }

        private void Disconnected(StreamListener Sender)
        {
            Sender.DisconnectedEvent -= Disconnected;
            Sender.DataRecievedEvent -= DataRecievedByServer;

            for (byte i = 0; i < streams.Length; ++i)
            {
                if (streams[i] == Sender)
                {
                    streams[i] = null;
                    ClientDisconnectedEvent(this, (byte)(i + 1));
                    return;
                }
            }
        }

        public void SendDataOnSingleStream(byte[] data, byte clientID)
        {
            if (streams[clientID - 1] == null) return;
            if (data.Length > 8_192)
            {
                GD.Print("[BaseServer] Err : Message too long : " + data.Length);
            }
            else
            {
                streams[clientID - 1].Write(data);
            }
        }

        public void SendDataOnAllStreams(byte[] data) //avoids checks and returns to save optimisations
        {
            if (data.Length > 8_192)
            {
                GD.Print("[BaseServer] Err : Message too long : " + data.Length);
            }
            else
            {
                streams[0]?.Write(data);
                streams[1]?.Write(data);
                streams[2]?.Write(data);
                streams[3]?.Write(data);

                streams[4]?.Write(data);
                streams[5]?.Write(data);
                streams[6]?.Write(data);
                streams[7]?.Write(data);

                streams[8]?.Write(data);
                streams[9]?.Write(data);
                streams[10]?.Write(data);
                streams[11]?.Write(data);

                streams[12]?.Write(data);
                streams[13]?.Write(data);
                streams[14]?.Write(data);
                streams[15]?.Write(data);
            }
        }

        public NetworkStream GetStream(byte idx)
        {
            if (streams[idx] != null) return streams[idx].GetStream();

            GD.Print("[BaseServer] Stream " + idx + " is null");
            return null;
        }
    }
}
