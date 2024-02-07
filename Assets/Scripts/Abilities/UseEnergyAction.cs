
using UnityEngine;

public class UseEnergyAction : Ability
{
    private int newEnergyAmount;
    private int oldEnergyAmount;
    public int amount = 0;

    public override void Perform()
    {
        oldEnergyAmount = PlayerManager.Instance.CurrentEnergy;
        newEnergyAmount = PlayerManager.Instance.CurrentEnergy - amount;
        PlayerManager.Instance.CurrentEnergy = newEnergyAmount;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }

    public override void Redo()
    {
        PlayerManager.Instance.CurrentEnergy = newEnergyAmount;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }

    public override void Undo()
    {
        PlayerManager.Instance.CurrentEnergy = oldEnergyAmount;
        EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
    }

    public override bool CanPerform(Vector3Int position)
    {
        return PlayerManager.Instance.CurrentEnergy - amount >= 0;
    }
}
