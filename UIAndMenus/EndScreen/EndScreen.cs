using Godot;
using System;

public class EndScreen : Control
{
    byte listLength = 0;

    [Export]
    PackedScene loadedFinalScoreBox;
    [Export]
    PackedScene loadedLoserScoreBox;
    [Export]
    Entity[] players;

    [Export]
    VBoxContainer loserContainer;

    


    public void Init(Entity[] allPlayers)
    {
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
            this.AddChild(fsb, true);

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

        

        //DEBUG #############################################
        /*if (players == null)
        {
            players = new Entity[] {    new Pirate(), new Blahaj(), new Monstropis(),
                                        new Pirate(), new Blahaj(), new Monstropis(), 
                                        new Pirate(), new Blahaj(), new Monstropis() };

            for(int ii = 0; ii < players.Length; ii++)
            {
                players[ii].DebugSetNameTag("Dbg wnr" + (ii + 1));
                
            }
           
        }
        */
        //DEBUG #############################################

        loserContainer = this.GetNode("VBoxContainer") as VBoxContainer;

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
