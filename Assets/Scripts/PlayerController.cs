using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : GridEntity
{
    int movementCost = 1;

    // Current Turn
    private Stack<IUndoRedoAction> currentTurn = new Stack<IUndoRedoAction>();


    private enum PlayerStates
    {
        Waiting,
        PlayerTurn,
    }
    [SerializeField]
    private PlayerStates _state = PlayerStates.PlayerTurn;

    public override void Update()
    {
        if(EventManager.Instance.EventsRunning)
        {
            return;
        }
        base.Update();
        // Check for keyboard input and update the target grid position accordingly
        if (_state == PlayerStates.PlayerTurn)
        {
            HandleInput();
            // Example: Undo and redo actions using keyboard shortcuts
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (currentTurn.Count > 0)
                {
                    currentTurn.Pop().Undo();
                }
                else
                {
                    UndoRedoManager.Instance.UndoToPlayer();
                    PlayerManager.Instance.currentEnergy = PlayerManager.Instance.maxEnergy;
                }
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                UndoRedoManager.Instance.Redo();
            }
        }

        CheckEndTurn();
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
            currentTurn.Push(new CompositeAction(
                new List<IUndoRedoAction>() { new MoveGridEntityAction(this, targetGridPosition, newTargetGridPosition),
                                              new UseEnergyAction(PlayerManager.Instance.currentEnergy - movementCost, PlayerManager.Instance.currentEnergy) },
                this));
            PlayerManager.Instance.currentEnergy -= movementCost;
            targetGridPosition = newTargetGridPosition;
            EventManager.Instance.InvokeEvent(Enums.EventType.UpdateUI);
        }
    }

    private void CheckEndTurn()
    {
        if (PlayerManager.Instance.currentEnergy == 0 && _state != PlayerStates.Waiting)
        {
            PlayerEndTurn();
        }
    }

    private void PlayerEndTurn()
    {
        _state = PlayerStates.Waiting;
        UndoRedoManager.Instance.AddUndoAction(new CompositeAction(currentTurn.ToList(), this));
        currentTurn = new Stack<IUndoRedoAction>();
        EventManager.Instance.InvokeEvent(Enums.EventType.EndPlayerTurn);
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
        _state = PlayerStates.PlayerTurn;
        PlayerManager.Instance.currentEnergy = PlayerManager.Instance.maxEnergy;
    }
    #endregion Evemt Handlers
}