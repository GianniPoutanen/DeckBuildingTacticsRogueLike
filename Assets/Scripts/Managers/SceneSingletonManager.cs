using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSingletonManager : MonoBehaviour
{
	#region Singleton Pattern

	private static SceneSingletonManager instance;

	public static SceneSingletonManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<SceneSingletonManager>();
				if (instance == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(SceneSingletonManager).Name;
					instance = obj.AddComponent<SceneSingletonManager>();
				}
			}
			return instance;
		}
	}

	#endregion

	private void Awake()
	{
		// Subscribe to the sceneLoaded event
		SceneManager.sceneLoaded += OnSceneLoaded;
	}


	private void OnDestroy()
	{
		// Unsubscribe from the sceneLoaded event
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		// Perform any necessary actions when a new scene is loaded
		Debug.Log("Scene loaded: " + scene.name);

		// Example: Check if the loaded scene is a gameplay scene and handle singleton objects accordingly
		if (scene.name == "GameplayScene")
		{
			// Example: Ensure GameManager singleton is present
			EnsureGameManager();
		}
	}

	private void EnsureGameManager()
	{
		if (GameManager.Instance == null)
		{
			// Example: Instantiate GameManager prefab if it doesn't exist
			Instantiate(Resources.Load("GameManagerPrefab"));
		}
	}
}
