using System.Collections.Generic;
using UnityEngine;

public class TopMapGraphManager : MonoBehaviour
{
    public GameObject nodePrefab;  // Prefab for the graph nodes
    public int numberOfNodes = 10;  // Number of nodes in the graph
    public Transform entryPointPrefab;  // Prefab for entry points
    public Transform exitPointPrefab;   // Prefab for the final exit point

    private List<Transform> nodes = new List<Transform>();
    private Transform exitPoint;

    void Start()
    {
        // Create nodes
        CreateNodes();

        // Connect nodes to form a connected graph
        ConnectNodes();

        // Create entry points
        CreateEntryPoints();

        // Create the final exit point
        CreateExitPoint();
    }

    void CreateNodes()
    {
        for (int i = 0; i < numberOfNodes; i++)
        {
            GameObject node = Instantiate(nodePrefab, new Vector3(i * 2, 0, 0), Quaternion.identity);
            nodes.Add(node.transform);
        }
    }

    void ConnectNodes()
    {
        for (int i = 0; i < numberOfNodes - 1; i++)
        {
            // Connect each node to the next one
            ConnectNodes(nodes[i], nodes[i + 1]);
        }
    }

    void ConnectNodes(Transform nodeA, Transform nodeB)
    {
        // This is a simple example; you might want to adjust the connection logic based on your game's requirements.
        Debug.DrawLine(nodeA.position, nodeB.position, Color.green, 10f);

        // Add your logic here to create connections between nodes.
        // You might want to use a more complex algorithm to generate connections based on your game's needs.

        // Attach MapNode script to the nodes
        MapNode mapNodeA = nodeA.gameObject.AddComponent<MapNode>();
        MapNode mapNodeB = nodeB.gameObject.AddComponent<MapNode>();

        // Set the scenes to load for each node
        mapNodeA.sceneToLoad = "YourSceneNameA";
        mapNodeB.sceneToLoad = "YourSceneNameB";
    }


    void CreateEntryPoints()
    {
        // Example: Create two entry points
        Instantiate(entryPointPrefab, nodes[0].position + new Vector3(-1, 1, 0), Quaternion.identity);
        Instantiate(entryPointPrefab, nodes[0].position + new Vector3(1, 1, 0), Quaternion.identity);
    }

    void CreateExitPoint()
    {
        // Example: Create a final exit point
        exitPoint = Instantiate(exitPointPrefab, nodes[numberOfNodes - 1].position + new Vector3(0, 1, 0), Quaternion.identity);
    }
}
