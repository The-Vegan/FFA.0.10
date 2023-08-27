using FFA.Empty.Empty.Network.Client;
using Godot;



public class GenericController : Node2D
{
    protected Entity entity;
    protected Global global;

    protected short packet;
    public void Init(Entity owner) { entity = owner; }
    public void Init(Global owner) { global = owner; }
}




