
using System;
using System.Collections.Generic;
using UnityEngine;

public class CompositeAction : Ability
{
    public Guid Guid { get; set; } = new Guid();
    public List<Ability> actions = new List<Ability>();

    public override void Undo()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            UndoRedoManager.Instance.Undo();
        }
    }

    public override void Redo()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            UndoRedoManager.Instance.Redo();
        }
    }

    public override void Perform()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            actions[i].Perform();
        }
        UndoRedoManager.Instance.AddUndoAction(this);
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

public class CompositeAbilityBuilder : AbilityBuilder
{
    CompositeAction ability = new CompositeAction();

    public CompositeAbilityBuilder(CompositeAction compositeAction)
    {
        ability = compositeAction;
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        ability.Performer = performer;
        return base.SetPerformer(performer);
    }

    public override Ability Build()
    {
        return ability;
    }
}