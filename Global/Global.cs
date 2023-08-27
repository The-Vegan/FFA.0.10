using FFA.Empty.Empty.Network.Client;
using FFA.Empty.Empty.Network.Server;
using Godot;
using System;
using System.Collections.Generic;

public partial class Global : Node
{
    //Level
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public GLevel gMap;

    public byte playerCharID = 0;
    public byte gamemode = 0;
    public byte numberOfTeams = 1;

    public PackedScene bufferLvlToLoad;
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Level

    //QOF options
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public byte bufferFrameInput = 2;

    public float musicVolume = 1.0f;
    public float sfxVolule = 1.0f;
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //QOF options

    //Network
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public GNetwork Network;
    

    public byte clientID = 0;

    public bool launchAborted = false;
    public bool isMultiplayer => Network.client.exists;
    public bool hasServer => Network.server.exists;

    

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Network

    //Active Scene
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    private Node activeScene;
    private void SetActiveScene(Node n) 
    { 
        activeScene = n;
    }

    public bool isPlayingLevel => (activeScene.GetType() == typeof(Level));
    public Level GetLevel() { return activeScene as Level; }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Active Scene

    public override void _Ready()
    {
        SetActiveScene(this.GetTree().Root.GetNode<MainMenu>("MainMenu"));

        Network = GetNode<GNetwork>("GNetwork");
        
    }
    //Scene Loading and other instancing
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void CloseMenuAndOpenLevel(MainMenu mm, PackedScene levelToLoad)
    {
        Level loadedLevel = levelToLoad.Instance() as Level;
        loadedLevel.Init(this,false);

        GetTree().Root.AddChild(loadedLevel, true);
        SetActiveScene(loadedLevel);
        
        mm.QueueFree();
    }

    public void ResetNetworkConfigAndGoBackToMainMenu()
    {
        if (activeScene.GetType() != typeof(MainMenu))
        {
            activeScene.QueueFree();
            LoadMenu();
        }
        else (activeScene as MainMenu).BackToMainMenu();

        Network.server.Terminate();
        Network.client.Disconnect();

        GC.Collect();
    }

    private void LoadMenu()
    {
        PackedScene menu = GD.Load<PackedScene>("res://UIAndMenus/MainMenu.tscn");
        MainMenu loadedMenu = menu.Instance() as MainMenu;
        GetTree().Root.AddChild(loadedMenu);
        SetActiveScene(loadedMenu);
    }

    public void PostCountdownProcedure(HostServer sender)//Called from beginLaunch
    {
        GD.Print("[Global] PostCountdownProcedure called");
        sender.AssignRandomCharacters();

        Level LoadedLevel = bufferLvlToLoad.Instance() as Level;
        if (LoadedLevel == null)
        {
            GD.Print("[Global] Error, Failed to load Lvl");
            sender.AbortLaunch();
            launchAborted = true;
            return;
        }
        

        GD.Print("[Global] Lvl loaded with success");

        LoadedLevel.InitPlayerAndModeServer(gamemode, numberOfTeams);
        //Dictionary<byte, Vector2> IDToPositions = 
        GetTree().Root.AddChild(LoadedLevel, true);

        GD.Print("[Global]Position sync dictionary recieved with success");

        sender.SetUnReady();
        sender.SendStartSignalToAllClients(LoadedLevel.GetLvlID());
        Network.client.SignalReady();

        //TODO Check for failure? 
        //TODO Check if failure is an option.
        activeScene.QueueFree();
        SetActiveScene(LoadedLevel);
        //Failure is not an option
    }



    internal void LoadMapFromID(byte mapID)
    {
        if (hasServer) return;
        GD.Print("[Global] loading Level with ID :" + mapID);
        string mapPath;
        switch (mapID)
        {
            case 1:
                mapPath = "res://Levels/Kyomira1.tscn";
                break;
            case 2:
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();
        }
        if (String.IsNullOrEmpty(mapPath)) throw new ArgumentException();

        //Level map = GD.Load<Level>(mapPath);

        PackedScene mapScene = GD.Load<PackedScene>(mapPath);
        Level map = mapScene.Instance() as Level;

        map.InitPlayerAndModeClient();
        GetTree().Root.AddChild(map);

        activeScene.QueueFree();
        SetActiveScene(map);

    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Scene Loading

    //Menu Interface
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void CountDownTimer() { (activeScene as MainMenu)?.CountDownTimer();}
    public void DisplayPlayerList(PlayerInfo[] players) {(activeScene as MainMenu)?.DisplayPlayerList(players);}


    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Menu Interface

    
}