using FFA.Empty.Empty.Network.Client;
using FFA.Empty.Empty.Network.Server;
using Godot;
using System;
using System.Collections.Generic;

public class GServer : Node
{
    Global global;

    public bool exists => server != null;
    private HostServer server;
    public override void _Ready()
    {
        global = GetParent().GetParent<Global>();
    }
    public bool CreateServer()
    {
        try
        {
            this.server = new HostServer(global);
            this.server.AbortingLaunch += delegate { global.launchAborted = true; };
            this.server.CountdownWithoutEvents += global.PostCountdownProcedure;
            this.server.HostServerDisposableEvent += delegate { server = null; GC.Collect(); };
        }
        catch (Exception) { return false; }
        return true;

    }


    public void Terminate() { server?.Terminate(); }

    public PlayerInfo[] GetPlayer() { return server.GetPlayer(); }
    public PlayerInfo[] GetPlayerFromServer() { return server.GetPlayer(); }
    public void BeginLaunch() { new System.Threading.Thread(server.BeginLaunch).Start(); }
    public void SendMovePacket(byte id, short packet, float timing) { server.SendMovePacket(id, packet, timing); }
    public void SendSync(List<Entity> allEntities) { new System.Threading.Thread(delegate () { server.SendSync(allEntities); }).Start(); }

    public void SendPathLoad(string texturePath)
    {
        new System.Threading.Thread(delegate () { server.SendTexturePath(texturePath); }).Start();
    }
}
