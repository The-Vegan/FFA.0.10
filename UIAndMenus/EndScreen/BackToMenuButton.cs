using Godot;
using System;

public class BackToMenuButton : Button
{
    Global global;
    public async override void _Ready()
    {
        global = GetTree().Root.GetNode<Global>("Global");
        Tween tween = this.GetNode<Tween>("Tween");
        tween.InterpolateProperty(this, "modulate", this.Modulate, Color.Color8(0xff, 0xff, 0xff,0xff),7f,
            Tween.TransitionType.Expo,Tween.EaseType.Out);
        await ToSignal(GetTree().CreateTimer(3), "timeout");
        tween.Start();
    }
    public override void _Pressed()
    {
        global.ResetNetworkConfigAndGoBackToMainMenu();
    }

}
