using Godot;
using System;
using System.Collections.Generic;

public class Attack : Node2D
{
    //TECHNICAL
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected byte currentBeat = 0;
    protected byte maxBeat;
    protected bool firstBeatWasCalled = false;
    protected List<List<short[]>> packagedAtkData;
    protected List<short> keyChain = new List<short>() { 0 };

    protected Vector2 gridPos;

    private const byte X = 0;
    private const byte Y = 1;
    private const byte DAMAGE = 2;
    private const byte LOCK = 3;
    private const byte KEY = 4;
    private const byte ANIM = 5;
    private const byte STATUS = 6;

    protected float timing = 0f;

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //TECHNICAL

    //DEPENDANCIES
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected Entity source;
    protected Level level;

    protected float damageCorrection = 1f;

    protected String folderPath;
    protected String beatAnimPath;

    protected PackedScene damageTileScene = GD.Load("res://Abstract/DamageTile.tscn") as PackedScene;

    public Dictionary<Vector2, DamageTile> posToTiles = new Dictionary<Vector2, DamageTile>();

    public Entity GetSource() { return source; }
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //DEPENDANCIES

    //ANIMATIONS
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected bool flipableAnims = false;
    protected byte[] animations;
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //ANIMATIONS
    public void InitAtk(Entity attacker ,List<List<short[]>> atkData ,Level map,String path ,byte[] collumns ,bool flipable , float time)
    {
        this.source = attacker;
        this.packagedAtkData = atkData;
        this.level = map;
        this.folderPath = path;
        this.animations = collumns;
        this.flipableAnims = flipable;
        
        this.maxBeat = (byte)atkData.Count;

        this.gridPos = source.pos;

        this.Position = (source.pos * 64) + new Vector2(32,16);

        this.timing = time;
    }

    private float FixFirstBeatPlaySpeed(float timing)//Timing is how early relative to beat update the attack was instanced
    {
        { 
        /*
        * If timing == 0    : duration = 0.33 ; ratio : 1
        * If timing == 0.33 : duration = 0.66 ; ratio : 2
        * If timing == 0.66 : duration = 1.00 ; ratio : 3
        *
        * If timing == 0    : playspeed = 1/1
        * If timing == 0.33 : playspeed = 1/2
        * If timing == 0.66 : playspeed = 1/3 
        */
        }
        if (maxBeat == 1) return (1f / (1f + (timing / 0.33f)));
        {
        /*
        * If timing == 0    : duration = 0.66 ; ratio : 1
        * If timing == 0.33 : duration = 1.00 ; ratio : 1.5
        * If timing == 0.66 : duration = 1.33 ; ratio : 2
        *
        * If timing == 0    : playspeed = 1/1
        * If timing == 0.33 : playspeed = 1/1.5
        * If timing == 0.66 : playspeed = 1/2 
        */
        }
        return (1f / (1f + (timing / 0.66f)));
    }
    public override void _Ready()
    {
        BeatAtkUpdate();
        damageCorrection = (float) (/**/((0.3f / (Math.Pow(Math.Abs(timing - 0.475), 1.6f) + 0.3f)) + (0.3f / (Math.Pow(Math.Abs(timing - 0.625f), 1.6f) + 0.3f))/2f)/**/);

        GD.Print("[Attack] DamageMultiplier is " + damageCorrection + " Divider was " + (Math.Pow(Math.Abs(timing - 0.55), 1.6) + 0.3) + " Timing was " + timing);
    }

    private void BeatAtkUpdate()
    {
        if (!firstBeatWasCalled)
        {
            firstBeatWasCalled = true;
            
            beatAnimPath = folderPath + "F" + 1 + ".png";
            SpriteFrames textureAnime = LoadSpriteSheet(0);

            List<short[]> frameAtkData = packagedAtkData[0];
            for (int tile = 0; tile < frameAtkData.Count; tile++)
            {
                short[] currentTile = frameAtkData[tile];//Select one tile at a time

                if (keyChain.Contains(currentTile[LOCK]))
                {                   
                    if (level.GetCell((int)(gridPos.x + currentTile[X]), (int)(gridPos.y + currentTile[Y])) != 2)//checks for wall
                    {
                        CreateDamageTile(textureAnime,
                                         new Vector2((gridPos.x + currentTile[X]), (gridPos.y + currentTile[Y])),
                                         currentTile[ANIM],
                                         currentTile[DAMAGE],
                                         currentTile[STATUS],
                                         damageCorrection,
                                         FixFirstBeatPlaySpeed(timing));

                        if (!keyChain.Contains(currentTile[KEY])) keyChain.Add(currentTile[KEY]);
                    }
                }
            }
        }
        else
        {
            currentBeat++;
            if (currentBeat == 1) return;
            if (currentBeat > maxBeat)
            {
                this.QueueFree();
                return;
            }

            beatAnimPath = folderPath + "F" + currentBeat + ".png";
            var textureAnime = LoadSpriteSheet((byte)(currentBeat - 1));

            var frameAtkData = packagedAtkData[currentBeat - 1];
            for (int tile = 0; tile < frameAtkData.Count; tile++)
            {
                var currentTile = frameAtkData[tile];//Select one tile at a time

                if (keyChain.Contains(currentTile[LOCK]))
                {
                    if (level.GetCell((int)(gridPos.x + currentTile[X]), (int)(gridPos.y + currentTile[Y])) != 2)//checks for wall
                    {
                        CreateDamageTile(textureAnime,
                                         new Vector2((gridPos.x + currentTile[X]), (gridPos.y + currentTile[Y])),
                                         currentTile[ANIM],
                                         currentTile[DAMAGE],
                                         currentTile[STATUS],
                                         damageCorrection);

                        if (!keyChain.Contains(currentTile[KEY]))
                        {

                            keyChain.Add(currentTile[KEY]);
                        }
                    }//Check for walls
                }//Check for LOCK
            }//Loops through every tiles in frameAtkData
        }//if else first beat
        UpdateDamageTiles();
        level.AttackCheckForEntity(this);
    }
    //DAMAGETILES
    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    protected void CreateDamageTile(SpriteFrames texture, Vector2 tilePos, short anim, short damage,short statusEffect,float dmgMultiplier,float playSpeed)
    {
        damage = (short)(damage * dmgMultiplier);

        DamageTile instancedDTS = damageTileScene.Instance() as DamageTile;
        Vector2 tile = tilePos - gridPos;
        
        bool flippableX = false, flippableY = false;
        if (flipableAnims){ if (tile.x < 0) flippableX = true; if (tile.y < 0) flippableY = true; }

        instancedDTS.InitDamageTile(this,
                                    tilePos,                //Coordinates
                                    "c" + anim, texture,    //Animation texture
                                    damage, statusEffect,   //Damage
                                    flippableX, flippableY, //Animation flip
                                    playSpeed);             //Animation speed : VARIABLE (FIRST BEAT ONLY)

        this.AddChild(instancedDTS, true);
        
        instancedDTS.Position += (tilePos - gridPos) * 64;
    }
    protected void CreateDamageTile(SpriteFrames texture, Vector2 tilePos, short anim, short damage, short statusEffect, float dmgMultiplier)
    {
        damage = (short)(damage * dmgMultiplier);

        DamageTile instancedDTS = damageTileScene.Instance() as DamageTile;
        Vector2 tile = tilePos - gridPos;

        bool flippableX = false, flippableY = false;
        if (flipableAnims) { if (tile.x < 0) flippableX = true; if (tile.y < 0) flippableY = true; }

        instancedDTS.InitDamageTile(this,
                                    tilePos,                //Coordinates
                                    "c" + anim, texture,    //Animation texture
                                    damage, statusEffect,   //Damage
                                    flippableX, flippableY, //Animation flip
                                    1f);                    //Animation speed : ALWAYS 1 (NEVER ON FIRST BEAT)

        this.AddChild(instancedDTS, true);

        instancedDTS.Position += (tilePos - gridPos) * 64;
    }

    protected SpriteFrames LoadSpriteSheet(byte sheet)
    {
        SpriteFrames sf = new SpriteFrames();
        byte rows = 20;
        if (sheet == (maxBeat - 1)) rows = 10;
       
        Texture spriteSheet = GD.Load(beatAnimPath) as Texture;
        
        //File not found
        if (spriteSheet == null)
        {
           GD.Print("[Attack] Can't find texture : " + beatAnimPath);
            spriteSheet = GD.Load("res://Entities/Default.png") as Texture;
            
        }
        //File not found

        for (byte col = 0; col < animations[sheet]; col++)
        {
            //create animations
            sf.AddAnimation("c" + col);
            sf.SetAnimationSpeed("c" + col,30);
            sf.SetAnimationLoop("c" + col, false);

            
            for (byte r = 0; r < rows; r++)
            {
                AtlasTexture atlas = new AtlasTexture();
                atlas.Atlas = spriteSheet;
                atlas.Region = new Rect2(new Vector2(col*64,r*64), new Vector2(64,64));
                sf.AddFrame("c" + col, atlas, r);
            }

        }

        return sf;
    }

    protected void UpdateDamageTiles()
    {
        int numberOfTiles = this.GetChildCount();
        for (int i = 0; i < numberOfTiles; i++)
        {
            DamageTile dt = this.GetChild(i) as DamageTile;
            if (dt == null) continue;

            if (posToTiles.ContainsKey(dt.GetCoords()))
            {
                if(dt.GetDamage() > posToTiles[dt.GetCoords()].GetDamage())
                {
                    posToTiles[dt.GetCoords()] = dt;
                }
            }
            else
            {
                posToTiles.Add(dt.GetCoords(), dt);
            }

        }
    }
    public void RemoveDamageTiles(DamageTile removedTile)
    {
        if(posToTiles.ContainsKey(removedTile.GetCoords()))
        {
            if (posToTiles[removedTile.GetCoords()] == removedTile)
                posToTiles.Remove(removedTile.GetCoords());
            //Might create ghostTiles (unlikely)
        }

    }

    //*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*\\
    //DAMAGETILES
}
