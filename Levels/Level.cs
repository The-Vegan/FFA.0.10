﻿using FFA.Empty.Empty.Network.Client;
using FFA.Empty.Empty.Network.Server;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public abstract class Level : TileMap
{
    //Level Variable
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected short globalBeat = 0;
    protected Random rand = new Random();
    protected Vector2[] spawnpoints = new Vector2[12];

    protected List<Vector2[]> TeamSpawnPoints = new List<Vector2[]>();

    protected Timer timer;
    protected Camera2D camera;
    protected PackedScene hudScene = GD.Load<PackedScene>("res://UIAndMenus/HUD/Hud.tscn");
    [Export]
    protected PackedScene loadedEndScreen;

    protected ControllerPlayer controllerPlayer;

    protected bool teamMode = false;

    public abstract byte GetLvlID();
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Level Variable

    //Network
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //protected Dictionary<byte,NetworkController> distantPlayerControllers = new Dictionary<byte, NetworkController>();
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Network

    //Entities Variables
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected byte idToGive = 0;
    protected List<Entity> allEntities = new List<Entity>();
    protected Dictionary<byte, Entity> idToEntity = new Dictionary<byte, Entity>();
    protected Dictionary<Vector2, Entity> coordToEntity = new Dictionary<Vector2, Entity>();
    protected List<Attack> allAtks = new List<Attack>();
    [Export]
    protected List<GenericController> allControllers = new List<GenericController>() { null };
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Entities Variables

    //DEPENDANCIES
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\  
    protected Global global;

    protected Entity mainPlayer;
    
    protected PackedScene atkScene = GD.Load("res://Abstract/Attack.tscn") as PackedScene;

    [Signal]
    protected delegate void checkEndingCondition();

    protected PackedScene pirateScene = GD.Load("res://Entities/Pirate/Pirate.tscn") as PackedScene;
    protected PackedScene blahajScene = GD.Load("res://Entities/Blahaj/Blahaj.tscn") as PackedScene;
    protected PackedScene monstropisScene = GD.Load("res://Entities/Monstropis/Monstropis.tscn") as PackedScene;

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //DEPENDANCIES


    //INIT METHODS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void InitGlobal(Global singleton)
    {
        global = singleton;
        global.gMap.SetMap(this);
        if (global.isMultiplayer)
        {
            if (global.hasServer) return;
        }
        
        InitPlayerAndMode(global.playerCharID, global.gamemode, global.numberOfTeams);
    }

    public bool InitPlayerAndMode(byte chosenCharacter, byte gameMode, byte numberOfTeams)//Solo/client
    {

        InitGameMode(gameMode, numberOfTeams);
        if (global.isMultiplayer) mainPlayer = CreateEntityInstance(chosenCharacter, " ", global.Network.client.id);
        else mainPlayer = CreateEntityInstance(chosenCharacter, " ",1);//CreateEntityInstance Adds the entity to the List of all entities

        GD.Print("[Level] LoadCompleted in level solo/client");
        return true;

    }
    
    public void InitPlayerAndModeServer(byte gameMode,byte numberOfTeams)//Host
    {
        if (!global.hasServer) return;
        
        PlayerInfo[] players = global.Network.server.GetPlayerFromServer();
        if (players.Length > 16) throw new ArgumentException("[Level] Invalid PlayerInfo Array");

        InitGameMode(gameMode, numberOfTeams);

        PackedScene controllerToLoad = GD.Load<PackedScene>("res://Abstract/NetworkController.tscn");

        Entity entity;
        
        
        for (byte i = 0;i < players.Length; i++)
        {
            //Creates players according to playerInfo list
            if (players[i] == null) continue;
            entity = CreateEntityInstance(players[i].characterID, players[i].name, players[i].clientID);
            Spawn(entity);

            //Creates controller
            NetworkController loadedController = controllerToLoad.Instance() as NetworkController;
            AddChild(loadedController);
            loadedController.Init(entity);
            allControllers.Add(loadedController);

            //Identifies MainPlayer
            if (global.clientID == players[i].clientID)
            {
                
                mainPlayer = entity;
            }

        }
    }

    public void InitPlayerAndModeClient()
    {
        if (global.hasServer) return;

        PlayerInfo[] players = global.Network.client.GetPlayersInfo();
        PackedScene networkController = GD.Load<PackedScene>("res://Abstract/NetworkController.tscn");
        for(byte i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;

            if (players[i].clientID == global.clientID)
            {
                Spawn(mainPlayer);
            }
            else
            {
                Spawn(CreateEntityInstance(players[i].characterID, players[i].name, players[i].clientID));
            }
        }


    }

    protected void InitGameMode(byte gameMode, byte numberOfTeams)
    {
        switch (gameMode)//TODO : code the modes
        {
            case 0:
            default://fail-safe
                this.Connect("checkEndingCondition", this, "ClassicEndCond");
                teamMode = false;
                InitSpawnPointsClasssic();
                GD.Print("[Level] Classic endCond Connected");
                break;
            case 1:
                this.Connect("checkEndingCondition", this, "TeamEndCond");
                teamMode = true;
                InitSpawnPointsTeam(numberOfTeams);
                GD.Print("[Level] Team endCond Connected");
                break;
            case 2:
                this.Connect("checkEndingCondition", this, "CTFEndCond");//CTF NOT CODED
                teamMode = true;
                InitSpawnPointsCTF(numberOfTeams);
                GD.Print("[Level] CTF endCond Connected");
                break;
            case 3:
                this.Connect("checkEndingCondition", this, "SiegeEndCond");//SACKING NOT CODED
                teamMode = true;
                InitSpawnPointsSiege();
                GD.Print("[Level] Siege endCond Connected");
                break;
        }
    }

    //OVERRIDE
    protected abstract void InitSpawnPointsClasssic();
    protected abstract void InitSpawnPointsTeam(int nbrOfTeams);
    protected abstract void InitSpawnPointsCTF(int nbrOfTeams);
    protected abstract void InitSpawnPointsSiege();//Always 4 teams
    //OVERRIDE

    public override void _Ready()
    {
        camera = this.GetNode("Camera2D") as Camera2D;
        timer = this.GetNode<Timer>("Timer");
        this.RemoveChild(camera);
        Hud hud = hudScene.Instance() as Hud;

        PackedScene controllScene = GD.Load("res://Abstract/ControllerPlayer.tscn") as PackedScene;
        ControllerPlayer cp = controllScene.Instance() as ControllerPlayer;
        allControllers[0] = cp;
        this.AddChild(cp);

        
        mainPlayer.Connect("noteHiter", hud, "HitNote");
        mainPlayer.AddChild(camera);
        camera.AddChild(hud);
        camera.Current = true;

        if (global.isMultiplayer)
        {
            cp.Init(global);
        }
        else
        {
            cp.Init(mainPlayer);
            timer.Start();
        }
    }
    public void StartTimer() { timer.Start(); }//Launched when all clients are "ready"
    public float GetTime()
    {
        if (timer == null) return 0;
        return timer.TimeLeft;
    }

    protected void SpawnAllEntities(byte numberOfEntities)
    {
        PackedScene cpu = GD.Load("res://Abstract/GenericController.tscn") as PackedScene;
        for (int i = allEntities.Count; i < numberOfEntities; i++)
        {
           CreateEntityInstance(cpu,"");
        }
        for (int i = 0; i < allEntities.Count; i++)
        {
            Spawn(allEntities[i]);
        }

    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //INIT METHODS

    //ENTITY RELATED METHODS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected Entity CreateEntityInstance(PackedScene pcs,string nametag)
    {
        
        while (idToEntity.ContainsKey(idToGive) || idToGive == 0) 
        { 
            idToGive++;
        }
        return CreateEntityInstance(rand.Next(1,4), nametag,idToGive);//Creates random entity
    }
    protected Entity CreateEntityInstance(int entityID, string nametag, byte setEntityID)
    {
        if (setEntityID == 0 || setEntityID > 16) throw new ArgumentException("ClientID must be between 1 and 16, was " + setEntityID);
        Entity playerEntity;
        //Selects correct entity from parameter ID
        switch (entityID)
        {
            case 1://Pirate
                playerEntity = pirateScene.Instance() as Pirate;
                break;
            case 2://♥
                playerEntity = blahajScene.Instance() as Blahaj;
                break;
            case 3:
                playerEntity = monstropisScene.Instance() as Monstropis;
                break;
            default://Random
                return CreateEntityInstance(rand.Next(1, 4), nametag, setEntityID);

        }//End of characters switch statement

        //Finalizes configurations for player entity
        allEntities.Add(playerEntity);
        idToEntity.Add(setEntityID, playerEntity);
        
        playerEntity.Init(this, nametag, setEntityID);
        this.AddChild(playerEntity, true);
        
        if (teamMode)
        {
            //TODO//TODO//TODO//TODO//TODO
        }
        else
        {
            playerEntity.team = playerEntity.id;
        }
        return playerEntity;
    }
    public void DeleteEntity(Entity entity)
    {
        byte eID = entity.id;
        idToEntity.Remove(entity.id);
        allEntities.Remove(entity);

        entity.QueueFree();
        GD.Print("[Level] " + entity + " has been deleted from existance");
    }

    public void MoveEntity(Entity entity,Vector2 newTile)
    {
        //Checks if tile is walkable
        if (this.GetCell((int)(newTile.x),(int)(newTile.y)) == 0)
        {
            entity.Moved(newTile);
            EntityCheckForAttack(entity);
        }
        else
        {
            GD.Print("[Level] Invalid Destination :" + newTile);
            entity.Moved(entity.pos);
        }
    }
    public void MoveEntity(Entity entity, Vector2[] targetTiles, short damage)
    {
        for(int i = 0; i < targetTiles.Length; i++)
        {
            Vector2 newTile = targetTiles[i];
            if (this.GetCell((int)(newTile.x), (int)(newTile.y)) == 0)
            {
                entity.Moved(newTile);
                EntityCheckForAttack(entity);
                return;
            }
            else if (damage > 0)
            {
                if (this.GetCell((int)(newTile.x), (int)(newTile.y)) == 3)//if tile is occupied by another entity
                {
                    Entity target = coordToEntity[newTile];
                    if ((target.GetHealthPoint()) < damage)
                    {
                        entity.Moved(newTile);
                        EntityCheckForAttack(entity);
                    }
                }
                

            }

        }
        GD.Print("[Level] No valid Destinations in provided array");
        entity.Moved(entity.pos);


    }

    public async void Spawn(Entity entity)
    {
        if (!this.IsInsideTree()) await ToSignal(this, "ready");

        byte failures = 0;//Forces spawning if fails too much

        entity.ResetHealth();
        while (failures < 65)
        {
            if (!teamMode)
            {
                int randomTile = rand.Next(spawnpoints.Length);

                if (SpawnPointInoccupied(randomTile))//If tile isn't occupied
                {
                    entity.Moved(spawnpoints[randomTile]);
                    await ToSignal(entity.GetNode("Tween"), "tween_completed");
                    entity.SetSelfModulate(Color.Color8(255,255,255,255));
                    break;
                }
            }
            else //if (teamMode)
            {
                int randomTile = rand.Next(TeamSpawnPoints[entity.team].Length);

                if ((this.GetCell((int)TeamSpawnPoints[entity.team][randomTile].x, (int)TeamSpawnPoints[entity.team][randomTile].y) == 0) && (failures < 64))
                {
                    entity.Moved(TeamSpawnPoints[entity.team][randomTile]);
                    await ToSignal(entity.GetNode("Tween"), "tween_completed");
                    entity.SetSelfModulate(Color.Color8(255, 255, 255, 255));
                    break;
                }
            }
            failures++;
        }
       
    }

    private bool SpawnPointInoccupied(int randomTile)
    {
        Vector2 tile = spawnpoints[randomTile];
        if ((this.GetCell((int)tile.x, (int)tile.y) != 0)) return false;

        for (short yAxis = (short)(tile.y - 2); yAxis <= (short)(tile.y + 2); yAxis++)
        {
            for (short xAxis = (short)(tile.x - 2); xAxis <= (short)(tile.x + 2); xAxis++)
            {
                if (this.GetCell(xAxis, yAxis) == 3) return false;
            }
        }
        return true;
    }

    public void SetEntityPacket(byte entityID, short packet, float timing)
    {
        try
        {

            Entity entity = idToEntity[entityID];
            if (entity.actedThisBeat) return;
            NetworkController c = allControllers[entity.id] as NetworkController;
            if(c == null) return;
            
            c.PacketSetByServer(packet);
            float delta = Math.Abs(entity.timing - timing);//Obtient l'écart entre lvl et entité
            GD.Print("[Level] Movement set by distant machine on entity" + entityID);
            if (delta < 0.03f) entity.timing = timing;//Cecks for lag(in seconds)

            if (global.hasServer)
            {
                global.Network.server.SendMovePacket(entity.id, entity.packet, entity.timing);
            }
        }
        catch (KeyNotFoundException) { GD.Print("[Level] SetEntityPacket couldn't find Entity or NetworkController"); }
        
    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ENTITY RELATED METHODS

    //ATTACK RELATED METHODS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void CreateAtk(Entity source, List<List<short[]>> atkData, ushort[] textureIDs, byte[] collumns, bool flipable)
    {
        Attack atkInstance = atkScene.Instance() as Attack;
        allAtks.Add(atkInstance);
        atkInstance.InitAtk(source, atkData, this, textureIDs, collumns, flipable,this.GetTime());
        this.AddChild(atkInstance, true);

    }

    public void AttackCheckForEntity(Attack atk)
    {
        for (byte i = 0; i < allEntities.Count; i++)
        {
            
            Entity e = allEntities[i];
            if (atk.posToTiles.ContainsKey(e.pos))
            {
                if (atk.posToTiles[e.pos].GetDamage() == 0) continue;
                e.Damaged(atk);
                e.MidBeatDamage(atk);
            }
        }
    }

    public void EntityCheckForAttack(Entity entity)
    {
        ushort numberOfAtk = (ushort)allAtks.Count;
        for(ushort i = 0; i < numberOfAtk; i++)
        {
            Attack atk = allAtks[i];
            if (atk == null) continue;
            if (atk.posToTiles.ContainsKey(entity.pos))
            {
                if (atk.posToTiles[entity.pos].GetDamage() == 0) continue;
                entity.Damaged(atk);

            }
        }
    }

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ATTACK RELATED METHODS

    //ENDING CONDITION
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\

    protected async void ClosingArena()
    {
        EndScreen es = loadedEndScreen.Instance() as EndScreen;
        
        Entity[] valids = Entity.StripNonPlayerEntities(allEntities);
        if (es == null) return;
        es.Init(valids);

        await ToSignal(GetTree().CreateTimer(2), "timeout");
        timer.Stop();
        (es.GetChild(0) as Node2D).Position = new Vector2(0, -864);
        
        camera.AddChild(es);

    }

    protected virtual void ClassicEndCond()
    {
        if (globalBeat == 200)
        {
            GD.Print("[Level][ENDCOND] ENDCOND FIRED");
            ClosingArena();
        }
    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ENDING CONDITION

    //NETWORKING
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void ResyncEntities(List<SyncEntityPacket> allPackets)
    {
        for(byte i = 0; i < allPackets.Count; i++)
        {
            Entity resync = idToEntity[allPackets[i].entityID];

            resync.Sync(allPackets[i]);
        }
    }

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //NETWORKING
    public void TimerUpdate()
    {
        globalBeat++;
        GD.Print("[Level][Server] - - - - - - - - - - - - - - - - - - - - - - - - " + globalBeat);
        if(global.hasServer) global.Network.server.SendSync(allEntities); 
      
        UpdateAllEntities();

        UpdatePositionDictionary();

        GetTree().CallGroup("Attacks", "BeatAtkUpdate");//Also updates the hud

        EmitSignal("checkEndingCondition");
    }
    public void TimerUpdate(LocalClient sender)
    {
        if (global.hasServer) return;

        timer.OneShot = true;
        timer.Start();
        globalBeat++;
        GD.Print("[Level][Client] - - - - - - - - - - - - - - - - - - - - - - - - " + globalBeat);

        UpdateAllEntities();

        UpdatePositionDictionary();

        GetTree().CallGroup("Attacks", "BeatAtkUpdate");//Also updates the hud
        
    }

    protected void UpdateAllEntities()
    {
        for (int i = 0;i < allEntities.Count; i++)
        {
            allEntities[i].BeatUpdate();
        }
    }

    private void UpdatePositionDictionary()
    {
        coordToEntity = new Dictionary<Vector2, Entity>();
        
        for (int i= 0; i < allEntities.Count; i++)
        {
            if(!coordToEntity.ContainsKey(allEntities[i].pos)) //If no entities are already on that tile
                coordToEntity.Add(allEntities[i].pos, allEntities[i]);//Adds the position of the entity as it's key
        }
    }



}
