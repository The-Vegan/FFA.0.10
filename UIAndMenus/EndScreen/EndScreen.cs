using Godot;
using System;
using System.Collections.Generic;

public class EndScreen : CanvasLayer
{
    byte listLength = 0;

    [Export]
    PackedScene loadedFinalScoreBox;
    [Export]
    PackedScene loadedLoserScoreBox;
    [Export]
    Entity[] players;

    Node2D n2d;

    Control container;
    [Export]
    VBoxContainer loserContainer;

    


    public void Init(Entity[] allPlayers)
    {
        if (allPlayers == null) return;
        {//CombSort
            byte gap = (byte)(allPlayers.Length >> 1);

            while (gap != 0)
    {
                Entity tempEntity;

                for (byte i = 0; i < allPlayers.Length - gap; i++)
                {

                    if (allPlayers[i].score > allPlayers[i + gap].score)
        {
                        //Swap
                        tempEntity = allPlayers[i];
                        allPlayers[i] = allPlayers[i + gap];
                        allPlayers[i + gap] = tempEntity;
                    }
        }

                gap--;
            }
        }//CombSort


        players = allPlayers;


    }

    private void SetCanvas(byte podium, Entity entity)
    {

        
        if (listLength < 3)
        {
            FinalScoreBox fsb = loadedFinalScoreBox.Instance() as FinalScoreBox;
            fsb.Init(podium, entity);
            n2d.AddChild(fsb, true);

            fsb.RectPosition = new Vector2(0, listLength * 192);
            listLength++;
        }
        else//Not in top 3 displayed
    {
            LoserScoreBox lsb = loadedLoserScoreBox.Instance() as LoserScoreBox;
            lsb.Init(podium, entity);
            loserContainer.AddChild(lsb,true);
        }
    }

    public override void _Ready()
    {
        n2d = this.GetNode<Node2D>("Node2D");
        container = this.GetNode<Control>("Node2D/EndScreen");
        Tween tween = this.GetNode<Tween>("Node2D/EndScreen/Tween");
        ColorRect cr = this.GetNode<ColorRect>("Node2D/ColorRect");


        tween.InterpolateProperty(n2d, "position",new Vector2(0,-576),Vector2.Zero,5f,
            Tween.TransitionType.Elastic,Tween.EaseType.Out);
        tween.Start();

        tween.InterpolateProperty(cr, "color",cr.Color, Color.Color8(0,0,0,128), 5f,
            Tween.TransitionType.Sine, Tween.EaseType.Out);
        tween.Start();
        //DEBUG #############################################
        if (players == null)
        {
            players = new Entity[] {    new Pirate(), new Blahaj(), new Monstropis(),
                                        new Pirate(), new Blahaj(), new Monstropis(), 
                                        new Pirate(), new Blahaj(), new Monstropis() };

            for(int ii = 0; ii < players.Length; ii++)
        {
                players[ii].DebugSetNameTag("Dbg wnr" + (ii + 1));
                players[ii].score = (short)(10 - ii);
            }
           
        }
        
        //DEBUG #############################################

        loserContainer = this.GetNode("Node2D/EndScreen/LoserList/VBoxContainer") as VBoxContainer;

        SetCanvas(1, players[0]);//Sets first player's banner

        byte podium = 1;
        for(byte i = 1; (i < players.Length) ; i++)
        {
            if ((podium != 4) && (players[i].score != players[i - 1].score)) {
                podium = (byte)(i + 1);
                if (podium > 4) podium = 4;
                    }

            SetCanvas(podium, players[i]);
        }

     


    }


}
