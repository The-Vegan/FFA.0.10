using Godot;
using System;

public class GNetwork : Node
{
    public GServer server;
    public GClient client;
    public override void _Ready()
    {
        server = GetNode<GServer>("GServer");
        client = GetNode<GClient>("GClient");
    }

}
