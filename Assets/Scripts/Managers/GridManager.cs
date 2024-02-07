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
    public Tilemap enemyAttackTilemap;
    public Tilemap castTilemap;

    // List of entities
    public List<GridEntity> entities = new List<GridEntity>();

    [Header("Tiles")]
    public Tile enemySelectionTile;
    public Tile castSelectionTile;

    private List<Vector3Int> selectedEnemyDangerPositions = new List<Vector3Int>();
    private List<Ability> enemyAbilitiesQueued = new List<Ability>();

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    public Entity GetEntityUnderMouse()
    {
        // Cast a ray from the mouse position to check if it hits the entity
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.transform.root.GetComponent<Entity>();
        }

        return null;
    }

    public void UpdateSelectedEnemyAttackTiles(List<Vector3Int> positions)
    {
        selectedEnemyDangerPositions = positions;
        UpdateEnemyAttackTiles();
    }

    public void UpdateEnemyAttackTiles()
    {
        BoundsInt bounds = floorTilemap.cellBounds;
        enemyAttackTilemap.ClearAllTiles();
        List<Vector3Int> selectedGridPositions = new List<Vector3Int>();

        foreach (var ability in enemyAbilitiesQueued)
            selectedGridPositions.AddRange(ability.GetAbilityPositions());

        foreach (var position in bounds.allPositionsWithin)
        {
            TileBase currentTile = floorTilemap.GetTile(position);

            // Selected tiles
            if (currentTile != null)
            {
                // Check if the position matches the selected grid position
                if (selectedGridPositions.Contains(position) || selectedEnemyDangerPositions.Contains(position))
                {
                    // Set the tile to the selected tile
                    enemyAttackTilemap.SetTile(position, enemySelectionTile);
                }
                else
                {
                    // Set all other tiles to the unselected tile
                    enemyAttackTilemap.SetTile(position, null);
                }
            }
        }
    }

    // Update the tilemap to reflect the selected tiles
    public void UpdateCastPositionsTilemap(List<Vector3Int> selectedGridPositions)
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
                    castTilemap.SetTile(position, enemySelectionTile);
                }
                else
                {
                    // Set all other tiles to the unselected tile
                    castTilemap.SetTile(position, null);
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

    public void AddQueuedAttackHandler(Ability ability)
    {
        enemyAbilitiesQueued.Add(ability);
        UpdateEnemyAttackTiles();
    }

    public void RemoveQueuedAttackHandler(Ability ability)
    {
        enemyAbilitiesQueued.Remove(ability);
        UpdateEnemyAttackTiles();
    }

    public void EndPlayerTurnHandler()
    {
        selectedEnemyDangerPositions.Clear();
        UpdateEnemyAttackTiles();
    }

    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener<Entity>(Enums.EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.AddListener<Entity>(Enums.EventType.EntityDestroyed, EntityDestroyedHandler);
        EventManager.Instance.AddListener<Ability>(Enums.EventType.AttackQueued, AddQueuedAttackHandler);
        EventManager.Instance.AddListener<Ability>(Enums.EventType.AttackDequeued, RemoveQueuedAttackHandler);
        EventManager.Instance.AddListener(Enums.EventType.EndPlayerTurn, EndPlayerTurnHandler);
    }

    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener<Entity>(Enums.EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.RemoveListener<Entity>(Enums.EventType.EntityDestroyed, EntityDestroyedHandler);
        EventManager.Instance.RemoveListener<Ability>(Enums.EventType.AttackQueued, AddQueuedAttackHandler);
        EventManager.Instance.RemoveListener<Ability>(Enums.EventType.AttackDequeued, RemoveQueuedAttackHandler);
        EventManager.Instance.RemoveListener(Enums.EventType.EndPlayerTurn, EndPlayerTurnHandler);
    }

    #endregion Event Handlers

    #region Helper functions

    public List<Vector3Int> GetGridPositionsWithinDistance(Vector3Int centerPosition, int distance)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                if (Mathf.Abs(x) + Mathf.Abs(y) > distance)
                    continue;

                Vector3Int gridPos = new Vector3Int(centerPosition.x + x, centerPosition.y + y);
                result.Add(gridPos);
            }
        }

        return result;
    }

    public List<Vector2Int> GetGridPositionsWithinDistance(Vector2Int centerPosition, int distance)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                // Skip positions outside the specified distance.
                if (Mathf.Abs(x) + Mathf.Abs(y) > distance)
                    continue;

                Vector2Int gridPos = new Vector2Int(centerPosition.x + x, centerPosition.y + y);
                result.Add(gridPos);
            }
        }

        return result;
    }


    public GridEntity GetEntityOnPosition(Vector3Int gridPosition)
    {
        return GetEntityOnPosition(gridPosition, new List<string>());
    }

    public GridEntity GetEntityOnPosition(Vector3Int gridPosition, List<string> entityMask)
    {
        Vector3Int pos = gridPosition * new Vector3Int(1, 1, 0);
        // Loop through each entity
        foreach (GridEntity entity in entities)
        {
            // Get the entity's position in world coordinates
            Vector3Int entityPosition = entity.targetGridPosition;

            // Convert the entity's position to grid coordinates
            Vector3Int entityGridPosition = GetGridPositionFromWorldPoint(entityPosition);

            // Check if the entity is at the given grid position
            if (pos.Equals(entityGridPosition) && (entityMask.Count == 0 || entityMask.Contains(entity.tag)))
            {
                return entity;
            }
        }
        return null;
    }

    public List<GridEntity> GetEntitiesOnPositions(List<Vector3Int> gridPositions)
    {
        return GetEntitiesOnPositions(gridPositions, new List<string>());
    }

    public List<GridEntity> GetEntitiesOnPositions(List<Vector3Int> gridPositions, List<string> entityMask)
    {
        List<GridEntity> results = new List<GridEntity>();
        // Loop through each entity
        foreach (GridEntity entity in entities)
        {
            // Get the entity's position in world coordinates
            Vector3 entityPosition = entity.targetGridPosition;

            // Convert the entity's position to grid coordinates
            Vector3Int entityGridPosition = GetGridPositionFromWorldPoint(entityPosition);

            // Check if the entity is at the given grid position
            if (((gridPositions.Contains(entityGridPosition) && entityMask.Count == 0)) || (gridPositions.Contains(entityGridPosition) && entityMask.Contains(entity.tag)))
            {
                results.Add(entity);
            }
        }
        return results;
    }

    public List<Vector3Int> GetValidPositionsInDistance(Vector3Int position, int distance, bool includingDiagonalMovement, List<string> entityMask)
    {
        List<Vector3Int> allPositions = GetPositionsInDistance(position, distance, includingDiagonalMovement);
        List<Vector3Int> results = new List<Vector3Int>();

        foreach (Vector3Int pos in allPositions)
        {
            int walkDistance = (FindPath(position, pos).Count - 1);
            if (walkDistance <= distance)
            {
                results.Add(pos);
            }
        }
        return results;
    }

    public List<Vector3Int> GetValidPositionsInDistance(Vector3Int position, int distance, bool includingDiagonalMovement = false)
    {
        return GetValidPositionsInDistance(position, distance, includingDiagonalMovement, new List<string>());
    }

    public int GetWalkingDistance(Vector3Int startPos, Vector3Int endPos)
    {
        return FindPath(startPos, endPos).Count - 1;
    }

    public List<Vector3Int> GetPositionsInDistance(Vector3Int position, int distance, bool includingDiagonalMovement)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        for (int i = -distance; i <= distance; i++)
        {
            for (int j = -distance; j <= distance; j++)
            {
                if (includingDiagonalMovement || (i != 0 && j != 0 && Mathf.Abs(i) + Mathf.Abs(j) <= distance))
                {
                    Vector3Int newPos = position + new Vector3Int(i, j, 0);
                    result.Add(newPos);
                }
            }
        }

        return result;
    }


    // Get positions in a grid from a starting position towards a specific direction at a certain distance
    public Vector3[] GetPositionsInDirection(Vector3 startPos, Vector3 direction, int distance)
    {
        if (distance < 0)
        {
            Debug.LogError("Distance cannot be negative.");
            return null;
        }

        // List to store the positions
        var positions = new List<Vector3>();

        for (int i = 1; i <= distance; i++)
        {
            Vector3 newPos = startPos + direction * i;
            positions.Add(newPos);
        }

        return positions.ToArray();
    }


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

    public List<Vector3Int> GetPositionsInCone(Vector3Int targetPosition, int distance, float coneAngle)
    {
        List<Vector3Int> positionsInCone = new List<Vector3Int>();

        // Iterate over a square area around the target position
        for (int i = -distance; i <= distance; i++)
        {
            for (int j = -distance; j <= distance; j++)
            {
                Vector3Int currentPosition = targetPosition + new Vector3Int(i, j, 0);

                // Calculate the angle between the current position and the target position
                float angleToCurrentPosition = Vector2.SignedAngle(Vector2.up, new Vector2(currentPosition.x - targetPosition.x, currentPosition.y - targetPosition.y));

                // Check if the current position is within the cone angle
                if (Mathf.Abs(angleToCurrentPosition) <= coneAngle / 2f)
                {
                    positionsInCone.Add(currentPosition);
                }
            }
        }

        return positionsInCone;
    }

    public List<Vector3Int> GetPositionsInArea(Vector3Int targetPosition, int areaSize)
    {
        List<Vector3Int> positionsInArea = new List<Vector3Int>();

        // Iterate over a square area around the target position
        for (int i = -areaSize; i <= areaSize; i++)
        {
            for (int j = -areaSize; j <= areaSize; j++)
            {
                Vector3Int currentPosition = targetPosition + new Vector3Int(i, j, 0);
                positionsInArea.Add(currentPosition);
            }
        }

        return positionsInArea;
    }

    public List<Vector3Int> GetPositionsInStraightLine(Vector3Int targetPosition, Vector2 direction, int distance)
    {
        List<Vector3Int> positionsInLine = new List<Vector3Int>();

        // Ensure the direction is normalized
        Vector2Int calculatedDirection = new Vector2Int(Mathf.RoundToInt(direction.normalized.x), Mathf.RoundToInt(direction.normalized.y));

        // Iterate over the specified distance in both horizontal and vertical directions
        for (int i = 1; i <= distance; i++)
        {
            // Horizontal movement
            Vector3Int horizontalPosition = targetPosition + new Vector3Int(calculatedDirection.x * i, 0, 0);
            positionsInLine.Add(horizontalPosition);

            // Vertical movement
            Vector3Int verticalPosition = targetPosition + new Vector3Int(0, calculatedDirection.y * i, 0);
            positionsInLine.Add(verticalPosition);

            // Diagonal movement
            Vector3Int diagonalPosition = targetPosition + new Vector3Int(calculatedDirection.x * i, calculatedDirection.y * i, 0);
            positionsInLine.Add(diagonalPosition);
        }

        return positionsInLine;
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
