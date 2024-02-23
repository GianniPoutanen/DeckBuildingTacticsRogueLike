
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Composite Ability")]
public class CompositeAction : Ability
{
    public Guid Guid { get; set; } = new Guid();
    public List<Ability> actions = new List<Ability>();

    public bool TriggerOnFirstAction = false;

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
        if (TriggerOnFirstAction)
        {
            if (!actions[0].CanPerform(pos))
            {
                result = false;
            }
        }
        else
        {
            foreach (Ability action in actions)
            {
                if (!action.CanPerform(pos))
                {
                    result = false;
                    break;
                }
            }
        }
        return result;
    }
}

public class CompositeAbilityBuilder : AbilityBuilder
{
    CompositeAction compositeAction = new CompositeAction();

    public CompositeAbilityBuilder(CompositeAction compositeAction)
    {
        this.compositeAction = compositeAction;
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        compositeAction.Performer = performer;
        foreach (Ability ability in compositeAction.actions)
            ability.Performer = performer;
        return base.SetPerformer(performer);
    }

    public override AbilityBuilder SetTargetPosition(Vector3Int position)
    {
        compositeAction.TargetPosition = position;
        foreach (Ability ability in compositeAction.actions)
            ability.TargetPosition = position;
        return base.SetTargetPosition(position);
    }

    public override AbilityBuilder SetAmount(int amount)
    {
        foreach (Ability ability in compositeAction.actions)
            AbilityBuilder.GetBuilder(ability).SetAmount(amount);
        return base.SetAmount(amount);
    }

    public override AbilityBuilder SetCost(int cost)
    {
        foreach (Ability ability in compositeAction.actions)
            AbilityBuilder.GetBuilder(ability).SetCost(cost);
        return base.SetCost(cost);
    }

    public override AbilityBuilder SetCard(Card card)
    {
        foreach (Ability ability in compositeAction.actions)
            AbilityBuilder.GetBuilder(ability).SetCard(card);
        return base.SetCard(card);
    }

    public override AbilityBuilder SetEntityMask(List<string> mask)
    {
        foreach (Ability ability in compositeAction.actions)
            AbilityBuilder.GetBuilder(ability).SetEntityMask(mask);
        return base.SetEntityMask(mask);
    }

    public override Ability Build()
    {
        return compositeAction;
    }
}