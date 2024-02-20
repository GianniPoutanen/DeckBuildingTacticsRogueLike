using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class Enemy : GridEntity
{

    [Header("Attacking Variables")]
    [SerializeField]
    public Queue<Ability> attackQueue = new Queue<Ability>();
    public int damage = 3;
    [SerializeField]
    public Ability[] AttacksInQueue;
    [SerializeField]
    public List<Attack> possibleAttacks = new List<Attack>();
    [SerializeField]
    public List<Attack> lastCheckedPossibleAttacks = new List<Attack>();
    protected PlayerController Player
    {
        get { return PlayerManager.Instance.Player; }
    }

    public override void Start()
    {
        base.Start();

        foreach (Attack attack in possibleAttacks)
            SetUpAttack(attack);

        foreach (Attack attack in lastCheckedPossibleAttacks)
            SetUpAttack(attack);
    }

    public override void Update()
    {
        base.Update();
        AttacksInQueue = attackQueue.ToArray();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AttacksPanelSingleton.Instance.Attacks = possibleAttacks;
            AttacksPanelSingleton.Instance.LastCheckedAttacks = lastCheckedPossibleAttacks;
            UIManager.Instance.OpenUI(UIPanels.AttackPanel);
        }
    }
    public virtual IEnumerator DoTurn()
    {
        for (currentEnergy = 0; currentEnergy < maxEnergy;)
        {
            yield return new WaitForSeconds(0.1f);
            if (attackQueue.Count > 0)
            {
                // Move towards player default
                yield return PerformJabWithAttackInQueue();
                GridManager.Instance.UpdateEnemyActionTiles();
            }
            else
            {
                TryQueueAttack();

                if (attackQueue.Count == 0)
                {
                    // Move towards player default
                    yield return StartCoroutine(PerformMoveAction());
                }
                else
                {
                    yield return StartCoroutine(PerformJabWithAttackInQueue());
                }
            }
        }
        yield return new WaitForSeconds(0.1f);
        yield return null;
    }

    public virtual void TryQueueAttack()
    {
        List<Attack> attacks = new List<Attack>();
        foreach (Attack attack in possibleAttacks)
        {
            AbilityBuilder.GetBuilder(attack.triggerAbility).SetPerformer(this).Build();
            if (attack.triggerAbility.CanPerform(Player.targetGridPosition))
                attacks.Add(attack);
        }
        if (attacks.Count == 0)
            foreach (Attack attack in lastCheckedPossibleAttacks)
            {
                AbilityBuilder.GetBuilder(attack.triggerAbility).SetPerformer(this).Build();
                if (attack.triggerAbility.CanPerform(Player.targetGridPosition))
                    attacks.Add(attack);
            }
        if (attacks.Count > 0)
        {
            int index = Random.Range(0, attacks.Count);
            (new EnqueuAttackAction(this, attacks[index])).Perform();
        }
    }

    public virtual IEnumerator PerformMoveAction()
    {
        pathIndex++;
        FindPathToPlayer();
        if (currentPath.Count > 0)
            StepTowardsGridPosition(currentPath);
        else
            currentEnergy++;

        yield return null;
    }


    public virtual IEnumerator PerformJabWithAttackInQueue()
    {
        Vector3 jabDirection = attackQueue.Peek().TargetPosition + new Vector3(0.5f, 0.5f) - sprite.transform.position;
        jabDirection = (new Vector3(jabDirection.x, jabDirection.y, 0)).normalized;
        yield return StartCoroutine(JabCoroutine(jabDirection, jabSpeed, jabDistance, attackQueue.Dequeue()));
        GridManager.Instance.UpdateEnemyActionTiles();
    }

    public void StepTowardsGridPosition(List<Vector3Int> path)
    {
        Vector3Int nextGridPosition = path.First();
        path.Reverse();
        foreach (var pos in path)
        {
            if (Vector3Int.Distance(pos, this.targetGridPosition) <= maxEnergy - currentEnergy)
            {
                nextGridPosition = pos;
                break;
            }
        }
        if (nextGridPosition != null && !GridManager.Instance.HasEntitiesAtPosition(nextGridPosition))
            AbilityBuilder.GetBuilder(new MoveSelfAbility()).SetPerformer(this).SetTargetPosition(nextGridPosition).SetRange(1).SetCost((int)Vector3Int.Distance(nextGridPosition, this.targetGridPosition)).Build().Perform();
        else
            currentEnergy++;
    }

    void FindPathToPlayer()
    {
        Vector3Int startCell = GridManager.Instance.GetGridPositionFromWorldPoint(this.targetGridPosition);
        Vector3Int targetCell = GridManager.Instance.GetClosestEmptyNeighbour(this.targetGridPosition, PlayerManager.Instance.Player.targetGridPosition);
        currentPath = GridManager.Instance.FindPath(startCell, targetCell, new List<string>() { "Player"} );
        string path = "";
        foreach (var p in currentPath)
            path += p + " ";
        Debug.Log(path);
        pathIndex = 0;
    }

    void SetUpAttack(Attack attack)
    {
        List<Ability> results = new List<Ability>();
        attack.triggerAbility = AbilityBuilder.GetBuilder(attack.triggerAbility).SetTargetPosition(Player.targetGridPosition).SetPerformer(this).Build();
        foreach (Ability ability in attack.followUpAbilities)
            results.Add(AbilityBuilder.GetBuilder(ability).SetTargetPosition(Player.targetGridPosition).SetPerformer(this).Build());
        attack.followUpAbilities = results;
    }
}
