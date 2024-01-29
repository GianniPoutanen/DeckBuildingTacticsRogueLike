using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridManager : MonoBehaviour
{
    #region Singleton Pattern

    private static GridManager instance;

    public static GridManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GridManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(GridManager).Name;
                    instance = obj.AddComponent<GridManager>();
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
            SubscribeToEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public Grid grid;
    // Tile Maps
    public Tilemap floorTilemap;
    public Tilemap selectionTilemap;
    // List of entities
    public List<GridEntity> entities = new List<GridEntity>();

    [Header("Tiles")]
    public Tile selectedTile;
    public Tile unselectedTile;

    void Update()
    {
        // Example: Set the selected grid position on mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int selectedGridPosition = GetGridPositionFromWorldPoint(mouseWorldPos);
            List<Vector3Int> selectedGridPositions = new List<Vector3Int>();

            selectedGridPositions.Add(new Vector3Int(selectedGridPosition.x, selectedGridPosition.y, 0));
            selectedGridPositions.AddRange(FindPath(selectedGridPosition, PlayerManager.Instance.Player.targetGridPosition));
            UpdateSelectedTilemap(selectedGridPositions);

        }
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }


    // Update the tilemap to reflect the selected tiles
    private void UpdateSelectedTilemap(List<Vector3Int> selectedGridPositions)
    {
        BoundsInt bounds = floorTilemap.cellBounds;

        foreach (var position in bounds.allPositionsWithin)
        {
            TileBase currentTile = floorTilemap.GetTile(position);

            // Selected tiles
            if (currentTile != null)
            {
                // Check if the position matches the selected grid position
                if (selectedGridPositions.Contains(position))
                {
                    // Set the tile to the selected tile
                    selectionTilemap.SetTile(position, selectedTile);
                }
                else
                {
                    // Set all other tiles to the unselected tile
                    selectionTilemap.SetTile(position, null);
                }
            }
        }
    }

    #region A*



    public List<Vector3Int> FindPath(Vector3Int startPos, Vector3Int targetPos, List<string> entityMask)
    {
        GridNode startNode = new GridNode(startPos);
        GridNode targetNode = new GridNode(targetPos);
        return FindPath(startNode, targetNode, entityMask);
    }

    public List<Vector3Int> FindPath(Vector3Int startPos, Vector3Int targetPos)
    {
        GridNode startNode = new GridNode(startPos);
        GridNode targetNode = new GridNode(targetPos);
        return FindPath(startNode, targetNode, new List<string>());
    }

    public List<Vector3Int> FindPath(GridNode startNode, GridNode targetNode, List<string> entityMask)
    {
        List<Vector3Int> path = new List<Vector3Int>();

        MinHeap<GridNode> openSet = new MinHeap<GridNode>();
        HashSet<GridNode> closedSet = new HashSet<GridNode>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            GridNode currentNode = openSet.RemoveMin();
            closedSet.Add(currentNode);

            if (closedSet.Count > 5000)
            {
                break;
            }

            if (currentNode.Equals(targetNode))
            {
                path = RetracePath(startNode, currentNode);
                return path;
            }

            foreach (GridNode neighbor in GetValidNeighbours(currentNode, entityMask))
            {
                if (closedSet.Contains(neighbor))
                    continue;
                
                int newCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor);
                neighbor.Parent = currentNode;

                if (newCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                {
                    neighbor.GCost = newCostToNeighbor;
                    neighbor.HCost = GetDistance(neighbor, targetNode);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return path; // No path found
    }


    public List<GridNode> GetValidNeighbours(GridNode node, List<string> entityMask)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>()
        {
            node.Position + new Vector3Int(1,0),
            node.Position + new Vector3Int(-1,0),
            node.Position + new Vector3Int(0,1),
            node.Position + new Vector3Int(0,-1)
        };
        List<GridNode> returnList = new List<GridNode>();
        foreach (Vector3Int neighbour in neighbours)
        {
            if (FloorTileExhists(neighbour) && !HasEntitiesAtPosition(neighbour, entityMask))
                returnList.Add(new GridNode(neighbour));
        }
        return returnList;
    }

    // Retrace the path from the start node to the target node
    public static List<Vector3Int> RetracePath(GridNode startNode, GridNode targetNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        GridNode currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
            if (currentNode == null) break;
        }

        path.Reverse();
        return path;
    }

    // Get the distance between two grid nodes
    public static int GetDistance(GridNode nodeA, GridNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        int dstY = Mathf.Abs(nodeA.Position.y - nodeB.Position.y);

        // Manhattan distance
        return dstX + dstY;
    }
    #endregion A*

    #region Event Handlers

    public void EntitySpawnedHandler(Entity entity)
    {
        if (entity is GridEntity)
            entities.Add((GridEntity)entity);
    }

    public void EntityDestroyedHandler(Entity entity)
    {
        if (entity is GridEntity)
            entities.Add((GridEntity)entity);
    }

    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener<Entity>(Enums.EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.AddListener<Entity>(Enums.EventType.Entitydestroyed, EntityDestroyedHandler);
    }
    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener<Entity>(Enums.EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.RemoveListener<Entity>(Enums.EventType.Entitydestroyed, EntityDestroyedHandler);
    }

    #endregion Event Handlers

    #region Helper functions
    // Check if any entities inhabit a given grid position
    public bool HasEntitiesAtPosition(Vector3Int gridPosition, List<string> entityMask)
    {
        // Loop through each entity
        foreach (GridEntity entity in entities)
        {
            // Get the entity's position in world coordinates
            Vector3 entityPosition = entity.targetGridPosition;

            // Convert the entity's position to grid coordinates
            Vector3Int entityGridPosition = GetGridPositionFromWorldPoint(entityPosition);

            // Check if the entity is at the given grid position
            if (entityGridPosition == gridPosition && entityMask.Contains(entity.tag))
            {
                // There is an entity at the given position
                return true;
            }
        }
        // No entities found at the given position
        return false;
    }

    // Check if any entities inhabit a given grid position
    public bool HasEntitiesAtPosition(Vector3Int gridPosition)
    {
        // Loop through each entity
        foreach (GridEntity entity in entities)
        {
            // Get the entity's position in world coordinates
            Vector3 entityPosition = entity.targetGridPosition;

            // Convert the entity's position to grid coordinates
            Vector3Int entityGridPosition = GetGridPositionFromWorldPoint(entityPosition);

            // Check if the entity is at the given grid position
            if (entityGridPosition == gridPosition)
            {
                // There is an entity at the given position
                return true;
            }
        }
        // No entities found at the given position
        return false;
    }

    public Vector3Int GetGridPositionFromWorldPoint(Vector3 worldPosition)
    {
        return new Vector3Int(
            Mathf.FloorToInt(worldPosition.x),
            Mathf.FloorToInt(worldPosition.y),
            Mathf.FloorToInt(worldPosition.z)
        );
    }

    public Vector3 GetWorldPointFromGridPosition(Vector3Int gridPosition)
    {
        return new Vector3(gridPosition.x + 0.5f, gridPosition.y + 0.5f, gridPosition.z);
    }

    public List<Vector3Int> GetAdjacentGridPositions(Vector3Int gridPosition)
    {
        List<Vector3Int> adjacentPositions = new List<Vector3Int>
    {
        gridPosition + new Vector3Int(1, 0, 0),
        gridPosition + new Vector3Int(-1, 0, 0),
        gridPosition + new Vector3Int(0, 1, 0),
        gridPosition + new Vector3Int(0, -1, 0),
    };
        return adjacentPositions;
    }

    public bool IsFloorGridPositionEmpty(Vector3Int gridPosition)
    {
        if (HasEntitiesAtPosition(gridPosition) || !FloorTileExhists(gridPosition))
            return false;
        return true;
    }

    public bool FloorTileExhists(Vector3Int gridPosition)
    {
        // Check if there is a tile at the given position
        TileBase tile = floorTilemap.GetTile(floorTilemap.WorldToCell(gridPosition));
        if (tile != null)
        {
            return true; // Tile exists, not empty
        }
        // Both entity and tile are absent, position is empty
        return false;
    }



    #endregion Helper functions
}
