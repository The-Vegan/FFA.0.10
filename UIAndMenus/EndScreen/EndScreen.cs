using Godot;
using System;
using System.Collections.Generic;

public class EndScreen : Control
{


    private Global global;
    List<Entity> entities;

    public void Init(List<Entity> es)
    {
        entities = es;
    }
    public override void _Ready()
    {
        global = GetTree().Root.GetNode<Global>("Global");
        PackedScene box = GD.Load("res://UIAndMenus/EndScreen/FinalScoreBox.tscn") as PackedScene;

        SortEntities();

        for (int i = 0; i < 3; i++) 
        {
            if (entities[i] == null) return;
            FinalScoreBox loadedBox = box.Instance() as FinalScoreBox;
            loadedBox.Init((byte)(i + 1), entities[i]);
        }





    }

    private void SortEntities()
    {
        for (int i = 0; i < entities.Count; i++)
        {
            //removes non player entities from list.
            while (i < entities.Count && (entities[i] == null || !entities[i].isValidTarget)) { entities.RemoveAt(i); }



        }
    }
}
