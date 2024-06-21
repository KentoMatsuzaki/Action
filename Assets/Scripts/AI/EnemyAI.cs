using UnityEngine;

/// <summary>BehaviourTree�œG�𐧌䂷��N���X</summary>
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
        // ����A�N�V����
        var patrolAction = new ActionNode(() =>
        {
            return _enemy.BTPatrol(_patrolPoint, _previousPos, _speed, _patrolRange);
        });

        // �����A�N�V����
        var fleeAction = new ActionNode(() =>
        {
            return _enemy.BTFlee(_player, _previousPos, _speed, _fleeRange);
        });

        // �ǐՃA�N�V����
        var chaseAction = new ActionNode(() =>
        {
            return _enemy.BTChase(_player, _previousPos, _speed, _chaseRange);
        });

        // �U���A�N�V����
        var attackAction = new ActionNode(() =>
        {
            return _enemy.BTAttack();
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

        // �U���V�[�P���X
        var attackSequence = new SequenceNode();
        attackSequence.AddChild(isPlayerClose);
        attackSequence.AddChild(hasEnoughHP);
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
}
