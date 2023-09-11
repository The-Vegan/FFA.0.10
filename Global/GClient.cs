using FFA.Empty.Empty.Network.Client;
using Godot;
using System;

public class GClient : Node
{
    public bool exists => client != null;
    public byte id => client.clientID;

    private LocalClient client;
    private Global global;
    public override void _Ready()
    {
        global = GetParent().GetParent<Global>();
    }
    public bool CreateClient(string serverIP)
    {
        try
        {
            this.client = new LocalClient(serverIP);
            client.global = this.GetParent().GetParent<Global>();
            if (client.global == null) throw new Exception("[GClient] WTF, Global is null???");
        }
        catch (Exception e)
        {
            GD.Print("[Global] Err Creating Client : " + e);
            return false;
        }
        return true;
    }

    public void Disconnect()
    {
        client?.Disconnect();
        client = null;
    }

    public PlayerInfo[] GetPlayersInfo() { return client?.GetPlayersInfo(); }
    public void SendPacketToServer(short p) { new System.Threading.Thread(delegate () { client?.SendPacketToServer(p); }).Start(); }
    public void SendCharIDAndName(string name) { client?.SendCharIDAndName(name, global.playerCharID); }
    public void SignalReady() { client.SignalReady(); }
}
