using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

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
    [Header(" Tile Maps")]
    public Tilemap floorTilemap;
    public Tilemap enemyAttackTilemap;
    public Tilemap enemyMovementTilemap;
    public Tilemap castTilemap;

    // List of entities
    [Header(" Entities on Grid")]
    public List<GridEntity> entities = new List<GridEntity>();

    [Header("Tiles")]
    public Tile enemyAttackTile;
    public Tile enemyMoveTile;
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
        UpdateEnemyActionTiles();
    }


    public void UpdateEnemyActionTiles()
    {
        List<Vector3Int> selectedAttackGridPositions = new List<Vector3Int>();
        List<Vector3Int> selectedMoveGridPositions = new List<Vector3Int>();
        foreach (Entity entity in entities)
        {
            if (entity is Enemy)
            {
                Enemy enemy = (Enemy)entity;
                if (enemy.attackQueue.Count > 0)
                {
                    AbilityBuilder.GetBuilder(enemy.attackQueue.Peek()).SetPerformer(enemy);
                    selectedAttackGridPositions.AddRange(enemy.attackQueue.Peek().GetAbilityPositions());
                }
            }
        }
        foreach (var pos in selectedAttackGridPositions)
            Debug.Log(pos);
        BoundsInt bounds = floorTilemap.cellBounds;
        foreach (var position in bounds.allPositionsWithin)
        {
            TileBase currentTile = floorTilemap.GetTile(position);

            // Selected tiles
            if (currentTile != null)
            {
                // Check if the position matches the selected grid position
                if (selectedAttackGridPositions.Contains(position) || selectedEnemyDangerPositions.Contains(position))
                {
                    // Set the tile to the selected tile
                    enemyAttackTilemap.SetTile(position, enemyAttackTile);
                }
                else
                {
                    // Set all other tiles to the unselected tile
                    enemyAttackTilemap.SetTile(position, null);
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
            entities.Remove((GridEntity)entity);
    }

    public void AddQueuedAttackHandler(Ability ability)
    {
        enemyAbilitiesQueued.Add(ability);
        UpdateEnemyActionTiles();
    }

    public void RemoveQueuedAttackHandler(Ability ability)
    {
        enemyAbilitiesQueued.Remove(ability);
        UpdateEnemyActionTiles();
    }

    public void EndPlayerTurnHandler()
    {
        selectedEnemyDangerPositions.Clear();
        UpdateEnemyActionTiles();
    }

    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener<Entity>(EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.AddListener<Entity>(EventType.EntityDestroyed, EntityDestroyedHandler);
        EventManager.Instance.AddListener<Ability>(EventType.AttackQueued, AddQueuedAttackHandler);
        EventManager.Instance.AddListener<Ability>(EventType.AttackDequeued, RemoveQueuedAttackHandler);
        EventManager.Instance.AddListener(EventType.EndPlayerTurn, EndPlayerTurnHandler);
    }

    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener<Entity>(EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.RemoveListener<Entity>(EventType.EntityDestroyed, EntityDestroyedHandler);
        EventManager.Instance.RemoveListener<Ability>(EventType.AttackQueued, AddQueuedAttackHandler);
        EventManager.Instance.RemoveListener<Ability>(EventType.AttackDequeued, RemoveQueuedAttackHandler);
        EventManager.Instance.RemoveListener(EventType.EndPlayerTurn, EndPlayerTurnHandler);
    }

    #endregion Event Handlers

    #region Helper functions

    public void ClearAllSelectionTilemaps()
    {
        castTilemap.ClearAllTiles();
        enemyAttackTilemap.ClearAllTiles();
        enemyMovementTilemap.ClearAllTiles();
    }

    public void HighlightSelectedPositions(List<Vector3Int> positions, TileMapType mapType, TileType tileType)
    {
        Tilemap map = GetTilemap(mapType);
        TileBase tile = GetTile(tileType);
        BoundsInt bounds = floorTilemap.cellBounds;
        foreach (var position in bounds.allPositionsWithin)
        {
            TileBase currentTile = floorTilemap.GetTile(position);

            // Selected tiles
            if (currentTile != null)
            {
                // Check if the position matches the selected grid position
                if (positions.Contains(position))
                {
                    // Set the tile to the selected tile
                    map.SetTile(position, tile);
                }
                else
                {
                    // Set all other tiles to the unselected tile
                    map.SetTile(position, null);
                }
            }
        }
    }

    public static Vector3Int RoundToCardinal(Vector3 inputDirection)
    {
        // Calculate the angle in radians
        float angle = Mathf.Atan2(inputDirection.y, inputDirection.x);

        // Convert the angle to degrees
        float angleDegrees = Mathf.Rad2Deg * angle;

        // Round the angle to the nearest multiple of 90 degrees
        float roundedAngleDegrees = Mathf.Round(angleDegrees / 90) * 90;

        // Convert the rounded angle back to radians
        float roundedAngle = Mathf.Deg2Rad * roundedAngleDegrees;

        // Calculate the rounded direction vector
        Vector3Int roundedDirection = new Vector3Int((int)Mathf.Cos(roundedAngle), (int)Mathf.Sin(roundedAngle), 0);

        return roundedDirection;
    }

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

    public bool IsEntityOnPosition(Vector3Int gridPosition)
    {
        return IsEntityOnPosition(gridPosition, new List<string>());
    }

    public bool IsEntityOnPosition(Vector3Int gridPosition, List<string> entityMask)
    {
        return GetEntityOnPosition(gridPosition, entityMask) != null;
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
            if (entity.enabled)
            {
                // Get the entity's position in world coordinates
                Vector3Int entityPosition = entity.targetGridPosition;

                // Convert the entity's position to grid coordinates
                Vector3Int entityGridPosition = GetGridPositionFromWorldPoint(entityPosition);

                // Check if the entity is at the given grid position
                if (pos.Equals(entityGridPosition) && (entityMask == null || entityMask.Count == 0 || entityMask.Contains(entity.tag)))
                {
                    return entity;
                }
            }
        }
        return null;
    }

    public bool IsEntityOnPositions(List<Vector3Int> gridPosition)
    {
        return IsEntityOnPositions(gridPosition, new List<string>());
    }

    public bool IsEntityOnPositions(List<Vector3Int> gridPosition, List<string> entityMask)
    {
        return GetEntitiesOnPositions(gridPosition, entityMask).Count > 0;
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
            if (gridPositions.Contains(entityGridPosition) && (entityMask.Count == 0 || entityMask.Contains(entity.tag)))
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
        return FindPath(startPos, endPos).Count;
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


    /// <summary>
    /// Get positions in a grid from a starting position towards a specific direction at a certain distance
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="direction"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public List<Vector3Int> GetPositionsInDirection(Vector3Int startPos, Vector3Int direction, int distance, int deadZoneRange = 0)
    {
        if (distance < 0)
        {
            Debug.LogError("Distance cannot be negative.");
            return null;
        }

        // List to store the positions
        var positions = new List<Vector3Int>();

        for (int i = 1 + deadZoneRange; i <= distance; i++)
        {
            Vector3Int newPos = startPos + direction * i;
            // Return if hit end
            if (!floorTilemap.HasTile(newPos))
                return positions;

            positions.Add(newPos);
        }

        return positions;
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

    public Vector3Int GetClosestEmptyNeighbour(Vector3Int originPosition, Vector3Int worldPosition)
    {
        List<Vector3Int> possiblePositions = GetNeighbours(worldPosition);
        List<Vector3Int> checkPositions = new List<Vector3Int>();
        Vector3Int emptyEnemyPos = originPosition;
        int checkAmount = 0;
        while (emptyEnemyPos.Equals(originPosition) && checkAmount < 100)
        {
            emptyEnemyPos = GetClosestPositionToOrigin(originPosition, possiblePositions.Where(x => !IsEntityOnPosition(x, new List<string>() { "Enemy" })).ToList());
            if (emptyEnemyPos != originPosition)
                return emptyEnemyPos;
            checkPositions.AddRange(possiblePositions);
            possiblePositions.Clear();
            checkPositions.ForEach(x => possiblePositions.AddRange(GetNeighbours(x).Where(y => !checkPositions.Contains(y))));
            checkAmount++;
        }

        return originPosition;
    }

    public List<Vector3Int> GetNeighbours(Vector3Int worldPosition)
    {
        List<Vector3Int> neighbours = new List<Vector3Int>()
        {
            worldPosition + new Vector3Int(1,0),
            worldPosition + new Vector3Int(-1,0),
            worldPosition + new Vector3Int(0,1),
            worldPosition + new Vector3Int(0,-1)
        };
        return neighbours;
    }

    public Vector3Int GetClosestPositionToOrigin(Vector3Int originPosition, List<Vector3Int> positions)
    {
        Vector3Int furthestStep = Vector3Int.one * 9999;
        foreach (var possibleStep in positions)
        {
            if (Vector3Int.Distance(originPosition, possibleStep) < Vector3Int.Distance(originPosition, furthestStep))
            {
                furthestStep = possibleStep;
            }
        }
        if (furthestStep == Vector3Int.one * 9999)
            return originPosition;
        return furthestStep;
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

    public Tilemap GetTilemap(TileMapType type)
    {
        switch (type)
        {
            case TileMapType.CastPositions:
                return castTilemap;
            case TileMapType.EnemyMovePositions:
                return enemyMovementTilemap;
            case TileMapType.EnemyAttackPositions:
                return enemyAttackTilemap;
            default:
                return floorTilemap;
        }
    }
    public TileBase GetTile(TileType type)
    {
        switch (type)
        {
            case TileType.CastTile:
                return castSelectionTile;
            case TileType.EnemyAttackTile:
                return enemyAttackTile;
            case TileType.EnemyMoveTile:
                return enemyMoveTile;
            default:
                return castSelectionTile;
        }
    }

    #endregion Helper functions
}
