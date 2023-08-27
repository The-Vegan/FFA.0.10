using Godot;

public class LevelLoader : Button
{
    [Export]
    protected PackedScene lvlToLoad;
    protected MainMenu mainMenu;
    protected Global global;
    public override void _Ready()
    {
        global = this.GetTree().Root.GetNode<Global>("Global");
        mainMenu = GetParent().GetParent() as MainMenu;
    }

    public override void _Pressed()
    {
        
        if(global.isMultiplayer)
        {
            GD.Print("[LevelLoader] multiplayer");
            mainMenu.MoveCameraTo(6);
            global.bufferLvlToLoad = lvlToLoad;
        }
        else
        {
            GD.Print("[LevelLoader] Solo");
            global.CloseMenuAndOpenLevel(mainMenu, lvlToLoad);
        }
    }
}
