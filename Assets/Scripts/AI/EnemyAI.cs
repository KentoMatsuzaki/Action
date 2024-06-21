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
    [SerializeField] private float _range;

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

    //private BaseNode ConstructBehaviorTree()
    //{
    //    // �A�N�V�����̒�`
    //    var patrolAction = new ActionNode(() =>
    //    {
    //        return _enemy.BTPatrol(_patrolPoint, _previousPos, _speed, _range);
    //    });

    //    var fleeAction = new ActionNode(() =>
    //    {
    //        return _enemy.BTFlee(_player, _previousPos, _speed);
    //    });

    //    var chaseAction = new ActionNode(() =>
    //    {
    //        return _enemy.BTChase(_player, _previousPos, _speed);
    //    });

    //    var attackAction = new ActionNode(() =>
    //    {
    //        return _enemy.BTAttack();
    //    });

    //    // �����̒�`
    //    var isPlayerClose = new ConditionNode(() => _enemy.IsPlayerClose());
    //    var isPlayerAway = new ConditionNode(() => _enemy.IsPlayerInRange(player, attackRange));

    //    // �V�[�P���X��Z���N�^�[�̒�`
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
