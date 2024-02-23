using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

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

    [Header("Statuses")]
    public Status[] DissabledStatuses = new Status[0];
    public bool Stunned;
    public bool Rooted;
    public Dictionary<Status, int> Statuses = new Dictionary<Status, int>();

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

    public virtual void SetupEntity()
    {

    }

    public virtual void TurnStart()
    {
        UIManager.Instance.UpdateUI();
    }

    public virtual void SetStatus(Status status, int amount)
    {
        if (!Statuses.ContainsKey(status))
            Statuses.Add(status, 0);
         Statuses[status] = amount;
    }
    public virtual int GetStatus(Status status)
    {
        if (!Statuses.ContainsKey(status))
            Statuses.Add(status, 0);
        return Statuses[status];
    }

    public virtual void GiveStatus(Status status, int amount)
    {
        if (!DissabledStatuses.Contains(status))
        {
            if (!Statuses.ContainsKey(status))
                Statuses.Add(status, 0);

            Statuses[status] += amount;
            switch (status)
            {
                case Status.Hasten:
                    if (this is PlayerController)
                    {
                        PlayerManager.Instance.CurrentEnergy += amount;
                        UIManager.Instance.UpdateUI();
                    }
                    else
                    {

                    }
                    //TODO increase energy
                    break;
            }
        }
    }

    public virtual void Destroy()
    {
        throw new System.Exception();
    }

    #region Healing and Damage

    public virtual void Shield(int amount)
    {
        EventManager.Instance.InvokeEvent<Entity>(EventType.EntityInteracted, this);
        Armour += amount;
    }

    public virtual void Heal(int amount)
    {
        EventManager.Instance.InvokeEvent<Entity>(EventType.EntityInteracted, this);
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
        EventManager.Instance.InvokeEvent<Entity>(EventType.EntityInteracted, this);
        int totalAmount = ((amount * (GetStatus(Status.Weaken) > 0 ? 2 : 1)) - GetStatus(Status.Shielded)) * (GetStatus(Status.Marked)> 0 ? 3 : 1);
        int leftOver = totalAmount;
        if (Armour > 0)
        {
            if (Armour - totalAmount <= 0)
            {
                leftOver = totalAmount - Armour;
                Armour = 0;
            }
            else
            {
                Armour -= totalAmount;
            }
        }
        if (Armour == 0)
        {
            if (Health - leftOver <= 0)
                Health = 0;
            else
                Health -= leftOver;
        }
        // Hacky way of getting rid of marked but allowing undo
        if (GetStatus(Status.Marked) > 0 && this is GridEntity)
            UndoRedoManager.Instance.AddUndoAction(new GiveTargetStatusAbility() { Performer = (GridEntity)this, amount = -1, status = Status.Marked });

        DeathCheck();
    }

    public virtual void PierceDamage(int amount)
    {
        EventManager.Instance.InvokeEvent<Entity>(EventType.EntityInteracted, this);
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

    #region Event Handlers

    public virtual void SubscribeToEvents()
    {
        EventManager.Instance.AddListener(EventType.AllEntitiesSpawned, SetupEntity);
    }

    public virtual void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener(EventType.AllEntitiesSpawned, SetupEntity);
    }

    #endregion Event Handlers

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
            while (Vector3.Distance(sprite.transform.localPosition, originalPosition) > 0.1)
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
