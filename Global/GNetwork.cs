using Godot;
using System;

public class GNetwork : Node
{
    [Export]
    public GServer server;
    [Export]
    public GClient client;
    public override void _Ready()
    {
        server = GetNode<GServer>("GServer");
        client = GetNode<GClient>("GClient");
    }

}
