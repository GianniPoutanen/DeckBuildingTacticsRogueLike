using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string cardName = "Card";
    public string description;
    public int cost;

    [Header("Actions List")]
    [SerializeField]
    public List<IUndoRedoAction> actions = new List<IUndoRedoAction>();

    public Card() { }
    public Card(int cost) { this.cost = cost; }

    public virtual void Play()
    {
        UndoRedoManager.Instance.AddUndoAction(new UseEnergyAction(PlayerManager.Instance.CurrentEnergy - cost, PlayerManager.Instance.CurrentEnergy));
        PlayerManager.Instance.CurrentEnergy -= cost;

    }
    public virtual bool CanPlay()
    {
        // This method can be overridden in the subclasses
        if (PlayerManager.Instance.CurrentEnergy >= cost)
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
