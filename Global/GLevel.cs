using FFA.Empty.Empty.Network.Client;
using Godot;
using System;
using System.Collections.Generic;

public class GLevel : Node
{
    private System.Threading.Mutex textureAdderMutex = new System.Threading.Mutex();
    private Level map;
    private Global global;
    public override void _Ready(){ global = GetParent() as Global; }
    public void SetMap(Level lvl) { GD.Print("[GLevel]Map was Set to " + lvl); map = lvl; }
    [Export]
    private Texture[] loadedAttackTextures = new Texture[] { GD.Load("res://Entities/Default.png") as Texture };
    
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


    public void StartLevelTimer()
    {
        if(map == null) GD.Print("[GLevel] Level null");
        if (!map.IsInsideTree())GD.Print("[GLevel] Level Out of tree");

        map.StartTimer();

    }

    //ATTACK TEXTURES
    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
    public Texture GetTexture(ushort index)
    {
        if (index >= loadedAttackTextures.Length) return loadedAttackTextures[0];
        if (loadedAttackTextures[index] == null) return loadedAttackTextures[0];
        return loadedAttackTextures[index];
    }

    public ushort LoadTexture(String texturePath)
    {
        Texture loaded = GD.Load(texturePath) as Texture;

        if (loaded == null) return 0;
        for(ushort i = 0; i < loadedAttackTextures.Length; i++)
        {
            if (loaded == loadedAttackTextures[i]) return i;
        }
        AddTexture(loaded);
        GD.Print("[Glevel] Added texture : " + (loadedAttackTextures.Length - 1));

        if (global.hasServer) global.Network.server.SendPathLoad(texturePath);

        return (ushort)(loadedAttackTextures.Length -1);
    }

    private void AddTexture(Texture loaded)
    {
        textureAdderMutex.WaitOne();
        
        Texture[] arr = new Texture[loadedAttackTextures.Length + 1];
        for (ushort i = 0; i < loadedAttackTextures.Length; i++) arr[i] = loadedAttackTextures[i];
        arr[loadedAttackTextures.Length] = loaded;
        loadedAttackTextures = arr;

        textureAdderMutex.ReleaseMutex();

        
    }
    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
    //ATTACK TEXTURES
}
