using Godot;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FFA.Empty.Empty.Network.Server
{
    public class StreamListener
    {
        private NetworkStream stream;
        private long lastPing = 0;
        public NetworkStream GetStream() { return stream; }
        //Events
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        public delegate void Disconnected(StreamListener Sender);
        public event Disconnected DisconnectedEvent = delegate { };

        public delegate void DataRecieved(object sender, byte[] data, NetworkStream stream);
        public event DataRecieved DataRecievedEvent = delegate { };
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Events


        public StreamListener(NetworkStream str)
        {
            this.stream = str;

            new System.Threading.Thread(Listening).Start();
            new System.Threading.Thread(PingThread).Start();
        }
        public void Close() { stream.Close(); }
        public void Listening()
        {
            byte[] buffer = new byte[8_192];
            while (true)
            {
                try
                {
                    int lu = stream.Read(buffer, 0, buffer.Length);
                    if (lu == 0) { continue; }
                    if (buffer[0] == 0) try { ping.SetResult(true); } catch (InvalidOperationException) { GD.Print("[StreamListener] Ping recieved out of cooldown"); }
                    else DataRecievedEvent(this, buffer, stream);
                }
                catch (Exception e)  
                {
                    GD.Print("[StreamListener]" + e.ToString());
                    stream.Close();
                    stream.Dispose();
                    DisconnectedEvent(this);
                    break;
                }

            }

        }

        public void Write(byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        //Ping
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        private TaskCompletionSource<bool> ping;

        private async void PingThread()
        {
            byte lostPings = 0;
            while (lostPings < 10)
            {
                System.Threading.Thread.Sleep(1000);


                Stopwatch pingTime = new Stopwatch();
                ping = new TaskCompletionSource<bool>();
                pingTime.Start();
                try
                {
                    stream.Write(new byte[]{
                        0,//Ping protocol
                        (byte)(lastPing >> 24),
                        (byte)(lastPing >> 16),
                        (byte)(lastPing >> 8),
                        (byte)lastPing },
                    0,
                    5);

                }
                catch (Exception e)
                {
                    GD.Print("[StreamListener] Terminating Stream, Exception caught : " + e);
                    break;
                }

                new System.Threading.Thread(TimeoutThread).Start();
                if (await ping.Task)
                {
                    pingTime.Stop();
                    lostPings = 0;
                    lastPing = pingTime.ElapsedMilliseconds;
                }
                else
                {
                    lostPings++;
                    lastPing = lostPings * 1000;
                    GD.Print("[StreamListener] lost " + lostPings);
                }

            }
            DisconnectedEvent(this);
            stream.Close();
            stream.Dispose();

        }
        private void TimeoutThread()
        {
            System.Threading.Thread.Sleep(750);
            ping.TrySetResult(false);
        }
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
        //Ping
    }
}
