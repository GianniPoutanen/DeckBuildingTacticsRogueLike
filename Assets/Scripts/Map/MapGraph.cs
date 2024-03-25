using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGraph : MonoBehaviour
{


    public Transform entryNode;
    public Transform exitNode;
    public GameObject nodePrefab;
    public int numEntries = 3;
    public int numberOfLevels = 5;
    public int minNodesPerLevel = 2;
    public int maxNodesPerLevel = 5;
    public float chaosFactor = 0.5f;
    public float levelNodeNumberDistanceFactor = 0.5f;

    public float distanceBetweenNodes = 1f;
    public Dictionary<int, List<Node>> nodes = new Dictionary<int, List<Node>>();


    void Start()
    {
        GenerateGraph();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            GenerateGraph();
    }

    public void GenerateGraph()
    {
        while (nodes.Keys.Count > 0)
        {
            while (nodes[nodes.Keys.First()].Count > 0)
            {
                var node = nodes[nodes.Keys.First()][0];
                nodes[nodes.Keys.First()].RemoveAt(0);
                Destroy(node.gameObject);
            }
            nodes.Remove(nodes.Keys.First());
        }

        Vector3 startNodePos = entryNode.position;
        Vector3 endNodePos = exitNode.position;
        Vector3 perpendicular = PerpendicularCounterClockwise(endNodePos - startNodePos).normalized;

        List<List<Node>> levels = new List<List<Node>>();
        for (int level = 0; level < numberOfLevels; level++)
        {
            float numberOfNodes = level < (numberOfLevels - 1) ? Random.Range(minNodesPerLevel, maxNodesPerLevel + 1) : 1;
            nodes.Add(level, new List<Node>());
            for (float i = 0; i < numberOfNodes; i++)
            {
                // Get the position between pointA and pointB
                Vector3 newPosition = Vector3.Lerp(startNodePos, endNodePos, (float)((float)(level) / (float)(numberOfLevels - 1)));
                // Find a vector perpendicular to the line between pointA and pointB

                Vector3 finalPosition = newPosition + (perpendicular * (levelNodeNumberDistanceFactor / numberOfNodes) * distanceBetweenNodes * ((0.5f + i) - ((numberOfNodes / 2f))));
                finalPosition += new Vector3(-chaosFactor + Random.Range(0, chaosFactor), -chaosFactor + Random.Range(0, chaosFactor));
                Node node = Instantiate(nodePrefab, finalPosition, Quaternion.identity).GetComponent<Node>();
                node.transform.SetParent(this.transform);
                node.level = level;
                node.index = (int)i;
                nodes[level].Add(node);
            }
        }

        GenerateRandomConnections();
    }


    void GenerateRandomConnections()
    {
        //
        for (int level = 0; level < numberOfLevels - 1; level++)
        {
            int numberOfNodes = nodes[level].Count;
            for (int i = 0; i < numberOfNodes; i++)
            {
                // Change To Connect
                List<Node> possibleNodes = GetPossibleNodes(level, i);
                foreach (Node node in possibleNodes)
                    nodes[level][i].ConnectTo(node);
            }
        }
    }

    private List<Node> GetPossibleNodes(int level, int index)
    {
        List<Node> currentLevel = nodes[level];
        List<Node> nextLevel = nodes[level + 1];
        List<Node> results = new List<Node>();

        int possibleNodeStartIndex = Mathf.CeilToInt((((float)index * ((float)nextLevel.Count / (float)currentLevel.Count)))) - 1;
        int possibleNodeEndIndex = Mathf.FloorToInt(((float)index * ((float)nextLevel.Count / (float)currentLevel.Count))) + 1;
        possibleNodeStartIndex = possibleNodeStartIndex < 0 ? 0 : possibleNodeStartIndex;
        if (index > 0 && possibleNodeStartIndex < currentLevel[index - 1].GetConnectedNodesMaxIndex())
            possibleNodeStartIndex = currentLevel[index - 1].GetConnectedNodesMaxIndex();
        possibleNodeEndIndex = possibleNodeEndIndex > nextLevel.Count - 1 ? nextLevel.Count - 1 : possibleNodeEndIndex;

        for (int i = possibleNodeStartIndex; i <= possibleNodeEndIndex; i++)
            results.Add(nextLevel[i]);
        return results;
    }

    public Vector3 PerpendicularClockwise(Vector2 vector2)
    {
        return new Vector2(vector2.y, -vector2.x);
    }

    public Vector3 PerpendicularCounterClockwise(Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }


    // Fisher-Yates shuffle algorithm
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public IEnumerator StrikeOffLevel(int level)
    {
        yield return null;
    }
}
