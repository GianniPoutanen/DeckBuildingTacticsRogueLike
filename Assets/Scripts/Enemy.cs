using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : GridEntity
{
    public int maxEnergy = 1;
    public int currentEnergy;
    public int pathIndex = 0;
    public List<Vector3Int> currentPath = new List<Vector3Int>();

    public IEnumerator DoTurn()
    {
        pathIndex++;
        FindPathToPlayer();
        if (currentPath.Count > 0 && !GridManager.Instance.HasEntitiesAtPosition(currentPath.First()))
            StepTowardsGridPosition(currentPath.First());
        yield return null;
    }

    public void StepTowardsGridPosition(Vector3Int nextGridPosition)
    {
        MoveGridEntityAction action = new MoveGridEntityAction()
        {
            Performer = this,
            target = this,
            newPosition = nextGridPosition,
            oldPosition = targetGridPosition
        };

        UndoRedoManager.Instance.AddUndoAction(action);
        targetGridPosition = nextGridPosition;
    }

    void FindPathToPlayer()
    {
        Vector3Int startCell = GridManager.Instance.GetGridPositionFromWorldPoint(this.targetGridPosition);
        Vector3Int targetCell = GridManager.Instance.GetGridPositionFromWorldPoint(PlayerManager.Instance.Player.targetGridPosition);
        currentPath = GridManager.Instance.FindPath(startCell, targetCell, new List<string>());
        string path = "";
        foreach (var p in currentPath)
            path += p + " ";
        Debug.Log(path);
        pathIndex = 0;
    }


}
