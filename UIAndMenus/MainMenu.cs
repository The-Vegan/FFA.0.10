using FFA.Empty.Empty.Network.Client;
using FFA.Empty.Empty.Network.Server;
using Godot;
using System;
using System.Collections.Generic;

public class MainMenu : Control
{
	//Nodes
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\

	protected Camera2D camera;
	protected VBoxContainer playerListBox;
	protected Label ipLineEditMessage;
	public LineEdit nameBox;

	public Sprite resetNetworkConfigForm;
	protected Button launchButton;

	protected Label countDownLabel;
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	//Nodes

	//Level Initialisation Variables
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	private Global global;
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	//Level Initialisation Variables

	//Network
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\

	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	//Network

	public override void _Ready()
	{
		global = this.GetTree().Root.GetNode<Global>("Global");

		camera = this.GetNode("Camera2D") as Camera2D;

		playerListBox = this.GetNode("WaitForPlayers/VBoxContainer") as VBoxContainer;
		ipLineEditMessage = this.GetNode("ConnectToServer/IpTextBox/Label") as Label;
		nameBox = GetNode("Camera2D/CanvasLayer/NameBox") as LineEdit;

		launchButton = GetNode("WaitForPlayers/Start") as Button;

		resetNetworkConfigForm = GetNode("Camera2D/CanvasLayer/ResetNetworkConfigForm") as Sprite;

		countDownLabel = this.GetNode("WaitForPlayers/CountDown") as Label;
	}

	//Camera Position
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	protected List<Vector2> back = new List<Vector2>() { new Vector2(0, 0) };

	protected Vector2 MAINMENU = new Vector2(0, 0);
	protected Vector2 SOLO = new Vector2(0, -576);
	protected Vector2 CHARSELECT = new Vector2(-1024, 0);
	protected Vector2 LEVELSELECT = new Vector2(-2048, 0);
	protected Vector2 MULTI = new Vector2(0, 576);
	protected Vector2 JOIN = new Vector2(-1024, 576);
	protected Vector2 WAITFOROTHERS = new Vector2(-3072, 0);

	public void MoveCameraTo(sbyte destination)
	{
		if (destination == -1)
		{
			camera.Position = back[back.Count - 1];
			back.RemoveAt(back.Count - 1);

			if (back.Count <= 0) back.Add(MAINMENU);
			return;
		}

		back.Add(camera.Position);
		nameBox.Visible = false;
		switch (destination)
		{
			case 0://MainMenu                                   
				global.ResetNetworkConfigAndGoBackToMainMenu();
				break;
			case 1://Solo
				global.ResetNetworkConfigAndGoBackToMainMenu();
				camera.Position = SOLO;
				break;
			case 2://Character Select
				camera.Position = CHARSELECT;
				if (global.isMultiplayer) nameBox.Visible = true;
				break;
			case 3://Level Select
				camera.Position = LEVELSELECT;
				break;
			case 4://Multi
				camera.Position = MULTI;
				break;
			case 5://Join
				camera.Position = JOIN;
				break;
			case 6://WaitForHostToStartGame
				camera.Position = WAITFOROTHERS;
				launchButton.Visible = (global.hasServer);
				if (global.isMultiplayer) nameBox.Visible = true;
				break;
			default://Returns to MainMenu in case of error
				global.ResetNetworkConfigAndGoBackToMainMenu();
				break;
		}
	}
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	//Camera Position

	//IHM
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	public void SetGame(byte mode) { global.gamemode = mode; }
	public void SetCharacter(byte character) { global.playerCharID = character; global.Network.client.SendCharIDAndName(nameBox.Text); }
	public void SetPlayerName(string name) { global.Network.client.SendCharIDAndName(name); }

	public void CharacterChosen()//Called from the "Next" button on character screen
	{
		if (global.hasServer || !global.isMultiplayer)//If this is localhost OR if this is solo, choses level
			MoveCameraTo(3);
		else MoveCameraTo(6);
	}

	public void DisplayPlayerList(PlayerInfo[] playerList)
	{
		//Finds the label corresponding to players and sets thier state to "connected"
		for (byte i = 0; i < playerList.Length; i++)
		{

			CheckButton playerLabel = playerListBox.GetChild<CheckButton>(i);
			if (playerList[i] == null)
			{
				playerLabel.Pressed = false;
				playerLabel.Text = "*Empty*";
				continue;
			}
			playerLabel.Pressed = true;
			playerLabel.Text = playerList[i].ToString();
			GD.Print("[MainMenu] changed button " + playerList[i].clientID);
		}
	}

	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	//IHM

	//Network Related
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	

	public void BackToMainMenu()
	{
		GD.Print("[MainMenu] Reset Network Config");
		back.Clear();
		camera.Position = MAINMENU;
	}
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	//Network Related

	//Launched By Distant Host
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	private void OnLaunchedPressed()//You are the host
	{
		if (global.bufferLvlToLoad == null) {GD.Print("[MainMenu] Error, No level or Invalid level Selected"); return; }
		global.launchAborted = false;
		global.Network.server.BeginLaunch();
	}
	public async void CountDownTimer()
	{
		sbyte sec = 3;
		countDownLabel.Visible = true;

		while (sec >= 0)
		{
			countDownLabel.Text = sec.ToString();
			
			await ToSignal(GetTree().CreateTimer(0.25f), "timeout"); if (global.launchAborted) break;
			await ToSignal(GetTree().CreateTimer(0.25f), "timeout"); if (global.launchAborted) break;
			await ToSignal(GetTree().CreateTimer(0.25f), "timeout"); if (global.launchAborted) break;
			await ToSignal(GetTree().CreateTimer(0.25f), "timeout"); if (global.launchAborted) break;
			sec--;
		}
		countDownLabel.Visible = false;
	}

	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	//Launched By Distant Host

	//Level Loading
	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\


	//*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
	//Level Loading
}
