using Godot;
using System;
using System.Collections.Generic;

public class Blahaj : Entity
{

    public override void _Ready()
    {
        base._Ready();

        this.maxHP = 200;

        this.flippableAnim = true;
        this.atkFolder = "res://Entities/Blahaj/atk/";

        animPerBeat = new byte[]{4,5,3};

        DOWNATK = new List<List<short[]>>
        {
            new List<short[]>   //PHASE 1 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] {  0 , 1 ,54 , 0 , 1 , 0 , 0 },
                new short[] {  0 , 2 , 0 , 1 , 2 , 2 , 0 },
                new short[] {  1 , 1 , 0 , 1 , 3 , 1 , 0 },
                new short[] { -1 , 1 , 0 , 1 , 4 , 1 , 0 }
            },                  //PHASE 1 :::::::::::::::::::::::::::::::::::::::
            new List<short[]>   //PHASE 2 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] {  0 , 2 ,27 , 2 , 5 , 0 , 0 },

                new short[] {  0 , 3 , 0 , 5 , 8 , 4 , 0 },
                new short[] {  1 , 2 , 0 , 5 , 9 , 3 , 0 },
                new short[] { -1 , 2 , 0 , 5 ,10 , 3 , 0 },

                new short[] {  1 , 1 , 27 , 3 , 6 , 1 , 0 },

                new short[] {  1 , 2 , 0 , 6 , 9 , 3 , 0 },
                new short[] {  2 , 1 , 0 , 6 ,11 , 2 , 0 },


                new short[] { -1 , 1 ,27 , 4 , 7 , 1 , 0 },

                new short[] { -1 , 2 , 0 , 7 ,10 , 3 , 0 },
                new short[] { -2 , 1 , 0 , 7 ,12 , 2 , 0 },
            },                  //PHASE 2 :::::::::::::::::::::::::::::::::::::::
            new List<short[]>   //PHASE 3 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] { 2 , 1 , 12 ,11 , 0 , 2 , 0 },
                new short[] { 1 , 2 , 15 , 9 , 0 , 1 , 0 },
                new short[] { 0 , 3 , 12 , 8 , 0 , 0 , 0 },
                new short[] {-1 , 2 , 15 ,10 , 0 , 1 , 0 },
                new short[] {-2 , 1 , 12 ,12 , 0 , 2 , 0 }
            }                   //PHASE 3 :::::::::::::::::::::::::::::::::::::::
        };

        LEFTATK = new List<List<short[]>>
        {
            new List<short[]>   //PHASE 1 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] { -1 , 0 ,54 , 0 , 1 , 0 , 0 },
                new short[] { -2 , 0 , 0 , 1 , 2 , 2, 0 },
                new short[] { -1 , 1 , 0 , 1 , 3 , 1 , 0 },
                new short[] { -1 ,-1 , 0 , 1 , 4 , 1 , 0 }
            },                  //PHASE 1 :::::::::::::::::::::::::::::::::::::::
            new List<short[]>   //PHASE 2 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] { -2 , 0 , 27 , 2 , 5 , 0 , 0 },

                new short[] { -3 , 0 , 0 , 5 , 8 , 4 , 0 },
                new short[] { -2 , 1 , 0 , 5 , 9 , 3 , 0 },
                new short[] { -2 ,-1 , 0 , 5 ,10 , 3 , 0 },


                new short[] { -1 , 1 , 27 , 3 , 6 , 1 , 0 },

                new short[] { -2 , 1 , 0 , 6 , 9 , 3 , 0 },
                new short[] { -1 , 2 , 0 , 6 ,11 , 2 , 0 },


                new short[] { -1 ,-1 , 27 , 4 , 7 , 1 , 0 },

                new short[] { -2 ,-1 , 0 , 7 ,10 , 3 , 0 },
                new short[] { -1 ,-2 , 0 , 7 ,12 , 2 , 0 },


            },                  //PHASE 2 :::::::::::::::::::::::::::::::::::::::
            new List<short[]>   //PHASE 3 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] { -1 , 2 ,12 ,11 , 0 , 2 , 0 },
                new short[] { -2 , 1 ,15 , 9 , 0 , 1 , 0 },
                new short[] { -3 , 0 ,12 , 8 , 0 , 0 , 0 },
                new short[] { -2 ,-1 ,15 ,10 , 0 , 1 , 0 },
                new short[] { -1 ,-2 ,12 ,12 , 0 , 2 , 0 }
            }                   //PHASE 3 :::::::::::::::::::::::::::::::::::::::


        };

        RIGHTATK = new List<List<short[]>>
        {
            new List<short[]> //PHASE 1 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] {  1 , 0 , 54 , 0 , 1 , 0 , 0 },
                new short[] {  2 , 0 , 0 , 1 , 2 , 2 , 0 },
                new short[] {  1 , 1 , 0 , 1 , 3 , 1 , 0 },
                new short[] {  1 ,-1 , 0 , 1 , 4 , 1 , 0 }
            },                                  //PHASE 1 :::::::::::::::::::::::::::::::::::::::
            new List<short[]> //PHASE 2 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] {  2 , 0 , 27 , 2 , 5 , 0 , 0 },

                new short[] {  3 , 0 , 0 , 5 , 8 , 4 , 0 },
                new short[] {  2 , 1 , 0 , 5 , 9 , 3 , 0 },
                new short[] {  2 ,-1 , 0 , 5 ,10 , 3 , 0 },

                new short[] {  1 , 1 , 27 , 3 , 6 , 1 , 0 },

                new short[] {  2 , 1 , 0 , 6 , 9 , 3 , 0 },
                new short[] {  1 , 2 , 0 , 6 ,11 , 2 , 0 },


                new short[] {  1 ,-1 , 27 , 4 , 7 , 1 , 0 },

                new short[] {  2 ,-1 , 0 , 7 ,10 , 3 , 0 },
                new short[] {  1 ,-2 , 0 , 7 ,12 , 2 , 0 },


            },                                  //PHASE 2 :::::::::::::::::::::::::::::::::::::::
            new List<short[]> //PHASE 3 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] {  1 , 2 , 12 ,11 , 0 , 2 , 0 },
                new short[] {  2 , 1 , 15 , 9 , 0 , 1 , 0 },
                new short[] {  3 , 0 , 12 , 8 , 0 , 0 , 0 },
                new short[] {  2 ,-1 , 15 ,10 , 0 , 1 , 0 },
                new short[] {  1 ,-2 , 12 ,12 , 0 , 2 , 0 }
            }                                   //PHASE 3 :::::::::::::::::::::::::::::::::::::::


        };


        UPATK = new List<List<short[]>>
        {
            new List<short[]> //PHASE 1 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] {  0 ,-1 , 54 , 0 , 1 , 0 , 0 },
                new short[] {  0 ,-2 , 0 , 1 , 2 , 2 , 0 },
                new short[] {  1 ,-1 , 0 , 1 , 3 , 1 , 0 },
                new short[] { -1 ,-1 , 0 , 1 , 4 , 1 , 0 }
            },                                  //PHASE 1 :::::::::::::::::::::::::::::::::::::::
            new List<short[]> //PHASE 2 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] {  0 ,-2 , 27 , 2 , 5 , 0 , 0 },

                new short[] {  0 ,-3 , 0 , 5 , 8 , 4 , 0 },
                new short[] {  1 ,-2 , 0 , 5 , 9 , 3 , 0 },
                new short[] { -1 ,-2 , 0 , 5 ,10 , 3 , 0 },

                new short[] {  1 ,-1 , 27 , 3 , 6 , 1 , 0 },

                new short[] {  1 ,-2 , 0 , 6 , 9 , 3 , 0 },
                new short[] {  2 ,-1 , 0 , 6 ,11 , 2 , 0 },


                new short[] { -1 ,-1 , 27 , 4 , 7 , 1 , 0 },

                new short[] { -1 ,-2 , 0 , 7 ,10 , 3 , 0 },
                new short[] { -2 ,-1 , 0 , 7 ,12 , 2 , 0 },


            },                                  //PHASE 2 :::::::::::::::::::::::::::::::::::::::
            new List<short[]> //PHASE 3 :::::::::::::::::::::::::::::::::::::::
            {
                new short[] {  2 ,-1 , 12 ,11 , 0 , 2 , 0 },
                new short[] {  1 ,-2 , 15 , 9 , 0 , 1 , 0 },
                new short[] {  0 ,-3 , 12 , 8 , 0 , 0 , 0 },
                new short[] { -1 ,-2 , 15 ,10 , 0 , 1 , 0 },
                new short[] { -2 ,-1 , 12 ,12 , 0 , 2 , 0 }
            }                                   //PHASE 3 :::::::::::::::::::::::::::::::::::::::


        };

    }//Ready

    protected override void AskAtk()
    {
        if ((packet & 0b1111_0000) == 0) return;
        action = "Atk";
        cooldown = ATKCOOLDOWN;
        if ((packet & 0b0001_0000) != 0) map.CreateAtk(this, DOWNATK, downAtkAnimIDs, animPerBeat, flippableAnim);
        else if ((packet & 0b0010_0000) != 0) map.CreateAtk(this, LEFTATK,leftAtkAnimIDs, animPerBeat, flippableAnim);
        else if ((packet & 0b0100_0000) != 0) map.CreateAtk(this, RIGHTATK,leftAtkAnimIDs, animPerBeat, flippableAnim);
        else if ((packet & 0b1000_0000) != 0) map.CreateAtk(this, UPATK, downAtkAnimIDs, animPerBeat, flippableAnim);
    }

    protected override void LoadAllTextures()
    {
        new System.Threading.Thread(delegate ()
        {
            System.Threading.Thread.Sleep(this.id * id * 2);
            downAtkAnimIDs = new ushort[]
            {
                global.gMap.LoadTexture(atkFolder + "VF1.png"),
                global.gMap.LoadTexture(atkFolder + "VF2.png"),
                global.gMap.LoadTexture(atkFolder + "VF3.png")
            };

            leftAtkAnimIDs = new ushort[]
            {
                global.gMap.LoadTexture(atkFolder + "HF1.png"),
                global.gMap.LoadTexture(atkFolder + "HF2.png"),
                global.gMap.LoadTexture(atkFolder + "HF3.png")
            };
        }).Start();
    }
}
