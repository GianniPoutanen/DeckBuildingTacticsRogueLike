

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
                Performer.Statuses[status]--;
                break;
            case Status.Rooted:
                Performer.Rooted = false;
                Performer.Statuses[status]--;
                break;
            default:
                Performer.Statuses[status]++;
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
                Performer.PierceDamage(Performer.Statuses[Status.Poison]);
                Performer.Statuses[status]--;
                break;
            case Status.Regeneration:
                Performer.Heal(Performer.Statuses[Status.Regeneration]);
                Performer.Statuses[status]--;
                break;
            case Status.Stunned:
                Performer.Stunned = true;
                Performer.Statuses[status]--;
                break;
            case Status.Rooted:
                Performer.Rooted = true;
                Performer.Statuses[status]--;
                break;
            default:
                Performer.Statuses[status]--;
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