
using UnityEngine;

public class UseEnergyAction : Ability
{
    private int newEnergyAmount;
    private int oldEnergyAmount;

    public override void Perform()
    {
        base.Perform();
    }

    public override void Redo()
    {
        base.Redo();
    }

    public override void Undo()
    {
        base.Undo();
    }

    public override bool CanPerform(Vector3Int position)
    {
        return PlayerManager.Instance.CurrentEnergy - cost >= 0;
    }
}
