using UnityEngine;
using System.Collections;

/// <summary>BehaviourTreeで敵を制御するクラス</summary>
public class EnemyAI : MonoBehaviour
{
    /// <summary></summary>
    [SerializeField] private EnemyController _enemy;

    /// <summary></summary>
    [SerializeField] private Transform _player;

    /// <summary></summary>
    [SerializeField] private float _moveSpeed = 0.5f;

    /// <summary></summary>
    [SerializeField] private float _fleeSpeed = 1f;

    /// <summary></summary>
    [SerializeField] private float _patrolRange = 5.0f;

    /// <summary></summary>
    [SerializeField] private float _fleeRange = 7.5f;

    /// <summary></summary>
    [SerializeField] private float _chaseRange = 1.5f;

    /// <summary></summary>
    [SerializeField] private float _searchRange = 5.0f;

    /// <summary></summary>
    private Transform _patrolPoint;

    /// <summary></summary>
    private Vector3 _previousPos;

    /// <summary></summary>
    private BaseNode _root;

    /// <summary></summary>
    private bool _canAttack = true;

    void Start()
    {
        _root = ConstructBehaviorTree();
        _patrolPoint = _enemy.GetRandomPatrolPoint(_patrolRange);
    }

    void Update()
    {
        if(_enemy.IsDead()) return;

        if (_root != null)
        {
            _root.Execute();
        }
    }

    private BaseNode ConstructBehaviorTree()
    {
        // 巡回アクション
        var patrolAction = new ActionNode(() =>
        {
            return _enemy.BTPatrol(_patrolPoint, _previousPos, _moveSpeed, _patrolRange);
        });

        // 逃走アクション
        var fleeAction = new ActionNode(() =>
        {
            return _enemy.BTFlee(_player, _previousPos, _fleeSpeed, _fleeRange);
        });

        // 追跡アクション
        var chaseAction = new ActionNode(() =>
        {
            return _enemy.BTChase(_player, _previousPos, _moveSpeed, _chaseRange);
        });

        // 攻撃アクション
        var attackAction = new ActionNode(() =>
        {
            if(_canAttack)
            {
                _enemy.BTAttack();
                StartCoroutine(AttackCooldown());
                return NodeStatus.Success;
            }
            else
            {
                return NodeStatus.Failure;
            }
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

        // ダメージを受けていないか
        var isNotDamaged = new ConditionNode(() =>
        _enemy.IsNotDamaged());

        // 攻撃シーケンス
        var attackSequence = new SequenceNode();
        attackSequence.AddChild(isPlayerClose);
        attackSequence.AddChild(hasEnoughHP);
        attackSequence.AddChild(isNotDamaged);
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

    IEnumerator AttackCooldown()
    {
        _canAttack = false;
        yield return new WaitForSeconds(1.0f);
        _canAttack = true;
    }
}
