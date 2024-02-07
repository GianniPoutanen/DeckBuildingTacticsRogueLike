
using System.Collections.Generic;
using UnityEngine;

public class CompositeAction : Ability
{
    public List<Ability> actions = new List<Ability>();

    public override void Undo()
    {
        for (int i = 0; i <= actions.Count - 1; i++)
        {
            actions[i].Undo();
        }
    }

    public override void Redo()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Redo();
        }
    }

    public override void Perform()
    {
        base.Perform();
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Perform();
        }
    }

    public override bool CanPerform(Vector3Int pos)
    {
        bool result = true;
        foreach (Ability action in actions)
        {
            if (!action.CanPerform(pos))
            {
                result = false;
                break;
            }
        }
        return result;
    }
}