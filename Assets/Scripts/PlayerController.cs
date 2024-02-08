using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : GridEntity
{
    int movementCost = 1;
    public override int Armour { get { return PlayerManager.Instance.armour; } set { PlayerManager.Instance.armour = value; } }
    public override int MaxHealth { get { return PlayerManager.Instance.maxPlayerHealth; } set { PlayerManager.Instance.maxPlayerHealth = value; } }
    public override int Health { get { return PlayerManager.Instance.playerHealth; } set { PlayerManager.Instance.playerHealth = value; } }

    // Current Turn
    public Stack<Ability> CurrentActions = new Stack<Ability>();


    public override void Update()
    {
        if (EventManager.Instance.EventsRunning)
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
            CompositeAction movePlayerAction = new CompositeAction()
            {
                actions = new List<Ability>(){new MoveSelfBuilder().SetPerformer(this).SetTargetPosition(newTargetGridPosition).Build(),
                                              new UseEnergyAction() { Performer = this, amount = movementCost } }
            };
            CurrentActions.Push(movePlayerAction);
            movePlayerAction.Perform();
            EventManager.Instance.InvokeEvent(EventType.UpdateUI);
        }
    }


    #region Undo Redo Action

    #endregion Undo Redo Action

    #region Evemt Handlers
    public override void SubscribeToEvents()
    {
        base.SubscribeToEvents();
        EventManager.Instance.AddListener(EventType.EndEnemyTurn, EndEnemyTurnHandler);
    }
    public override void UnsubscribeToEvents()
    {
        base.UnsubscribeToEvents();
        EventManager.Instance.RemoveListener(EventType.EndEnemyTurn, EndEnemyTurnHandler);
    }

    public void EndEnemyTurnHandler()
    {
        PlayerManager.Instance.State = PlayerStates.PlayerTurn;
    }
    #endregion Evemt Handlers
}