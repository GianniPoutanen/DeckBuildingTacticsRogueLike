using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region Singleton Pattern
    private static EnemyManager instance;

    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EnemyManager>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("EnemyManager");
                    instance = obj.AddComponent<EnemyManager>();
                }
            }

            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SubscribeToEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion Singleton Pattern

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }



    private List<Enemy> enemies = new List<Enemy>();

    public void EndPlayerTurnHandler()
    {
        StartCoroutine(CommandAllEnemies());
    }

    // Command all enemies to do something
    public IEnumerator CommandAllEnemies()
    {
        foreach (Enemy enemy in enemies)
        {
            // Implement logic to command the enemy
            yield return enemy.DoTurn();
        }
        EventManager.Instance.InvokeEvent(Enums.EventType.EndEnemyTurn);
        yield return null;
    }


    #region Event Handlers

    public void EntitySpawnedHandler(Entity entity)
    {
        if (entity is Enemy)
            enemies.Add((Enemy)entity);
    }

    public void EntityDestroyedHandler(Entity entity)
    {
        if (entity is Enemy)
        enemies.Add((Enemy)entity);
    }

    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener(Enums.EventType.EndPlayerTurn, EndPlayerTurnHandler);
        EventManager.Instance.AddListener<Entity>(Enums.EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.AddListener<Entity>(Enums.EventType.Entitydestroyed, EntityDestroyedHandler);
    }

    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener(Enums.EventType.EndPlayerTurn, EndPlayerTurnHandler);
        EventManager.Instance.RemoveListener<Entity>(Enums.EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.RemoveListener<Entity>(Enums.EventType.Entitydestroyed, EntityDestroyedHandler);
    }

    #endregion Event Handlers
}
