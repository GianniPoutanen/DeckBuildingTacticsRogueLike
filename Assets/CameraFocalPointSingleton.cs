using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocalPointSingleton : MonoBehaviour
{

    #region Singleton Pattern
    private static CameraFocalPointSingleton instance;

    public static CameraFocalPointSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CameraFocalPointSingleton>();

                if (instance == null)
                {
                    GameObject obj = new GameObject("EnemyManager");
                    instance = obj.AddComponent<CameraFocalPointSingleton>();
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
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion Singleton Pattern

    public GameObject FocalPoint;
    public Vector3 Position;

    [Header("Offsets")]
    public Vector3 offset;
    public Vector3 playerOffset;

    private void Update()
    {
        if (FocalPoint == null)
        {
            this.transform.position = Position;
        }
        else
        {
            Vector3 pos = FocalPoint.transform.position + offset;
            if (FocalPoint.tag == "Player")
                pos += playerOffset;
            this.transform.position = pos;
        }
    }

    public void SetFocal(Vector3 position)
    {
        Position = position;
        FocalPoint = null;
    }

    public void SetFocal(GameObject focalObj)
    {
        FocalPoint = focalObj;
    }


}
