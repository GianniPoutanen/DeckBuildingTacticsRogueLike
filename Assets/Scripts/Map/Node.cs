using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Node : MonoBehaviour
{
    public List<Node> nextLevelConnections = new List<Node>();
    public List<Node> previousLevelConnections = new List<Node>();
    public LineRenderer mapLine;
    public int level = -1;
    public int index = -1;
    public SpriteRenderer sprite;

    public float sizeIncreaseFactor;
    private Vector3 startScale;

    private void Awake()
    {
        startScale = transform.localScale;
        sprite.color = new Color(Random.value, Random.value, Random.value);
    }

    public void ConnectTo(Node otherNode)
    {
        nextLevelConnections.Add(otherNode);
        DrawLineBetweenPoints(otherNode.transform.position);
        otherNode.previousLevelConnections.Add(this);
    }

    private void DrawLineBetweenPoints(Vector3 newPointPosition)
    {
        LineRenderer newLineRenderer = Instantiate(mapLine, this.transform);
        newLineRenderer.SetPosition(0, this.transform.position);
        newLineRenderer.SetPosition(1, newPointPosition);
    }

    public int GetConnectedNodesMaxIndex()
    {
        int result = -1;
        foreach (Node node in nextLevelConnections)
        {
            if (node.index > result)
            {
                result = node.index;
            }
        }
        return result;
    }

    private void OnMouseEnter()
    {
        if (CanClick())
            this.transform.localScale = startScale * sizeIncreaseFactor;
    }

    private void OnMouseExit()
    {
        this.transform.localScale = startScale;
    }

    private void OnMouseDown()
    {
        if (CanClick())
        {
            MapManager.Instance.currentNode = this;
            MapManager.Instance.HideMap();
            SceneManager.LoadScene("Bats4", LoadSceneMode.Additive);
        }
    }

    private bool CanClick()
    {
        return MapManager.Instance.currentNode != null ? MapManager.Instance.currentNode.nextLevelConnections.Contains(this) : level == 0;
    }
}
