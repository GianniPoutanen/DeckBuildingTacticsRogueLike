using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    #endregion

    // Tile Maps
    public Tilemap floorTilemap;
    public Tilemap selectionTilemap;
    // List of entities
    public List<GridEntity> entities = new List<GridEntity>();

    [Header("Tiles")]
    public Tile selectedTile;
    public Tile unselectedTile;



    private void Start()
    {
        EventManager.Instance.AddListener<Entity>(Enums.EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.AddListener<Entity>(Enums.EventType.Entitydestroyed, EntityDestroyedHandler);
    }

    void Update()
    {
        // Example: Set the selected grid position on mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int selectedGridPosition = GetGridPositionFromWorldPoint(mouseWorldPos);
            List<Vector3Int> selectedGridPositions = new List<Vector3Int>();
            selectedGridPositions.Add(new Vector3Int(selectedGridPosition.x, selectedGridPosition.y, 0));
            UpdateSelectedTilemap(selectedGridPositions);
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener<Entity>(Enums.EventType.EntitySpawned, EntitySpawnedHandler);
        EventManager.Instance.RemoveListener<Entity>(Enums.EventType.Entitydestroyed, EntityDestroyedHandler);
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

    #endregion Event Handlers

    #region Helper functions

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
