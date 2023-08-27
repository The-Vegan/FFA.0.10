using FFA.Empty.Empty.Network.Client;
using Godot;
using System;
using System.Collections.Generic;

public class GLevel : Node
{
    private Level map;
    private Global global;
    public override void _Ready()
    {
        global = GetParent<Global>();
    }

    public void SetMap(Level lvl) { map = lvl; }

    public void InitPlayerCoordinatesInLevel(Dictionary<byte, Vector2> IDtoPos) { map.InitPlayerCoordinates(IDtoPos); }

    public void SetEntityPacketOnLevel(byte entityID, short entityPacket, float time)
    {
        map?.SetEntityPacket(entityID, entityPacket, time);
    }
    public void ResyncEntitiesInLevel(List<SyncEntityPacket> packets)
    {
        map?.ResyncEntities(packets);
    }
    public void LevelTimerSync(LocalClient sender)
    {
        map?.TimerUpdate(sender);
    }
}
