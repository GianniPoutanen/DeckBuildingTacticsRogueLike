using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Enums;

public class PlayerController : GridEntity
{
    int movementCost = 1;

    // Current Turn
    public Stack<IUndoRedoAction> CurrentActions = new Stack<IUndoRedoAction>();


    public override void Update()
    {
        if(EventManager.Instance.EventsRunning)
        {
            return;
        }
        base.Update();
        // Check for keyboard input and update the target grid position accordingly
        if (PlayerManager.Instance.State == PlayerStates.PlayerTurn)
        {
            HandleInput();
            // Example: Undo and redo actions using keyboard shortcuts
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (CurrentActions.Count > 0)
                {
                    CurrentActions.Pop().Undo();
                }
                else
                {
                    UndoRedoManager.Instance.UndoToPlayer();
                    PlayerManager.Instance.CurrentEnergy = PlayerManager.Instance.maxEnergy;
                }
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                UndoRedoManager.Instance.Redo();
            }
        }
    }

    private void HandleInput()
    {
        // Check for button press and update the target grid position accordingly
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            TryMove(Vector3Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            TryMove(Vector3Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            TryMove(Vector3Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            TryMove(Vector3Int.right);
        }
    }

    private void TryMove(Vector3Int direction)
    {
        // Calculate the new target grid position
        Vector3Int newTargetGridPosition = targetGridPosition + direction;
        // Check if the new target position is valid
        if (GridManager.Instance.IsFloorGridPositionEmpty(newTargetGridPosition))
        {
            CurrentActions.Push(new CompositeAction(
                new List<IUndoRedoAction>() { new MoveGridEntityAction(this, targetGridPosition, newTargetGridPosition),
                                              new UseEnergyAction(PlayerManager.Instance.CurrentEnergy - movementCost, PlayerManager.Instance.CurrentEnergy) },
                this));
            PlayerManager.Instance.CurrentEnergy -= movementCost;
            targetGridPosition = newTargetGridPosition;
            EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
        }
    }


    #region Undo Redo Action

    #endregion Undo Redo Action

    #region Evemt Handlers
    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        EventManager.Instance.AddListener(Enums.EventType.EndEnemyTurn, EndEnemyTurnHandler);
    }
    public override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        EventManager.Instance.RemoveListener(Enums.EventType.EndEnemyTurn, EndEnemyTurnHandler);
    }

    public void EndEnemyTurnHandler()
    {
        PlayerManager.Instance.State = PlayerStates.PlayerTurn;
        PlayerManager.Instance.CurrentEnergy = PlayerManager.Instance.maxEnergy;
    }
    #endregion Evemt Handlers
}