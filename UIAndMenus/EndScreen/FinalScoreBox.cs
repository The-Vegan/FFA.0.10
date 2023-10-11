using Godot;
using System;

public class FinalScoreBox : Control
{

    //Images
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\

    protected String bannerPath;

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Images

    //Variables
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected String name = "default";
    protected short score = -1;
    protected short perfectBeats = 621;
    protected short missedBeats = 5;//mister beast

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Variables

    //Child Nodes
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected Sprite banner;
    protected Sprite portrait;
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ChildNode


    //Init Method
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public void Init(byte podium,Entity player)
    {
        switch (podium)
        {
            case 1://TODO : IMPLEMENT BANNER PATHS (I NEED THE TEXTURES FIRST)
                break;
            case 2:
                break;
            case 3:
                break;

            default: throw new ArgumentException();
        }

        this.name = player.GetNametag();
        this.perfectBeats = player.GetPerBeat();
        this.missedBeats = player.GetMisBeat();


    }
    public override void _Ready()
    {
        banner = this.GetChild(0) as Sprite;
        portrait = banner.GetNode("Portrait") as Sprite;

        banner.GetNode<Label>("Name").Text = name;
        banner.GetNode<Label>("Score").Text = score.ToString();
        banner.GetNode<Label>("PerfectBeats").Text = perfectBeats.ToString();
        banner.GetNode<Label>("MissedBeats").Text = missedBeats.ToString();

    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //Init Method
}
