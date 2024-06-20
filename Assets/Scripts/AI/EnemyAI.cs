using UnityEngine;

/// <summary>BehaviourTreeで敵を制御するクラス</summary>
public class EnemyAI : MonoBehaviour
{
    /// <summary></summary>
    private BaseNode _root;

    /// <summary></summary>
    private EnemyController _enemy;

    void Start()
    {
        _enemy = GetComponent<EnemyController>();
    }

    void Update()
    {
        
    }

    //private BaseNode ConstructBehaviorTree()
    //{
    //    // アクションの定義
    //    var patrolAction = new ActionNode(() =>
    //    {
    //        _enemy.Patrol();
    //        return NodeStatus.Running;
    //    });

    //    var escapeAction = new ActionNode(() =>
    //    {
    //        _enemy.Escape();
    //        return NodeStatus.Running;
    //    });

    //    var chaseAction = new ActionNode(() =>
    //    {
    //        return _enemy.Chase(player);
    //    });

    //    var attackAction = new ActionNode(() =>
    //    {
    //        return _enemy.Attack();
    //    });

    //    // 条件の定義
    //    var isPlayerVisible = new ConditionNode(() => enemy.IsPlayerInRange(player, detectionRange));
    //    var isPlayerInAttackRange = new ConditionNode(() => enemy.IsPlayerInRange(player, attackRange));

    //    // シーケンスやセレクターの定義
    //    var attackSequence = new SequenceNode();
    //    attackSequence.AddChild(isPlayerInAttackRange);
    //    attackSequence.AddChild(attackAction);

    //    var chaseSequence = new SequenceNode();
    //    chaseSequence.AddChild(isPlayerVisible);
    //    chaseSequence.AddChild(chaseAction);

    //    var rootSelector = new SelectorNode();
    //    rootSelector.AddChild(attackSequence);
    //    rootSelector.AddChild(chaseSequence);
    //    rootSelector.AddChild(patrolAction);

    //    return rootSelector;
    //}
}
