using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    #region Singleton Pattern

    private static MapManager instance;

    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MapManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(MapManager).Name;
                    instance = obj.AddComponent<MapManager>();
                }
            }
            return instance;
        }
    }

    #endregion

    public MapGraph[] maps;

    public Node currentNode = null;

    public void HideMap()
    {
        this.gameObject.gameObject.SetActive(false);
    }

    public void ShowMap()
    {
        this.gameObject.gameObject.SetActive(true);
    }
}
