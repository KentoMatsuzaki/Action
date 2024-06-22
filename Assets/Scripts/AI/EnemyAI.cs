using UnityEngine;
using System.Collections;

/// <summary>BehaviourTree�œG�𐧌䂷��N���X</summary>
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
        // ����A�N�V����
        var patrolAction = new ActionNode(() =>
        {
            return _enemy.BTPatrol(_patrolPoint, _previousPos, _moveSpeed, _patrolRange);
        });

        // �����A�N�V����
        var fleeAction = new ActionNode(() =>
        {
            return _enemy.BTFlee(_player, _previousPos, _fleeSpeed, _fleeRange);
        });

        // �ǐՃA�N�V����
        var chaseAction = new ActionNode(() =>
        {
            return _enemy.BTChase(_player, _previousPos, _moveSpeed, _chaseRange);
        });

        // �U���A�N�V����
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

        // �v���C���[���߂��ɂ��邩
        var isPlayerClose = new ConditionNode(() => 
        _enemy.IsPlayerClose(_player, _searchRange));

        // �v���C���[�������ɂ��邩
        var isPlayerAway = new ConditionNode(() => 
        _enemy.IsPlayerAway(_player, _searchRange));

        // �\���ȑ̗͂����邩
        var hasEnoughHP = new ConditionNode(() =>
        _enemy.HasEnoughHP());

        // �̗͂��s�����Ă��邩
        var doesNotHaveEnoughHP = new ConditionNode(() =>
        _enemy.DoesNotHaveEnoughHP());

        // �_���[�W���󂯂Ă��Ȃ���
        var isNotDamaged = new ConditionNode(() =>
        _enemy.IsNotDamaged());

        // �U���V�[�P���X
        var attackSequence = new SequenceNode();
        attackSequence.AddChild(isPlayerClose);
        attackSequence.AddChild(hasEnoughHP);
        attackSequence.AddChild(isNotDamaged);
        attackSequence.AddChild(chaseAction);
        attackSequence.AddChild(attackAction);

        // �����V�[�P���X
        var fleeSequence = new SequenceNode();
        fleeSequence.AddChild(isPlayerClose);
        fleeSequence.AddChild(doesNotHaveEnoughHP);
        fleeSequence.AddChild(fleeAction);
        
        // ����V�[�P���X
        var patrolSequence = new SequenceNode();
        patrolSequence.AddChild(isPlayerAway);
        patrolSequence.AddChild(patrolAction);

        // �Z���N�^�[
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
