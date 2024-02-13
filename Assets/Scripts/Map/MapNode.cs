using UnityEngine;
using UnityEngine.SceneManagement;

public class MapNode : MonoBehaviour
{
    public string sceneToLoad;  // The scene to load when the node is clicked

    private void OnMouseDown()
    {
        // Handle mouse click on the node
        LoadScene();    
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene to load is not specified for this node.");
        }
    }
}