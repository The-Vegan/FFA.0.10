using Godot;
using System;

public class DamagePlayer : AnimationPlayer
{
    short displayNumber = 0;
    Label displayLabel;
    public override void _Ready() 
    { 
        displayLabel = this.GetParent().GetNode<Label>("Label");
    }
    public void Damage(float timing,short damage)
    {
        //if (damage == 0) return;
        displayNumber += damage;
        if (displayNumber < 0) displayLabel.Text = (displayNumber * -1).ToString();
        else displayLabel.Text = displayNumber.ToString();
        
        
        if (displayNumber > 0) displayLabel.SetSelfModulate(Color.Color8(255,0,0,255));// = new Color(0xffff0000);       //RED
        else if (displayNumber < 0) displayLabel.SelfModulate =  Color.Color8(0,255,0,255);  //GREEN
        else
        {
            displayLabel.SelfModulate = Color.Color8(255, 255, 255, 255);                         //WHITE
            GD.Print("[DamagePlayer] modulate = " + displayLabel.SelfModulate);
        } 
            
        if (this.IsPlaying()) RelaunchAnim();//If animation already Playing : reset it

        //timing 0.00 -> 0.33 -> 1
        //timing 0.33 -> 0.66 -> 2
        //timing 0.65 -> 0.98 -> 3

        float playSpeed = (float)(1/((timing+0.33f) * 3));
        GD.Print("[DamagePlayer] anim will last for : "+ (0.33 / playSpeed) + ", timing was = " + timing + " end at " + ((0.33/playSpeed) - timing));
        this.Play("Damaged", -1, playSpeed);
    }

    private void RelaunchAnim()
    {
        float playSpeed = (float)(1 / ((GetTree().Root.GetNode<Global>("Global").GetLevel().GetTime() + 0.33f) * 3));
        this.Stop();
        this.Play("Damaged", -1, playSpeed);
        GetParent().GetParent<Entity>().CheckDeath();
    }

    public void AnimationOver(String anim_name)
    {
        GD.Print("[DamagePlayer] DAMAGE ANIMATION ENDED final damage = " + displayNumber);
        displayNumber = 0;
        GetParent().GetParent<Entity>().CheckDeath();
        
    }

    /*

     bool                       0.12 octets
     byte   char                1 octet
     short                      2 octets
     int    string  float       4 octets
     long           double      8 octets
     */

}
