using UnityEngine;
using Unity.TinyCharacterController.Control;

/// <summary>�G�̐���</summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>�A�j���[�^�[</summary>
    Animator _animator;

    /// <summary>�U���R���C�_�[</summary>
    [SerializeField, Header("�U���R���C�_�[")] Collider _attackCol;

    /// <summary>�v���C���[</summary>
    [SerializeField, Header("�v���C���[�̈ʒu")] Transform _player;

    /// <summary>�̗�</summary>
    [SerializeField, Header("�G��HP")] int _hp = 100;

    /// <summary>���G����</summary>
    [SerializeField, Header("���G����")] float _searchDistance;

    /// <summary>�T�E���h</summary>
    CriSoundManager _soundManager;

    /// <summary>�ő�HP</summary>
    int _maxHP;

    private Vector3 previousPosition;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _soundManager = CriSoundManager.Instance;
        _maxHP = _hp;
        Attack();
    }

    private void Update()
    {
        Chase(_player, previousPosition, 0.5f);
    }

    //-------------------------------------------------------------------------------
    // ��_���[�W���̃R�[���o�b�N�C�x���g
    //-------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[�ɍU�����󂯂��ꍇ && �_���[�W���󂯂Ă��Ȃ��ꍇ
        if (other.gameObject.tag == "PlayerAttack" && ! IsDamaged())
        {
            // �����Ă���ꍇ
            if (IsAlive())
            {
                // �_���[�W����
                TakeDamage(other);
            }

            // ����ł���ꍇ
            if (IsDead())
            {
                // ���S�A�j���[�V�������Đ�
                PlayDeadAnimation();

                // SE���Đ�
                PlayDeadSound();
            }

            // �_�E������ꍇ
            else if (IsDown())
            {
                // �_�E���A�j���[�V�������Đ�
                PlayDownAnimation();

                // �_���[�W�t���O���I���ɂ���
                SetDamaged(true);

                // SE���Đ�
                PlayDamageSound();
            }

            // �ʏ펞
            else
            {
                // �_���[�W�t���O���I���ɂ���
                SetDamaged(true);

                // �_���[�W�t���O���I�t�ɂ���
                Invoke(nameof(UnsetDamaged), 0.1f);

                // �q�b�g�J�E���g��1��������������
                IncrementHitCount();

                // SE���Đ�
                PlayDamageSound();
            }
        }
    }

    //-------------------------------------------------------------------------------
    // ��_���[�W���Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�U�����󂯂Ă���Œ���</summary>
    private bool IsDamaged() => _animator.GetBool("IsDamaged");

    /// <summary>�����Ă��邩</summary>
    private bool IsAlive() => _hp > 0;

    /// <summary>����ł��邩</summary>
    private bool IsDead() => _hp <= 0;

    /// <summary>�_�E�����邩</summary>
    bool IsDown() => _animator.GetInteger("HitCount") == 4 ? true : false;

    /// <summary>�_���[�W�ʂ��Z�o</summary>
    private int CalDamage(Collider other) => other.GetComponent<Attacker>().Power;

    /// <summary>�̗͂�ݒ�</summary>
    private void SetHP(int damage) => _hp -= damage;

    /// <summary>�_���[�W����</summary>
    private void TakeDamage(Collider other) => SetHP(CalDamage(other));

    /// <summary>���S�A�j���[�V�������Đ�</summary>
    private void PlayDeadAnimation() => _animator.Play("Died");

    /// <summary>�_�E���A�j���[�V�������Đ�</summary>
    private void PlayDownAnimation() => _animator.Play("Get Down");

    /// <summary>�_���[�W�t���O��ݒ�</summary>
    private void SetDamaged(bool value) => _animator.SetBool("IsDamaged", value);

    /// <summary>�_���[�W�t���O������</summary>
    private void UnsetDamaged() => SetDamaged(false);

    /// <summary>�q�b�g�J�E���g��1����������</summary>
    private void IncrementHitCount() => 
        _animator.SetInteger("HitCount", (_animator.GetInteger("HitCount") + 1));

    /// <summary>�q�b�g�J�E���g��0�ɐݒ肷��</summary>
    public void ResetHitCount() => _animator.SetInteger("HitCount", 0);

    /// <summary>�����҂��Ă���N���オ��g���K�[���I���ɂ���</summary>
    public void WaitSetRiseTrigger()
        => Invoke(nameof(SetRiseTrigger), 2.0f);

    /// <summary>�N���オ��g���K�[���I���ɂ���</summary>
    private void SetRiseTrigger() => _animator.SetTrigger("Rise");

    //-------------------------------------------------------------------------------
    // �U���̃C�x���g
    //-------------------------------------------------------------------------------

    /// <summary>�U���C�x���g</summary>
    public void Attack()
    {
        if (CanAttack())
        {
            // �A�j���[�V�������Đ�
            PlayAttackAnimation();

            // SE���Đ�
            Invoke(nameof(PlayAttackSound), 0.1f);
        }
    }

    /// <summary>�U���̖����C�x���g</summary>
    public void AttackImpactEvent()
    {
        // �U���R���C�_�[��L����
        EnableAttackCol();

        // �U���R���C�_�[�𖳌���
        Invoke(nameof(DisableAttackCol), 0.1f);
    }

    //-------------------------------------------------------------------------------
    // �U���Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�U���\���ǂ���</summary>
    private bool CanAttack() => _animator.GetBool("IsDamaged") ? false : true;

    private void PlayAttackAnimation() => _animator.Play("Attack");

    /// <summary>�U���R���C�_�[��L����</summary>
    private void EnableAttackCol() => _attackCol.enabled = true;

    /// <summary>�U���R���C�_�[�𖳌���</summary>
    private void DisableAttackCol() => _attackCol.enabled = false;

    //-------------------------------------------------------------------------------
    // SE�Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�L���[�����擾</summary>
    private string GetCueName(int index) => _soundManager._enemyCueNames[index];

    /// <summary>SE���Đ�</summary>
    private void PlaySE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetCueName(index), volume);

    /// <summary>�U��SE���Đ�</summary>
    private void PlayAttackSound() => PlaySE(0, 0.25f);

    /// <summary>�_���[�WSE���Đ�</summary>
    private void PlayDamageSound() => PlaySE(1, 1f);

    /// <summary>���SSE���Đ�</summary>
    private void PlayDeadSound() => PlaySE(2, 0.5f);

    //-------------------------------------------------------------------------------
    // BehaviourTree�̃m�[�h
    //-------------------------------------------------------------------------------

    /// <summary>�U���A�N�V����</summary>
    public NodeStatus BTAttack()
    {
        // ������Attack���\�b�h���Ăяo��
        Attack();

        // ��x�̍U�������������琬����Ԃ�
        return NodeStatus.Success; 
    }

    /// <summary>�ǐՃA�N�V����</summary>
    public NodeStatus Chase(Transform player, Vector3 previous, float speed)
    {
        if (GetDistanceToPlayer() >= 1.5f)
        {
            // �v���C���[�̕�������
            RotateTowardsPlayer(player);

            // �O���Ɉړ�����
            MoveForward(speed);

            // �ړ��ʂ𑬓x�ɕϊ����Đݒ肷��
            SetSpeedParameter(GetSpeed(GetPositionDiff(previous)));

            // ���W���X�V����
            previous = transform.position;
        }
        else
        {
            // �ړ��ʂ𑬓x�ɕϊ����Đݒ肷��
            SetSpeedParameter(0);
        }

        if (GetDistanceToPlayer() < 1.5f)
        {
            return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }

    /// <summary>�v���C���[���߂��ɂ��邩</summary>
    public bool IsPlayerClose() => GetDistanceToPlayer() <= _searchDistance;

    /// <summary>�\����HP�����邩</summary>
    public bool HasEnoughHP() => _hp >= _maxHP / 2;

    /// <summary>HP���s�����Ă��邩</summary>
    public bool DoesNotHaveEnoughHP() => _hp < _maxHP / 2;

    //-------------------------------------------------------------------------------
    // BehaviourTree�Ɋւ��鏈��
    //-------------------------------------------------------------------------------



    /// <summary>�v���C���[�Ƃ̋������Z�o����</summary>
    private float GetDistanceToPlayer() =>
        Vector3.Distance(transform.position, _player.position);

    /// <summary>�v���C���[�ւ̕������Z�o����</summary>
    private Vector3 GetDirectionToPlayer(Transform player)
        => player.position - transform.position;
 
    /// <summary>�v���C���[�ւ̍œK�����ꂽ�������Z�o����</summary>
    private Vector3 GetOptimizedDirectionToPlayer(Transform player) 
        => new Vector3(GetDirectionToPlayer(player).x, 0f, GetDirectionToPlayer(player).z).normalized;

    /// <summary>�v���C���[�ւ̉�]�������擾����</summary>
    private Quaternion GetRotationToPlayer(Vector3 dir) => Quaternion.LookRotation(dir);

    /// <summary>�v���C���[�̕�������</summary>
    private void RotateTowardsPlayer(Transform player) => 
        transform.rotation = Quaternion.Slerp(transform.rotation, 
            GetRotationToPlayer(GetOptimizedDirectionToPlayer(player)), Time.deltaTime * 10f);

    /// <summary>�O���Ɉړ�����</summary>
    private void MoveForward(float moveSpeed) => 
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

    /// <summary>���W�̍��������߂�</summary>
    private Vector3 GetPositionDiff(Vector3 previous) => transform.position - previous;

    /// <summary>�ړ����x�����߂�</summary>
    private float GetSpeed(Vector3 diff) => diff.magnitude / Time.deltaTime;

    /// <summary>�ړ����x��ݒ肷��</summary>
    private void SetSpeedParameter(float speed) => _animator.SetFloat("Speed", speed);
}
