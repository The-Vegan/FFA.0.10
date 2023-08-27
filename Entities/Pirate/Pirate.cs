using Godot;
using System;
using System.Collections.Generic;

public class Pirate : Entity
{
    /// A FAIRE
    /// RESOUDRE BUG DE ATKMOVE
    /// 
    /// A FAIRE
    private List<List<short[]>> DOWNMOVEATK = new List<List<short[]>>();
    private List<List<short[]>> LEFTMOVEATK = new List<List<short[]>>();
    private List<List<short[]>> RIGHTMOVEATK = new List<List<short[]>>();
    private List<List<short[]>> UPMOVEATK = new List<List<short[]>>();

    protected bool reallyMoved = false;
    public override void _Ready()
    {

        base._Ready();

        this.atkFolder = "res://Entities/Pirate/atk/";

        this.maxHP = 165;



        this.animPerBeat = new byte[] { 5 };
        {//Declare downAtkTiles

            List<short[]> frame1 = new List<short[]>
            {  //              X , Y , Dmg,Lck,Key,Anm,Sta
                new short[] { -1 , 1 , 42 , 0 , 0 , 0 , 0b0_001_000_000_000_000 },
                new short[] {  0 , 1 , 42 , 0 , 0 , 1 , 0b0_001_000_000_000_000 },
                new short[] {  1 , 1 , 42 , 0 , 0 , 2 , 0b0_001_000_000_000_000 }
            };

            DOWNATK.Add(frame1);
        }//Declare downAtkTiles
        {//Declare leftAtkTiles

            List<short[]> frame1 = new List<short[]>
            {   //              X ,  Y , Dmg,Lck,Key,Anm,Sta
                new short[] {  -1 , -1 , 42 , 0 , 0 , 0 , 0b0_001_000_000_000_000 },
                new short[] {  -1 ,  0 , 42 , 0 , 0 , 1 , 0b0_001_000_000_000_000 },
                new short[] {  -1 ,  1 , 42 , 0 , 0 , 2 , 0b0_001_000_000_000_000 }
            };

            LEFTATK.Add(frame1);
        }//Declare leftAtkTiles
        {//Declare rightAtkTiles

            List<short[]> frame1 = new List<short[]>
            {
                //              X ,  Y , Dmg,Lck,Key,Anm,Sta
                new short[] {  1 ,  1 , 42 , 0 , 0 , 0 , 0b0_001_000_000_000_000 },
                new short[] {  1 ,  0 , 42 , 0 , 0 , 1 , 0b0_001_000_000_000_000 },
                new short[] {  1 , -1 , 42 , 0 , 0 , 2 , 0b0_001_000_000_000_000 }
            };


            RIGHTATK.Add(frame1);
        }//Declare rightAtkTiles
        {//Declare upAtkTiles

            List<short[]> frame1 = new List<short[]>
            {
                new short[]
            {  1 , -1 , 42 , 0 , 0 , 0 , 0b0_001_000_000_000_000},
                new short[]
            {  0 , -1 , 42 , 0 , 0 , 1 , 0b0_001_000_000_000_000},
                new short[]
            {  -1 , -1 , 42 , 0 , 0 , 2 , 0b0_001_000_000_000_000}
};

            UPATK.Add(frame1);
        }//Declare upAtkTiles
        {//Declare downMoveAtkTiles

            List<short[]> frame1 = new List<short[]>
            {
                new short[]
            { -1 , 0 , 42 , 0 , 0 , 0 , 0b0_001_000_000_000_000},
                new short[]
            { -1 , 1 , 42 , 0 , 0 , 1 , 0b0_001_000_000_000_000},
                new short[]
            {  0 , 1 , 42 , 0 , 0 , 2 , 0b0_001_000_000_000_000},
                new short[]
            {  1 , 1 , 42 , 0 , 0 , 3 , 0b0_001_000_000_000_000},
                new short[]
            {  1 , 0 , 42 , 0 , 0 , 4 , 0b0_001_000_000_000_000}
            };

            DOWNMOVEATK.Add(frame1);
        }//Declare downMoveAtkTiles
        {//Declare leftMoveAtkTiles

            List<short[]> frame1 = new List<short[]>
            {
                new short[]
            {  0 ,-1 , 42 , 0 , 0 , 0 , 0b0_001_000_000_000_000},
                new short[]
            { -1 ,-1 , 42 , 0 , 0 , 1 , 0b0_001_000_000_000_000},
                new short[]
            { -1 , 0 , 42 , 0 , 0 , 2 , 0b0_001_000_000_000_000},
                new short[]
            { -1 , 1 , 42 , 0 , 0 , 3 , 0b0_001_000_000_000_000},
                new short[]
            {  0 , 1 , 42 , 0 , 0 , 4 , 0b0_001_000_000_000_000}
            };

            LEFTMOVEATK.Add(frame1);
        }//Declare leftMoveAtkTiles
        {//Declare rightMoveAtkTiles

            List<short[]> frame1 = new List<short[]>
            {
                new short[]
            {  0 , 1 , 42 , 0 , 0 , 0 , 0b0_001_000_000_000_000},
                new short[]
            {  1 , 1 , 42 , 0 , 0 , 1 , 0b0_001_000_000_000_000},
                new short[]
            {  1 , 0 , 42 , 0 , 0 , 2 , 0b0_001_000_000_000_000},
                new short[]
            {  1 ,-1 , 42 , 0 , 0 , 3 , 0b0_001_000_000_000_000},
                new short[]
            {  0 ,-1 , 42 , 0 , 0 , 4 , 0b0_001_000_000_000_000}
            };

            RIGHTMOVEATK.Add(frame1);

        }//Declare rightMoveAtkTiles
        {//Declare upMoveAtkTiles

            List<short[]> frame1 = new List<short[]>
            {
                new short[]
            {  1 , 0 , 42 , 0 , 0 , 0 , 0b0_001_000_000_000_000},
                new short[]
            {  1 ,-1 , 42 , 0 , 0 , 1 , 0b0_001_000_000_000_000},
                new short[]
            {  0 ,-1 , 42 , 0 , 0 , 2 , 0b0_001_000_000_000_000},
                new short[]
            { -1 ,-1 , 42 , 0 , 0 , 3 , 0b0_001_000_000_000_000},
                new short[]
            { -1 , 0 , 42 , 0 , 0 , 4 , 0b0_001_000_000_000_000}
            };

            UPMOVEATK.Add(frame1);
        }//Declare upMoveAtkTiles


    }

    protected override short PacketParser(short packetToParse)
    {
        if ((packetToParse >> 4) == (packetToParse & 0b1111))//move and Attack At the same time (no rest/items)
        {
            GD.Print("[Pirate] DashAtk");

            short parsedPacket = 0;
            if ((packetToParse & 0b0001) != 0) parsedPacket = 17;
            else if ((packetToParse & 0b0010) != 0) parsedPacket = 34;
            else if ((packetToParse & 0b0100) != 0) parsedPacket = 68; //THIS ONE
            else if ((packetToParse & 0b1000) != 0) parsedPacket = 136;

            return parsedPacket;
        }

        return base.PacketParser(packetToParse);
    }

    public override void Moved(Vector2 newTile)
    {
        GD.Print("[Pirate] Moved");
        if (pos == newTile)
        {
            reallyMoved = false;
            return;
        }
        else reallyMoved = true;

        map.SetCell((int)pos.x, (int)pos.y, 0);

        pos = newTile;
        map.SetCell((int)pos.x, (int)pos.y, 3);

        tween.InterpolateProperty(this, "position",                          //Property to interpolate
            this.Position, new Vector2((pos.x * 64) + 32, (pos.y * 64) + 16),//initVal,FinalVal
            0.33f,                                                           //Duration
            Tween.TransitionType.Sine, Tween.EaseType.Out);                  //Tween says Trans rights
        tween.Start();
        animPlayer.Play("Move");
        this.ForcePlay(action + direction, map.GetTime());
    }

    protected override void AskAtk()
    {
        if ((packet & 0b1111_0000) == 0) return;
        action = "Atk";
        cooldown = (byte)(ATKCOOLDOWN + 1);
        if (((packet >> 4) == (packet & 0b1111)) && (reallyMoved))//if dash-atk
        {

            if (packet == 0b0001_0001) map.CreateAtk(this, DOWNMOVEATK, atkFolder + "DownMoveAtk", animPerBeat, flippableAnim);
            else if (packet == 0b0010_0010) map.CreateAtk(this, LEFTMOVEATK, atkFolder + "LeftMoveAtk", animPerBeat, flippableAnim);
            else if (packet == 0b0100_0100) map.CreateAtk(this, RIGHTMOVEATK, atkFolder + "RightMoveAtk", animPerBeat, flippableAnim);
            else if (packet == 0b1000_1000) map.CreateAtk(this, UPMOVEATK, atkFolder + "UpMoveAtk", animPerBeat, flippableAnim);

            return;
        }

        if ((packet & 0b0001_0000) != 0) map.CreateAtk(this, DOWNATK, atkFolder + "DownAtk", animPerBeat, flippableAnim);
        else if ((packet & 0b0010_0000) != 0) map.CreateAtk(this, LEFTATK, atkFolder + "LeftAtk", animPerBeat, flippableAnim);
        else if ((packet & 0b0100_0000) != 0) map.CreateAtk(this, RIGHTATK, atkFolder + "RightAtk", animPerBeat, flippableAnim);
        else if ((packet & 0b1000_0000) != 0) map.CreateAtk(this, UPATK, atkFolder + "UpAtk", animPerBeat, flippableAnim);
    }
    public override void SetPacketAsync(short p)
    {

        if (cooldown != 0) return;
        if (stun != 0) return;
        packet |= p;
        GD.Print("[Pirate] Packet Set");
        new System.Threading.Thread(async delegate ()
        {

            for (byte i = 0; i < global.bufferFrameInput; i++) await ToSignal(GetTree(), "idle_frame");

            if (actedThisBeat) return;


            actedThisBeat = true;
            GD.Print("[Pirate] Packet Locked");
            packet = PacketParser(packet);
            timing = map.GetTime();

            DirectionSetter();
            if ((packet & 0b1111_0000) != 0 && (packet & 0b1111) != 0)//if dashAtk
            {
                EmitSignal("noteHiter", (byte)(ATKCOOLDOWN + 1), true);

                AskMovement();
                AskAtk();

            }
            else if ((packet & 0b1111_0000) != 0)//If it's an attack
            {
                EmitSignal("noteHiter", (byte)(ATKCOOLDOWN + 1), true);

                AskAtk();
            }
            else if ((packet & 0b1111) != 0)
            {
                EmitSignal("noteHiter", 1, true);

                AskMovement();
            }

            this.Play(action + direction);
        }).Start();
    }
}
