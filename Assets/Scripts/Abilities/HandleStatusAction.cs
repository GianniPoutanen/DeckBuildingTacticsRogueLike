

using UnityEngine;

public class HandleStatusAction : Ability
{
    private int beforeHealth;
    private int beforeArmour;
    private int afterHealth;
    private int afterArmour;

    public Status status;
    public bool global { get { return range == 0; } }
    [Tooltip("If 0 then ability effect will be global")]
    public int range;

    public override void Undo()
    {
        Performer.Armour = beforeArmour;
        Performer.Health = beforeHealth;
        switch (status)
        {
            case Status.Marked:
                break;
            case Status.Stunned:
                Performer.Stunned = false;
                Performer.GiveStatus(status, 1);
                break;
            case Status.Rooted:
                Performer.Rooted = false;
                Performer.GiveStatus(status, 1);
                break;
            default:
                Performer.GiveStatus(status, 1);
                break;
        }
    }

    public override void Redo()
    {
        Perform();
    }

    public override void Perform()
    {
        beforeArmour = Performer.Armour;
        beforeHealth = Performer.Health;
        switch (status)
        {
            case Status.Poison:
                Performer.PierceDamage(Performer.GetStatus(Status.Poison));
                Performer.GiveStatus(status, -1);
                break;
            case Status.Regeneration:
                Performer.Heal(Performer.GetStatus(Status.Regeneration));
                Performer.GiveStatus(status, -1);
                break;
            case Status.Stunned:
                Performer.Stunned = true;
                Performer.GiveStatus(status, -1);
                break;
            case Status.Rooted:
                Performer.Rooted = true;
                Performer.GiveStatus(status, -1);
                break;
            default:
                Performer.GiveStatus(status, -1);
                break;
        }
        afterArmour = Performer.Armour;
        afterHealth = Performer.Health;
    }

    public override bool CanPerform(Vector3Int position)
    {
        var target = GridManager.Instance.GetEntityOnPosition(position, entityMask);
        return target != null && target != Performer && (global || GridManager.Instance.GetWalkingDistance(Performer.targetGridPosition, position) <= range);
    }
}