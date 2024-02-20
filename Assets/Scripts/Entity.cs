using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public bool Enabled = true;
    [Header("Turn Action Energy")]
    public int maxEnergy = 1;
    public int currentEnergy;

    [Header("Base Entity Stats")]
    public bool CanDamage;
    [SerializeField]
    public virtual int Armour { get; set; }
    public int StartingHeatlh;
    [SerializeField]
    public virtual int MaxHealth { get { return StartingHeatlh; } set { StartingHeatlh = value; } }
    public int CurrentHeatlh;
    [SerializeField]
    public virtual int Health { get { return CurrentHeatlh; } set { CurrentHeatlh = value; } }

    [Header("Poison")]
    public bool CanPosion = false;
    public int PoisonAmount = 0;

    [Header("Health Bar Location")]
    public Transform healthBarLocation;

    [Header("Jab Variables")]
    public float jabSpeed = 5f;
    public float jabDistance = 2f;
    private bool jabbing = false;

    [Header("Look and Feel")]
    public GameObject sprite;

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

    public virtual void Destroy()
    {
        throw new System.Exception();
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
            this.Destroy();
    }

    public virtual bool CanMoveTo(Vector3Int position)
    {
        return true;
    }
    #endregion Healing and Damage

    public abstract void SubscribeToEvents();
    public abstract void UnsubscribeToEvents();

    public IEnumerator JabCoroutine(Vector3 direction, float speed, float distance, Ability ability = null)
    {
        if (!jabbing)
        {
            jabbing = true;
            Vector3 jabDirection = new Vector3(direction.x, direction.y);
            Vector3 originalPosition = sprite.transform.localPosition;

            // Jab forward
            float jabDistanceRemaining = distance;
            while (jabDistanceRemaining > 0f)
            {
                sprite.transform.localPosition += jabDirection * speed * Time.deltaTime;
                jabDistanceRemaining -= speed * Time.deltaTime;
                yield return null;
            }

            if (ability != null)
            {
                ability.Perform();
            }
            // Wait for a short duration
            yield return new WaitForSeconds(0.01f);
            // Move back to the original position
            while (Vector3.Distance(sprite.transform.localPosition,originalPosition) > 0.1)
            {
                sprite.transform.localPosition = Vector3.Lerp(sprite.transform.localPosition, originalPosition, speed * Time.deltaTime);
                yield return null;
            }
            // Ensure the final position is exactly the original position
            sprite.transform.localPosition = originalPosition;
            jabbing = false;
        }
    }
}
