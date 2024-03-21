using UnityEngine;

public class DebugMenuController : MonoBehaviour
{
    public GameObject debugCanvas;

    void Start()
    {
        // Hide the debug canvas initially
        debugCanvas.SetActive(false);
    }

    void Update()
    {
        // Toggle the debug canvas when a key is pressed (for example, F1)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            debugCanvas.SetActive(!debugCanvas.activeSelf);
        }
    }


}
