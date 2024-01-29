using UnityEngine;

public class Card
{
    public string cardName;
    public string description;
    public int cost;

    public Card() { }

    public virtual void Play()
    {
        UndoRedoManager.Instance.AddUndoAction(new UseEnergyAction(PlayerManager.Instance.currentEnergy - cost, PlayerManager.Instance.currentEnergy));
        PlayerManager.Instance.currentEnergy -= cost;
    }
    public virtual bool CanPlay()
    {
        // This method can be overridden in the subclasses
        if (PlayerManager.Instance.currentEnergy >= cost)
            return true;
        return false;
    }
}

public class AttackCard : Card
{
    public int damage;

    public override void Play()
    {
        base.Play();
    }
}

public class DefenseCard : Card
{
    public int armor;

    public override void Play()
    {
        base.Play();
    }
}

public class SkillCard : Card
{
    public string effect;

    public override void Play()
    {
        base.Play();
    }
}
