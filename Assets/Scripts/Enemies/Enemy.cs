using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Playables;
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
    public Ability[] AttacksInQueue;
    [SerializeField]
    public List<Attack> possibleAttacks = new List<Attack>();
    protected PlayerController Player
    {
        get { return PlayerManager.Instance.Player; }
    }

    public override void Start()
    {
        base.Start();

        foreach (Attack attack in possibleAttacks)
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
            UIManager.Instance.OpenUI(UIPanels.AttackPanel);
        }
    }
    public virtual IEnumerator DoTurn()
    {
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
                PerformMoveAction();
            }
            else
            {
                yield return StartCoroutine(PerformJabWithAttackInQueue());
            }
        }

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
        if (attacks.Count > 0)
        {
            int index = Random.Range(0, attacks.Count);
            (new EnqueuAttackAction(this,attacks[index])).Perform();
        }
    }

    public virtual void PerformMoveAction()
    {
        pathIndex++;
        FindPathToPlayer();
        if (currentPath.Count > 0 && !GridManager.Instance.HasEntitiesAtPosition(currentPath.First()))
            StepTowardsGridPosition(currentPath.First());
    }

    public virtual IEnumerator PerformJabWithAttackInQueue()
    {
        Vector3 jabDirection = attackQueue.Peek().TargetPosition + new Vector3(0.5f, 0.5f) - sprite.transform.position;
        jabDirection = (new Vector3(jabDirection.x, jabDirection.y, 0)).normalized;
        yield return StartCoroutine(JabCoroutine(jabDirection, jabSpeed, jabDistance, attackQueue.Dequeue()));
        GridManager.Instance.UpdateEnemyActionTiles();
    }

    public void StepTowardsGridPosition(Vector3Int nextGridPosition)
    {
        AbilityBuilder.GetBuilder(new MoveSelfAbility()).SetPerformer(this).SetTargetPosition(nextGridPosition).SetRange(1).Build().Perform();
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

    void SetUpAttack(Attack attack)
    {
        List<Ability> results = new List<Ability>();
        attack.triggerAbility = AbilityBuilder.GetBuilder(attack.triggerAbility).SetTargetPosition(Player.targetGridPosition).SetPerformer(this).Build();
        foreach (Ability ability in attack.followUpAbilities)
            results.Add(AbilityBuilder.GetBuilder(ability).SetTargetPosition(Player.targetGridPosition).SetPerformer(this).Build());
        attack.followUpAbilities = results;
    }
}
