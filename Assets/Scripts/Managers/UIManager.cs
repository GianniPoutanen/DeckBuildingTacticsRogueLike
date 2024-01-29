using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singleton Pattern

    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(UIManager).Name;
                    instance = obj.AddComponent<UIManager>();
                    instance.canvas = obj.GetComponent<Canvas>();
                    instance.rectTransform = obj.GetComponent<RectTransform>();
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
            instance.canvas = this.GetComponent<Canvas>();
            instance.rectTransform = this.GetComponent<RectTransform>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public RectTransform rectTransform;
    public Canvas canvas;
    public Hand hand;
}
