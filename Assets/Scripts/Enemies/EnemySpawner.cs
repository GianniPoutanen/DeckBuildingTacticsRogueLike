using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Reference to the enemy prefab
    public int amount = 1;
    public GameObject[] possiblePositionTransforms;

    void Start()
    {
        possiblePositionTransforms = General.GetChildList(this.transform).ToArray();
        EnsureEnemyManager();
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < amount; i++)
            SpawnEnemy();
    }

    void SpawnEnemy()
    {
        // Spawn the enemy
        GameObject position = possiblePositionTransforms[Random.Range(0,possiblePositionTransforms.Length)];
        var posList= possiblePositionTransforms.ToList();
        posList.Remove(position);
        possiblePositionTransforms = posList.ToArray();
        GameObject enemy = Instantiate(enemyPrefab, position.transform .position, Quaternion.identity);
    }

    void EnsureEnemyManager()
    {
        // Check if there is an Enemy Manager in the scene
        EnemyManager enemyManager = FindObjectOfType<EnemyManager>();

        // If there's no Enemy Manager found, instantiate it
        if (enemyManager == null)
        {
            Debug.Log($"Spawned {EnemyManager.Instance.name}");
            
        }
    }

}
