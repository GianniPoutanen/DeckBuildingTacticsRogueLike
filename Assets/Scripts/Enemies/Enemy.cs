using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : GridEntity
{
    [Header("Entity Energy and Pathing")]
    public int maxEnergy = 1;
    public int currentEnergy;
    public int pathIndex = 0;
    public List<Vector3Int> currentPath = new List<Vector3Int>();

    [Header("Attacking Variables")]
    [SerializeField]
    public Queue<Ability> attackQueue = new Queue<Ability>();
    public int damage = 3;

    [SerializeField]
    public EnemyAttack quickAttack;
    [SerializeField]
    public EnemyAttack[] possibleAttacks;
    protected PlayerController Player
    {
        get { return PlayerManager.Instance.Player; }
    }


    public override void Start()
    {
        base.Start();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        if (attackQueue.Count > 0)
        {
            Ability ability = attackQueue.Dequeue();
            EventManager.Instance.InvokeEvent(EventType.AttackDequeued, ability);
        }
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridManager.Instance.UpdateSelectedEnemyAttackTiles(quickAttack.triggerAbility.GetPossiblePositions(targetGridPosition));
        }
    }
    public virtual IEnumerator DoTurn()
    {
        if (attackQueue.Count > 0)
        {
            // Move towards player default
            Ability ability = attackQueue.Dequeue();
            ability.Perform();
            EventManager.Instance.InvokeEvent(EventType.AttackDequeued, ability);
        }
        else
        {
            List<EnemyAttack> possibleAttacks = new List<EnemyAttack>();
            foreach(EnemyAttack attack in possibleAttacks)
            {
                if (attack.triggerAbility.CanPerform(Player.targetGridPosition))
                    possibleAttacks.Add(attack);
            }
            if (possibleAttacks.Count > 0)
            {
                int index = Random.Range(0, possibleAttacks.Count);
                AbilityBuilder abilityBuilder = AbilityBuilder.GetBuilder(possibleAttacks[index].triggerAbility);
                abilityBuilder.SetTargetPosition(Player.targetGridPosition).SetPerformer(this).Build().Perform();
                foreach (Ability ability in possibleAttacks[index].followUpAbilities)
                    attackQueue.Enqueue(AbilityBuilder.GetBuilder(ability).SetPerformer(this).SetTargetPosition(Player.targetGridPosition).Build());
            }
            else
            {
                // Move towards player default
                PerformMoveAction();
            }
        }

        AfterTurnCheckCanAttack();
        yield return null;
    }

    public virtual void AfterTurnCheckCanAttack()
    {
        List<EnemyAttack> possibleAttacks = new List<EnemyAttack>();
        foreach (EnemyAttack attack in possibleAttacks)
        {
            if (attack.triggerAbility.CanPerform(Player.targetGridPosition))
                possibleAttacks.Add(attack);
        }
        if (possibleAttacks.Count > 0)
        {
            int index = Random.Range(0, possibleAttacks.Count);
            AbilityBuilder abilityBuilder = AbilityBuilder.GetBuilder(possibleAttacks[index].triggerAbility);
            attackQueue.Enqueue(abilityBuilder.SetTargetPosition(Player.targetGridPosition).SetPerformer(this).Build());
            foreach (Ability ability in possibleAttacks[index].followUpAbilities)
                attackQueue.Enqueue(AbilityBuilder.GetBuilder(ability).SetPerformer(this).SetTargetPosition(Player.targetGridPosition).Build());
        }
    }

    public virtual void PerformMoveAction()
    {
        pathIndex++;
        FindPathToPlayer();
        if (currentPath.Count > 0 && !GridManager.Instance.HasEntitiesAtPosition(currentPath.First()))
            StepTowardsGridPosition(currentPath.First());
    }

    public void StepTowardsGridPosition(Vector3Int nextGridPosition)
    {
        Ability action = (new MoveSelfBuilder()).SetPerformer(this).SetTargetPosition(nextGridPosition).SetRange(1).Build();
        action.Perform();
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
