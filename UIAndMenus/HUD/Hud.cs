using Godot;
using System;
using System.Collections.Generic;

public class Hud : CanvasLayer
{

    private PackedScene note = GD.Load("res://UIAndMenus/HUD/BeatNote.tscn") as PackedScene;
    private Vector2 STARTPOS = new Vector2(88,-624);

    private String nextNote = "M";
    private AnimatedSprite[] noteList = new AnimatedSprite[6];

    public void HitNote(byte nmbrOfNote ,bool volontary)
    {
        if (nmbrOfNote >= 6) return;
        if (nmbrOfNote == 0) return;
        String anim = "Hit";
        if (!volontary) anim = "destroy";
        
        for (byte i = 0; i < nmbrOfNote; i++)
        {
            AnimatedSprite bn = noteList[i];
            if (bn == null) continue;

            Tween tween = bn.GetChild(0) as Tween;
            tween?.StopAll();//Locks position : null check to prevent random crash that happend once

            bn.Play(anim);
            bn.Connect("animation_finished", bn, "queue_free");
        }
    }

    private void ReorderNoteList()
    {
        for(byte i = 1; i < noteList.Length; i++)
        {
            noteList[i - 1] = noteList[i];
        }
    }

    private AnimatedSprite CreateNewNote()
    {
        AnimatedSprite bn = note.Instance() as AnimatedSprite;

        this.AddChild(bn, true);
        Tween tween = bn.GetChild(0) as Tween;

        bn.Position = STARTPOS;
        bn.Play(nextNote);
        tween.InterpolateProperty(bn, "position", bn.Position, new Vector2(88, 440), 3.9f, Tween.TransitionType.Linear);
        tween.Start();
        return bn;
    }
    private void DestroyNote(AnimatedSprite bn)
    {
        //If checks ordered by common occurence
        if (bn == null) return;
        if (bn.IsQueuedForDeletion()) return;//godot catches this first and write error message if this isn't checked
        if (bn.Animation == "Hit") return;
        if (bn.Animation == "destroy") return;
        
        bn.Play("destroy");
        bn.Connect("animation_finished", bn, "queue_free");
    }

    private void BeatAtkUpdate()
    {

        DestroyNote(noteList[0]);
        ReorderNoteList();
        noteList[noteList.Length - 1] = CreateNewNote();

        switch (nextNote)
        {
            case "C": nextNote = "W"; break;
            case "M": nextNote = "C"; break;
            case "W": nextNote = "M"; break;
        }

    }
}
