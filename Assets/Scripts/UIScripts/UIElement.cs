using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour
{
    protected RectTransform RectTransform { get { return this.gameObject.GetComponent<RectTransform>(); } }

    [Header("Sizing")]
    [SerializeField] public float targetSizeFactor = 1.0f; // Default target size factor is 1.
    [SerializeField] public float sizeFactor = 1.0f; // Default target size factor is 1.
    [SerializeField] public float growthSpeed = 1.0f; // Speed of the growth.

    public virtual void Start()
    {
        
    }

    public virtual void Update()
    {
        SetSize();
    }

    public void SetSize()
    {
        // Adjust the target size factor over time.
        if (Mathf.Abs(sizeFactor - targetSizeFactor) > 0.001f)
            sizeFactor = Mathf.Lerp(sizeFactor, targetSizeFactor, growthSpeed * Time.deltaTime);
        else
            sizeFactor = targetSizeFactor;

        // Apply the new size to the RectTransform.
        RectTransform.localScale = Vector3.one * sizeFactor;
    }

}
