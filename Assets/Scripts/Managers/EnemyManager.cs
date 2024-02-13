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
        if (enemies != null && enemies.Count > 0)
        {
            int enemyCount = enemies.Count;
            for (int i = 0; i < enemyCount; i++)
            {
                CameraFocalPointSingleton.Instance.SetFocal(enemies[i].gameObject);
                yield return new WaitForSeconds(0.2f);
                EventManager.Instance.InvokeEvent<Entity>(EventType.EntityTurn, enemies[i]);
                // Implement logic to command the enemy
                yield return enemies[i].DoTurn();
                yield return new WaitForSeconds(0.2f);
                if (enemies.Count != enemyCount) enemyCount--;
            }
        }
        CameraFocalPointSingleton.Instance.SetFocal(PlayerManager.Instance.Player.gameObject);
        EventManager.Instance.InvokeEvent(EventType.EndEnemyTurn);
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
            enemies.Remove((Enemy)entity);
    }

    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener(EventType.EndPlayerTurn, EndPlayerTurnHandler);
        EventManager.Instance.AddListener<Entity>(EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.AddListener<Entity>(EventType.EntityDestroyed, EntityDestroyedHandler);
    }

    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener(EventType.EndPlayerTurn, EndPlayerTurnHandler);
        EventManager.Instance.RemoveListener<Entity>(EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.RemoveListener<Entity>(EventType.EntityDestroyed, EntityDestroyedHandler);
    }

    #endregion Event Handlers
}
