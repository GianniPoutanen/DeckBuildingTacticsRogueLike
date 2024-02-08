using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Base Entity Stats")]
    public bool CanDamage;
    [SerializeField]
    public virtual int Armour { get; set; }
    public int StartingHeatlh;
    [SerializeField]
    public virtual int MaxHealth { get { return StartingHeatlh; } set { StartingHeatlh = value; } }
    [SerializeField]
    public virtual int Health { get; set; }

    [Header("Poison")]
    public bool CanPosion = false;
    public int PoisonAmount = 0;

    [Header("Health Bar Location")]
    public Transform healthBarLocation;

    public virtual void Start()
    {
        EventManager.Instance.InvokeEvent<Entity>(EventType.EntitySpawned, this);
        SubscribeToEvents();
        Health = MaxHealth;
    }

    public virtual void OnDestroy()
    {
        EventManager.Instance.InvokeEvent<Entity>(EventType.EntityDestroyed, this);
        UnsubscribeToEvents();
    }

    public virtual void TurnStart()
    {
        PierceDamage(PoisonAmount);
        PoisonAmount--;
    }

    #region Healing and Damage

    public virtual void Shield(int amount)
    {
        Armour += amount;
    }

    public virtual void Heal(int amount)
    {
        if (Health + amount > MaxHealth)
        {
            Health = MaxHealth;
        }
        else
        {
            Health += amount;
        }
    }

    public virtual void Damage(int amount)
    {
        int leftOver = amount;
        if (Armour > 0)
        {
            if (Armour - amount <= 0)
            {
                leftOver = amount - Armour;
                Armour = 0;
            }
            else
            {
                Armour -= amount;
            }
        }
        if (Armour == 0)
        {
            if (Health - leftOver <= 0)
                Health = 0;
            else
                Health -= leftOver;
        }
        DeathCheck();
    }
    public virtual void PierceDamage(int amount)
    {
        if (Health - amount <= 0)
            Health = 0;
        else
            Health -= amount;
        DeathCheck();
    }

    public virtual void DeathCheck()
    {
        EventManager.Instance.InvokeEvent(EventType.EntityDestroyed);
        if (Health <= 0)
            Destroy(this.gameObject);
    }

    public virtual bool CanMoveTo(Vector3Int position)
    {
        return true;
    }
    #endregion Healing and Damage

    public abstract void SubscribeToEvents();
    public abstract void UnsubscribeToEvents();
}
