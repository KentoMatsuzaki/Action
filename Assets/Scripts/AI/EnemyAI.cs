using UnityEngine;

/// <summary>BehaviourTreeで敵を制御するクラス</summary>
public class EnemyAI : MonoBehaviour
{
    /// <summary></summary>
    [SerializeField] private EnemyController _enemy;

    /// <summary></summary>
    [SerializeField] private Transform _player;

    /// <summary></summary>
    [SerializeField] private float _speed;

    /// <summary></summary>
    [SerializeField] private float _patrolRange;

    /// <summary></summary>
    [SerializeField] private float _fleeRange;

    /// <summary></summary>
    [SerializeField] private float _chaseRange;

    /// <summary></summary>
    [SerializeField] private float _searchRange;

    /// <summary></summary>
    private Transform _patrolPoint;

    /// <summary></summary>
    private Vector3 _previousPos;

    /// <summary></summary>
    private BaseNode _root;

    void Start()
    {
        _enemy = GetComponent<EnemyController>();
    }

    void Update()
    {
        
    }

    private BaseNode ConstructBehaviorTree()
    {
        // 巡回アクション
        var patrolAction = new ActionNode(() =>
        {
            return _enemy.BTPatrol(_patrolPoint, _previousPos, _speed, _patrolRange);
        });

        // 逃走アクション
        var fleeAction = new ActionNode(() =>
        {
            return _enemy.BTFlee(_player, _previousPos, _speed, _fleeRange);
        });

        // 追跡アクション
        var chaseAction = new ActionNode(() =>
        {
            return _enemy.BTChase(_player, _previousPos, _speed, _chaseRange);
        });

        // 攻撃アクション
        var attackAction = new ActionNode(() =>
        {
            return _enemy.BTAttack();
        });

        // プレイヤーが近くにいるか
        var isPlayerClose = new ConditionNode(() => 
        _enemy.IsPlayerClose(_player, _searchRange));

        // プレイヤーが遠くにいるか
        var isPlayerAway = new ConditionNode(() => 
        _enemy.IsPlayerAway(_player, _searchRange));

        // 十分な体力があるか
        var hasEnoughHP = new ConditionNode(() =>
        _enemy.HasEnoughHP());

        // 体力が不足しているか
        var doesNotHaveEnoughHP = new ConditionNode(() =>
        _enemy.DoesNotHaveEnoughHP());

        // 攻撃シーケンス
        var attackSequence = new SequenceNode();
        attackSequence.AddChild(isPlayerClose);
        attackSequence.AddChild(hasEnoughHP);
        attackSequence.AddChild(chaseAction);
        attackSequence.AddChild(attackAction);

        // 逃走シーケンス
        var fleeSequence = new SequenceNode();
        fleeSequence.AddChild(isPlayerClose);
        fleeSequence.AddChild(doesNotHaveEnoughHP);
        fleeSequence.AddChild(fleeAction);
        
        // 巡回シーケンス
        var patrolSequence = new SequenceNode();
        patrolSequence.AddChild(isPlayerAway);
        patrolSequence.AddChild(patrolAction);

        // セレクター
        var rootSelector = new SelectorNode();
        rootSelector.AddChild(attackSequence);
        rootSelector.AddChild(fleeSequence);
        rootSelector.AddChild(patrolSequence);

        return rootSelector;
    }
}
