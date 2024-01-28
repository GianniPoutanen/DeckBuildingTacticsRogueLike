using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : GridEntity
{
    public int pathIndex = 0;
    public List<Vector3Int> currentPath = new List<Vector3Int>();
    public IEnumerator DoTurn()
    {
        pathIndex++;

        if (currentPath.Count == 0 || GridManager.Instance.GetGridPositionFromWorldPoint(PlayerManager.Instance.player.transform.position) != currentPath.Last())
            FindPathToPlayer();
        StepTowardsGridPosition(currentPath[pathIndex]);
        yield return null;
    }

    public void StepTowardsGridPosition(Vector3Int nextGridPosition)
    {
        UndoRedoManager.Instance.AddUndoAction(new MoveGridEntity(this, targetGridPosition, nextGridPosition));
        targetGridPosition = nextGridPosition;
    }

    void FindPathToPlayer()
    {
        Vector3Int startCell = GridManager.Instance.GetGridPositionFromWorldPoint(transform.position);
        Vector3Int targetCell = GridManager.Instance.GetGridPositionFromWorldPoint(PlayerManager.Instance.player.transform.position);

        currentPath = GridManager.Instance.FindPath(startCell, targetCell);
        pathIndex = 0;
    }


}
