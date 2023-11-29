using Godot;
using System;

public class DamageTile : AnimatedSprite
{
    private Attack source;
    private Vector2 coordinates;
    private String animation;

    private short damage;
    private short status;
    

    public short GetDamage(){return damage;}
    public Vector2 GetCoords(){return coordinates;}
    public short GetStatus(){return status;}

    public void InitDamageTile(Attack attacker,Vector2 pos,String anim,SpriteFrames sf,short punch, short effect,bool flippedX, bool flippedY,float speed)
    {
        

        this.source = attacker;
        this.coordinates = pos;
        this.animation = anim;
        this.FlipH = flippedX;
        this.FlipV = flippedY;
        this.SpeedScale = speed;

#pragma warning disable CS0618 // Type or member is obsolete
        this.SetSpriteFrames(sf);
#pragma warning restore CS0618 // Type or member is obsolete

        this.damage = punch;
        this.status = effect;
    }

    public override void _Ready()
    {
        this.Play(animation);
        //play the animation
    }

    
    public void OnAnimationFinished()//Called through Signal
    {
        this.QueueFree();
    }

}
