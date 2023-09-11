using FFA.Empty.Empty.Network.Client;
using Godot;
using System;
using System.Collections.Generic;

public class GLevel : Node
{
    private Level map;
    public override void _Ready(){ }
    public void SetMap(Level lvl) { GD.Print("[GLevel]Map was Set to " + lvl); map = lvl; }

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

    public void SetEntityPacket(byte clientID, short move, float time)
    {
        map.SetEntityPacket(clientID, move, time);
    }

    public void StartLevelTimer()
    {
        if(map == null) GD.Print("[GLevel] Level null");
        if (!map.IsInsideTree())GD.Print("[GLevel] Level Out of tree");

        map.StartTimer();

    }
}
