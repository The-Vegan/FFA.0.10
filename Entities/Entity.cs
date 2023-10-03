using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class Entity : AnimatedSprite
{
    //DEPENDENCIES
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected Global global;

    protected Level map;
    protected Tween tween;
    protected AnimationPlayer animPlayer;
    public DamagePlayer damagePlayer;

    public byte team = 0;
    public byte id;

    protected String nameTag;
    public String GetNametag() { return nameTag; }

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //DEPENDENCIES

    //MOVEMENT RELATED
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected byte ATKCOOLDOWN = 1;
    public float timing = -1;

    public short packet;
    public Vector2 pos = new Vector2(-1, -1);
    //public Vector2 prevPos;

    protected byte stun = 0, cooldown = 6, respawnCooldown = 0;
    protected bool isDead = false;

    public bool actedThisBeat = false;

    [Signal]
    public delegate void noteHiter(byte nmbrOfNotes, bool volontary);

    protected short perfectBeats = 0;
    protected short missedBeats = 0;

    public short GetPerBeat() { return perfectBeats; }
    public short GetMisBeat() { return missedBeats; }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //MOVEMENT RELATED

    //ATTACK VARIABLES
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected short healthPoint;
    protected short maxHP;

    public short GetHealthPoint() { return healthPoint; }

    protected bool isValidTarget = true;//Clones and stuff don't give you points for hitting them

    protected short blunderBar = 0;
    protected short itemBar = 0;
    public byte itemID = 0;
    public sbyte GetBlunder() { return (sbyte)blunderBar; }
    public sbyte GetItembar() { return (sbyte)itemBar; }
    protected List<Attack> damagedBy;

    protected String atkFolder;
    protected byte[] animPerBeat;
    protected bool flippableAnim = false;

    protected ushort[] downAtkAnimIDs;
    protected ushort[] leftAtkAnimIDs;
    protected ushort[] rightAtkAnimIDs;
    protected ushort[] upAtkAnimIDs;

    protected List<List<short[]>> DOWNATK = new List<List<short[]>>();
    protected List<List<short[]>> LEFTATK = new List<List<short[]>>();
    protected List<List<short[]>> RIGHTATK = new List<List<short[]>>();
    protected List<List<short[]>> UPATK = new List<List<short[]>>();

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ATTACK VARIABLES

    //ANIMATION
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected bool midBeatInterrupted = false;
    protected String direction = "Down";
    protected String action = "Idle";
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ANIMATION

    //CTF
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    public sbyte heldFlag = -1;

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //CTF

    public void Init(Level level, String name, byte givenID)
    {
        this.map = level;
        this.nameTag = name;
        this.id = givenID;

    }
    public void Init(Level level, byte givenID)
    {
        this.map = level;
        this.nameTag = "";
        this.id = givenID;
    }
    public override void _Ready()
    {
        global = GetTree().Root.GetNode<Global>("Global");


        animPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
        damagePlayer = this.GetNode<DamagePlayer>("Node2D/DamagePlayer");

        tween = this.GetNode<Tween>("Tween");

        //Checking twice is faster
        if (global.hasServer) LoadAllTextures();
        else if (!global.isMultiplayer) LoadAllTextures();
    }


    //GESTION DES INPUTS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\

    public virtual void SetPacketAsync(short p)
    {
        if (actedThisBeat) return;
        if (cooldown != 0) return;
        if (stun != 0) return;
        packet |= p;
        GD.Print("[Entity] Packet Set");
        new System.Threading.Thread(async delegate ()
       {
           for (byte i = 0; i < global.bufferFrameInput; i++) await ToSignal(GetTree(), "idle_frame");



           if (actedThisBeat) return;
           actedThisBeat = true;
           
           packet = PacketParser(packet);
           timing = map.GetTime();
            //GD.Print("[Entity] Timing is : " + (timing - (1f / 6f)));

            DirectionSetter();

           if ((packet & 0b1111) != 0)//If it's a movement
            {
               EmitSignal("noteHiter", 1, true);

               AskMovement();
           }
           else if ((packet & 0b1111_0000) != 0)//If it's an attack
            {
               EmitSignal("noteHiter", (byte)(ATKCOOLDOWN + 1), true);

               AskAtk();
           }

       }).Start();
    }

    protected virtual short PacketParser(short packetToParse)
    {
        short parsedPacket = 0;

        if ((packetToParse & 0b1111_0000) != 0)//Is it an attack
        {
            if ((packetToParse & 0b0001_0000) != 0) parsedPacket = 16;
            else if ((packetToParse & 0b0010_0000) != 0) parsedPacket = 32;
            else if ((packetToParse & 0b0100_0000) != 0) parsedPacket = 64;
            else if ((packetToParse & 0b1000_0000) != 0) parsedPacket = 128;
        }
        else
        {
            if ((packetToParse & 0b10_0000_0000) != 0) return 512;//Is it rest

            if ((packetToParse & 0b0000_0001) != 0) parsedPacket = 1;       //Is it a movement
            else if ((packetToParse & 0b0000_0010) != 0) parsedPacket = 2;
            else if ((packetToParse & 0b0000_0100) != 0) parsedPacket = 4;
            else if ((packetToParse & 0b0000_1000) != 0) parsedPacket = 8;
        }

        if ((packetToParse & 256) != 0)
        {
            if (parsedPacket >= 16) parsedPacket >>= 4;//offsets attack into move
            parsedPacket |= 256;//enable item flag
        }

        return parsedPacket;
    }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //GESTION DES INPUTS

    //GESTION DES MOUVEMENTS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected virtual void AskMovement()
    {
        //if none of movement bits are set to true
        if ((packet & 0b1111) == 0) return;


        if ((packet & 0b0001) != 0) map.MoveEntity(this, pos + Vector2.Down);
        else if ((packet & 0b0010) != 0) map.MoveEntity(this, pos + Vector2.Left);
        else if ((packet & 0b0100) != 0) map.MoveEntity(this, pos + Vector2.Right);
        else if ((packet & 0b1000) != 0) map.MoveEntity(this, pos + Vector2.Up);
    }

    protected virtual void AskAtk()
    {
        if ((packet & 0b1111_0000) == 0) return;
        action = "Atk";
        cooldown = (byte)(ATKCOOLDOWN + 1);
        if ((packet & 0b0001_0000) != 0) map.CreateAtk(this, DOWNATK, downAtkAnimIDs, animPerBeat, flippableAnim);
        else if ((packet & 0b0010_0000) != 0) map.CreateAtk(this, LEFTATK, leftAtkAnimIDs, animPerBeat, flippableAnim);
        else if ((packet & 0b0100_0000) != 0) map.CreateAtk(this, RIGHTATK, rightAtkAnimIDs, animPerBeat, flippableAnim);
        else if ((packet & 0b1000_0000) != 0) map.CreateAtk(this, UPATK, upAtkAnimIDs, animPerBeat, flippableAnim);
    }

    public virtual void Moved(Vector2 newTile)
    {
        if (pos == newTile) return;

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
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //GESTION DES MOUVEMENTS

    //GESTION DES ANIMATIONS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected async void MidBeatAnimManager()
    {
        await ToSignal(GetTree().CreateTimer(0.33f), "timeout");


        AutoActionSetter();
        if (midBeatInterrupted) return;
        this.SpeedScale = 1;
        this.Play(action + direction);

    }

    protected void AutoActionSetter()
    {
        switch (action)
        {
            case "Idle":
            case "Cooldown":
                action = "Wait";
                break;
            case "Atk":
                action = "Cooldown";
                break;
            case "Wait":
                action = "Idle";
                break;

        }
    }

    protected void DirectionSetter()
    {
        //Can't compare with "0b0001_0001 beacause attack need to take priority on movement
        if ((packet & 0b1111_1111) == 0) return;
        if ((packet & 0b1111_0000) != 0)
        {
            if ((packet & 0b0001_0000) != 0) direction = "Down";
            else if ((packet & 0b0010_0000) != 0) direction = "Left";
            else if ((packet & 0b0100_0000) != 0) direction = "Right";
            else if ((packet & 0b1000_0000) != 0) direction = "Up";
        }
        else if ((packet & 0b1111) != 0)
        {
            if ((packet & 0b0001) != 0) direction = "Down";
            else if ((packet & 0b0010) != 0) direction = "Left";
            else if ((packet & 0b0100) != 0) direction = "Right";
            else if ((packet & 0b1000) != 0) direction = "Up";
        }
    }

    protected async void ForcePlay(String animation, float timing)
    {
        this.Stop();
        this.Frame = 0;

        this.SpeedScale = (1f / (1f + (timing / 0.33f)));

        this.Play(animation);
        await ToSignal(this, "animation_finished");
        midBeatInterrupted = true;

    }
    protected abstract void LoadAllTextures();
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //GESTION DES ANIMATIONS

    //GESTION DES DEGATS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\

    public void Damaged( Attack attack)
    {
        Entity source = attack.GetSource();
        if (source == this) return;
        if (source.team == this.team) return;
        //If damage is 0, this will not be called
        if (!damagedBy.Contains(attack))
        {

            damagedBy.Add(attack);

            short damage = attack.posToTiles[pos].GetDamage();

            healthPoint -= damage;

            damagePlayer.Damage(map.GetTime(), damage);

            if (isValidTarget)
            {
                source.HitSomeone((short)((damage << 2) + 5));
            }

        }
    }
    public void CheckDeath()
    {
        if (healthPoint < 0) Death();
    }
    protected void Death()
    {
        map.SetCell((int)pos.x, (int)pos.y, 0);//Makes tile walkable again

        if (!isValidTarget)
        {
            map.DeleteEntity(this);
            return;
        }


        for (int i = 1; i < damagedBy.Count; i++)
        {
            damagedBy[i].GetSource().HitSomeone((short)(50 / (damagedBy.Count - 1)));//Distributes 50 points between all killers
        }

        pos = Vector2.NegOne;
        this.Visible = false;

        isDead = true;

        respawnCooldown = 6;
    }


    public virtual void HitSomeone(short points)
    {
        this.itemBar += points;
        this.blunderBar -= (short)(points >> 2);

        if (itemBar > 100) itemBar = 100;
        if (blunderBar < 0) blunderBar = 0;
    }

    public void ResetHealth()
    {
        healthPoint = maxHP;
    }

    public void RestoreHealth(short hp)
    {
        this.healthPoint += hp;
        if (healthPoint > maxHP) healthPoint = maxHP;
    }

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //GESTION DES DEGATS
    public void BeatUpdate()
    {
        if (isDead)
        {
            respawnCooldown--;
            if (respawnCooldown <= 0) { isDead = false; map.Spawn(this); ResetBeatValues(); }
            return;
        }

        if (packet == 0) { this.ForcePlay("FailedInput", 0); action = "Idle"; }

        MidBeatAnimManager();
        new System.Threading.Thread(ResetBeatValues).Start();

        if (stun != 0) { cooldown = 0; stun--; return; }
        if (cooldown != 0) { cooldown--; return; }

    }//End of BeatUpdate
    private async void ResetBeatValues()
    {
        await ToSignal(GetTree().CreateTimer(0.03f), "timeout");//Prevents frame1 inputs (bad and glitchy)
        timing = -1;
        packet = 0;
        midBeatInterrupted = false;
        actedThisBeat = false;
        damagedBy = new List<Attack>();

    }

    public void Sync(FFA.Empty.Empty.Network.Client.SyncEntityPacket packet)
    {
        if (pos != packet.position)
        {
            map.SetCell((int)pos.x, (int)pos.y, 0);

            pos = packet.position;
            map.SetCell((int)pos.x, (int)pos.y, 3);

            this.Position = new Vector2((pos.x * 64) + 32, (pos.y * 64) + 16);
        }

        this.healthPoint = packet.health;
        this.blunderBar = packet.blunderbar;
        this.itemBar = packet.itembar;
        this.itemID = packet.heldItemID;
    }
}
