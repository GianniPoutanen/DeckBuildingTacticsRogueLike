using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEntity : Entity
{
    [Header("Entity Energy and Pathing")]
    public int pathIndex = 0;
    public List<Vector3Int> currentPath = new List<Vector3Int>();
    public float moveSpeed = 5f; // Adjust this value to control movement speed
    public Vector3Int targetGridPosition;
    public Vector3Int lastTargetedDirection;

    public override void Start()
    {
        base.Start();
        // Set the initial target grid position to the current position
        targetGridPosition = GridManager.Instance.GetGridPositionFromWorldPoint(transform.position);
    }

    public virtual void Update()
    {
        // Move towards the target grid position
        MoveTowardsTarget();
    }

    public override void Destroy()
    {
        DestroyEntityAbility destroyAction = (DestroyEntityAbility)AbilityBuilder.GetBuilder(new DestroyEntityAbility()).SetPerformer(this).Build();
        destroyAction.Perform();
    }

    public void SetTargetDirectionBasedOnPosition(Vector3Int position)
    {
        // Convert to cardinal
    }

    private void MoveTowardsTarget()
    {
        // Calculate the distance between the current position and the target position
        float distance = Vector3.Distance(transform.position, targetGridPosition);

        // Use Lerp to smoothly interpolate between the current position and the target position
        transform.position = Vector3.Lerp(transform.position, targetGridPosition, moveSpeed * Time.deltaTime);
    }

    public override void TurnStart()
    {
        foreach (Status status in Statuses.Keys)
            if (Statuses[status] > 0)
                UndoRedoManager.Instance.AddUndoAction(new HandleStatusAction() { Performer = this, status = status });
        base.TurnStart();
    }

    #region Event Handlers
    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
    }

    public override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
    }
    #endregion Event Handlers
}
