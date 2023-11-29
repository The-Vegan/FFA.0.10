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
    public void LoadTexture(String texturePath,ushort forcedIndex)
    {
        if (global.hasServer) return;
        Texture loaded = GD.Load(texturePath) as Texture;//Loads before Mutex grab so that threading has a purpose
        if (loaded == null) return;

        if (textureAdderMutex.WaitOne(100))
        {
            if(loadedAttackTextures.Length < forcedIndex)
            {
                Texture[] arr = new Texture[forcedIndex];
                Array.Copy(loadedAttackTextures,arr,loadedAttackTextures.Length);
                loadedAttackTextures = arr;
            }
            textureAdderMutex.ReleaseMutex();
            return;
        }
    }
    public ushort LoadTexture(String texturePath)
    {
        Texture loaded = GD.Load(texturePath) as Texture;//Loads before Mutex grab so that threading has a purpose
        if (loaded == null) return 0;//Failed to load Texture , Returns default.png
        if (textureAdderMutex.WaitOne(100))
        {
            for (ushort i = 0; i < loadedAttackTextures.Length; i++)
            {
                if (loaded == loadedAttackTextures[i]) { textureAdderMutex.ReleaseMutex(); return i; }//Releases mutex before returning the TextureIdx
            }
            AddTexture(loaded);

            if (global.hasServer) global.Network.server.SendPathLoad(texturePath);//Sends to clients if host
            ushort retVal = (ushort)(loadedAttackTextures.Length - 1);//Saves index before releasing Mutex
            textureAdderMutex.ReleaseMutex();
            return retVal;
        }
        
        GD.Print("[GLevel] Failed to aquire TextureLoading Mutex before Timeout");
        return 0;
    }

    private void AddTexture(Texture loaded)
    {
        Texture[] arr = new Texture[loadedAttackTextures.Length + 1];
        Array.Copy(loadedAttackTextures, arr, loadedAttackTextures.Length);
        arr[loadedAttackTextures.Length] = loaded;
        loadedAttackTextures = arr;
    }
    //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-\\
    //ATTACK TEXTURES
}
