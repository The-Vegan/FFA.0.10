using FFA.Empty.Empty.Network.Client;
using Godot;



public class GenericController : Node2D
{
    protected Entity entity;
    protected Global global;
    protected ControllableObject target;

    protected short packet;
    public void Init(Entity owner) { entity = owner; global = null; target = null; }
    public void Init(Global owner) { global = owner; entity = null; target = null; }
    public void Init(ControllableObject owner) {  target = owner; entity = null;global = null; }
}




