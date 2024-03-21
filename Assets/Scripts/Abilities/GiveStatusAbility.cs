
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Status/Give Status Ability")]
public class GiveStatusAbility : Ability
{
    public Status status;
    public int amount;
    public int beforeStatusAmount = 0;
    public int afterStatusAmount = 0;

    public override void Undo()
    {
        base.Undo();
        var target = GridManager.Instance.GetEntityOnPosition(TargetPosition, entityMask);
        if (target != null)
            target.SetStatus(status, beforeStatusAmount);
    }

    public override void Redo()
    {
        Perform();
    }

    public override void Perform()
    {
        var target = GridManager.Instance.GetEntityOnPosition(TargetPosition, entityMask);
        base.Perform();
        if (target != null)
        {
            Debug.Log($"{Performer.name} gives status {status.ToString()} to {target.name} of amount {amount}.");
            beforeStatusAmount = target.GetStatus(status);
            target.GiveStatus(status, amount);
            afterStatusAmount = target.GetStatus(status);
            UIManager.Instance.UpdateUI();
        }
    }
}