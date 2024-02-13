using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEntityAbility : Ability
{
    public override void Redo()
    {
        this.Perform();
    }

    public override void Undo()
    {
        Performer.gameObject.SetActive(true);
        EventManager.Instance.InvokeEvent<Entity>(EventType.EntitySpawned, Performer);
    }

    public override void Perform()
    {
        base.Perform();
        Performer.gameObject.SetActive(false);
        EventManager.Instance.InvokeEvent<Entity>(EventType.EntityDestroyed, Performer);
    }
}


public class DestroyEntityBuilder : AbilityBuilder
{
    DestroyEntityAbility destroyAbility = new DestroyEntityAbility();
    public DestroyEntityBuilder(DestroyEntityAbility ability)
    {
        this.destroyAbility = ability;
    }

    public override AbilityBuilder SetPerformer(GridEntity performer)
    {
        destroyAbility.Performer = performer;
        return this;
    }
    public override Ability Build()
    {
        return destroyAbility;
    }
}