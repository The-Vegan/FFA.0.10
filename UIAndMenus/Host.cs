using Godot;
using System.Net;

public class Host : Button
{
    protected Global global;
    protected MainMenu mm;
    public override void _Ready()
    {
        global = this.GetTree().Root.GetNode<Global>("Global");
        mm = this.GetParent().GetParent() as MainMenu;
    }

    public override void _Pressed()
    {
        mm.MoveCameraTo(1);
        if (!global.Network.server.CreateServer())
        {
            GD.Print("[Host] Failed to create server");
            return;
        }
        if (!global.Network.client.CreateClient(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString())) GD.Print("[Host] Failed to connect to localhost");
    }
}
