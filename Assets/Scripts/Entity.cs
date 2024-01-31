using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public bool CanDamage;
    public int Armour;
    public int Maxealth;
    public int Health;

    public virtual void Start()
    {
        EventManager.Instance.InvokeEvent<Entity>(Enums.EventType.EntitySpawned, this);
        SubscribeToEvents();
    }

    public virtual void OnDestroy()
    {
        EventManager.Instance.InvokeEvent<Entity>(Enums.EventType.EntityDestroyed, this);
        UnsubscribeToEvents();
    }

    #region Healing and Damage

    public virtual void Shield(int amount)
    {
        Armour += amount;
    }

    public virtual void Heal(int amount)
    {
        if (Health + amount > Maxealth)
        {
            Health = Maxealth;
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

        if (Health - leftOver <= 0)
            Health = 0;
        else
            Health -= leftOver;
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
        EventManager.Instance.InvokeEvent(Enums.EventType.EntityDestroyed);
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
