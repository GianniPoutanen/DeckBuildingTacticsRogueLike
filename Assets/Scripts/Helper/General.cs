using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class General
{
    public static List<GameObject> GetChildList(Transform parent)
    {
        List<GameObject> childList = new List<GameObject>();

        // Iterate through all the child objects of the parent
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            childList.Add(child.gameObject);
        }

        return childList;
    }
}
