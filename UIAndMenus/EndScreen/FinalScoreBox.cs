using Godot;
using System;

public class FinalScoreBox : Control
{

    //Variables
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected String name;
    protected short score;
    protected short perfectBeats;
    protected short missedBeats;//mister beast

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Variables

    //Child Nodes
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected Sprite banner;
    protected Texture bnrTexture;
    protected Texture prtrtTexture;
    protected Sprite portrait;
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ChildNode


    //Init Method
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void Init(byte podium,Entity player)
    {
        String folder = "res://UIAndMenus/EndScreen/Textures/";

        bnrTexture = GD.Load(folder + "w" + podium + ".png") as Texture;


        
        this.name = player.GetNametag();
        this.score = player.score;
        this.perfectBeats = player.GetPerBeat();
        this.missedBeats = player.GetMisBeat();
        this.prtrtTexture = player.GetPortrait();
        GD.Print("[FanalScoreBox][Init] score = " + score + " ; P = " + perfectBeats + " ; M = " + missedBeats);
    }
    public override void _Ready()
    {
        GD.Print("[FanalScoreBox] score = " + score + " ; P = " + perfectBeats + " ; M = " + missedBeats);

        banner = this.GetChild(0) as Sprite;
        portrait = banner.GetChild(0) as Sprite;

        banner.Texture = bnrTexture;
        banner.GetChild<Sprite>(0).Texture = prtrtTexture;

        banner.GetChild<Label>(1).Text = name;
        banner.GetChild<Label>(2).Text = "Score : " + score;
        banner.GetChild<Label>(3).Text = "Perfects : " + perfectBeats;
        banner.GetChild<Label>(4).Text = "Missed : " + missedBeats;

    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Init Method
}
