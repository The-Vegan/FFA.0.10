using Godot;
using System;

public class Join : Button
{
    protected MainMenu mm;
    public override void _Ready()
    {
        mm = this.GetParent().GetParent() as MainMenu;
    }

    public override void _Pressed()
    {
        mm.MoveCameraTo(5);//This is the button that send the player to the connexion screen, NOT TO BE CONFUSED WITH : ConnectToServerButton.cs
    }
}
