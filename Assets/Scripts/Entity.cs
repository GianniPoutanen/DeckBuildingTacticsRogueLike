using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public bool Enabled = true;
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

    IEnumerator JabCoroutine(Vector3 jabDirection, float speed, float duration, float distance)
    {
        // Calculate the target rotation angle
        float targetRotation = Mathf.Atan2(jabDirection.y, jabDirection.x) * Mathf.Rad2Deg;

        // Smoothly rotate towards the target rotation
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotationQuaternion = Quaternion.Euler(0, 0, targetRotation);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotationQuaternion, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Jab forward
        float jabDistanceRemaining = distance;
        while (jabDistanceRemaining > 0f)
        {
            transform.position += jabDirection * speed * Time.deltaTime;
            jabDistanceRemaining -= speed * Time.deltaTime;
            yield return null;
        }

        // Wait for a short duration
        yield return new WaitForSeconds(0.1f);

        // Return to original rotation
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(targetRotationQuaternion, startRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is exactly the original rotation
        transform.rotation = startRotation;
    }

    
    IEnumerator JabWithActionCoroutine(Ability ability, Vector3 jabDirection, float speed, float duration, float distance)
    {
        // Calculate the target rotation angle
        float targetRotation = Mathf.Atan2(jabDirection.y, jabDirection.x) * Mathf.Rad2Deg;

        // Smoothly rotate towards the target rotation
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotationQuaternion = Quaternion.Euler(0, 0, targetRotation);

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotationQuaternion, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Jab forward
        float jabDistanceRemaining = distance;
        while (jabDistanceRemaining > 0f)
        {
            transform.position += jabDirection * speed * Time.deltaTime;
            jabDistanceRemaining -= speed * Time.deltaTime;
            yield return null;
        }

        // Wait for a short duration
        yield return new WaitForSeconds(0.1f);

        // Return to original rotation
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.rotation = Quaternion.Slerp(targetRotationQuaternion, startRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is exactly the original rotation
        transform.rotation = startRotation;
    }
}
