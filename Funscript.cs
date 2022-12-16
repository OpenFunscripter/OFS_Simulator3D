using System;
using Godot;
using System.Collections.Generic;
using System.Linq;

public enum ScriptType
{
    MainStroke,
    Sway,
    Surge,
    Roll,
    Pitch,
    Twist,
    TypeCount
}

public struct Action
{
    public float timestamp;
    public float pos;
}

public class Funscript
{
    public string Name { get; private set; }
    public List<Action> Actions = new List<Action>();

    public Funscript(Godot.Collections.Dictionary changeEvent)
    {
        Name = changeEvent["name"] as string;
        UpdateFromEvent(changeEvent);
    }

    public void UpdateFromEvent(Godot.Collections.Dictionary changeEvent)
    {
        Actions.Clear();
        var script = changeEvent["funscript"] as Godot.Collections.Dictionary;
        var actions = script["actions"] as Godot.Collections.Array;
        foreach(Godot.Collections.Dictionary action in actions)
        {
            Actions.Add(new Action() { 
                timestamp = (action["at"] as float? ?? 0.0f) / 1000.0f,
                pos = action["pos"] as float? / 100.0f ?? 0.0f
            });
        }
    }

    public float GetPositionAt(float time) 
    {
        var idx = Actions.BinarySearch(0, Actions.Count, new Action() 
        {
            timestamp = time,
            pos = 0
        }, Comparer<Action>.Create((a,b) => Comparer<float>.Default.Compare(a.timestamp, b.timestamp)));

        if(idx < 0)
        {
            idx = ~idx;
            if(idx - 1 >= 0 && idx < Actions.Count)
            {
                var current = Actions[idx-1];
                var next = Actions[idx];

                float t = (time - current.timestamp) / (next.timestamp - current.timestamp);
                return Mathf.Lerp(current.pos, next.pos, t);
            }
            else if(Actions.Count > 0)
            {
                return Actions.Last().pos;
            }
        }
        else 
        {
            return Actions[idx].pos;
        }
        
        return 0.5f;
    }
}