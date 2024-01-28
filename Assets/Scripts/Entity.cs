using Unity.VisualScripting;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public virtual void Start()
    {
        EventManager.Instance.InvokeEvent<Entity>(Enums.EventType.EntitySpawned, this);
        SubscribeToEvents();
    }

    public virtual void OnDestroy()
    {
        EventManager.Instance.InvokeEvent<Entity>(Enums.EventType.Entitydestroyed, this);
        UnsubscribeToEvents();
    }

    public abstract void SubscribeToEvents();
    public abstract void UnsubscribeToEvents();
    
    
}
