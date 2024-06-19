using UnityEngine;
using System;
using System.Collections;

/// <summary>�G�̐���</summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>�A�j���[�^�[</summary>
    Animator _animator;

    /// <summary>�U���p�R���C�_�[</summary>
    [SerializeField] Collider _attackCol;

    /// <summary>�v���C���[</summary>
    [SerializeField] Transform _player;

    /// <summary>�̗�</summary>
    [SerializeField] int _hp = 100;

    /// <summary>���G����</summary>
    [SerializeField] float _searchDistance;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        Attack();
    }

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[�ɍU�����ꂽ�ꍇ
        if (other.gameObject.tag == "PlayerAttack" && !_animator.GetBool("IsDamaged"))
        {
            // ��_���[�W����
            GetDamage(other);

            // ���S��ԂɑJ�ڂ��ď����𔲂���
            if (IsDied())
            {
                _animator.Play("Died");
                return;
            }
            // �_�E����ԂɑJ�ڂ���
            else if (IsReadyForDown())
            {
                _animator.Play("Get Down");
                // �_���[�W�t���O���I���ɂ���
                SetIsDamagedTrue();
            }
            // �����ԂɑJ�ڂ���
            else
            {
                // �_���[�W�t���O���I���ɂ���
                SetIsDamagedTrue();

                // �_���[�W�t���O���I�t�ɂ���
                Invoke(nameof(SetIsDamagedFalse), 0.1f);

                // �q�b�g�J�E���g��1��������������
                AddHitCountByOne();
            }
        }
    }

    

    /// <summary>��_���[�W����</summary>
    void GetDamage(Collider other)
    {
        // ��_���[�W��
        int damage = other.GetComponent<Attacker>().GetAttackDamage();
        
        // �̗͂��X�V����
        if(_hp > 0) DecreaseHP(damage);
    }

    /// <summary>�󂯂��_���[�W�������̗͂�����������</summary>
    /// <param name="damage">��_���[�W��</param>
    void DecreaseHP(int damage) => _hp -= damage;

    /// <summary>���S���Ă��邩</summary>
    bool IsDied() => _hp <= 0 ? true : false;

    /// <summary>���̍U�����󂯂ă_�E�����邩</summary>
    bool IsReadyForDown() => _animator.GetInteger("HitCount") == 4 ? true : false;

    /// <summary>�A�j���[�^�[�́uIsDamaged�v�t���O���I���ɂ���</summary>
    public void SetIsDamagedTrue() => _animator.SetBool("IsDamaged", true);

    /// <summary>�A�j���[�^�[�́uIsDamaged�v�t���O���I�t�ɂ���</summary>
    public void SetIsDamagedFalse() => _animator.SetBool("IsDamaged", false);

    /// <summary>�q�b�g�J�E���g��1����������</summary>
    void AddHitCountByOne() => _animator.SetInteger("HitCount", (_animator.GetInteger("HitCount") + 1));

    /// <summary>�q�b�g�J�E���g��0�ɐݒ肷��</summary>
    public void ResetHitCount() => _animator.SetInteger("HitCount", 0);

    /// <summary>�U����ԂɑJ�ڂ���</summary>
    public void Attack()
    {
        if(CanAttack()) _animator.Play("Attack");
    }

    /// <summary>�U���\���ǂ���</summary>
    /// <returns>�U�����󂯂Ă��Ȃ��Ftrue / �U�����󂯂Ă���Ffalse</returns>
    public bool CanAttack() => _animator.GetBool("IsDamaged") ? false : true;

    /// <summary>�U���̖����C�x���g</summary>
    public void AttackImpactEvent()
    {
        _attackCol.enabled = true;
        Invoke(nameof(AttackEndEvent), 0.1f);
    }

    /// <summary>�U���̏I���C�x���g</summary>
    public void AttackEndEvent() => _attackCol.enabled = false;

    /// <summary>�ҋ@���Ă���A�N�V���������s����</summary>
    /// <param name="waitTime">�ҋ@����</param>
    /// <param name="action">�A�N�V����</param>
    IEnumerator WaitThenCallAction(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    /// <summary>�N���オ���ԂɑJ�ڂ���</summary>
    public void WaitForSecondsToRiseUp()
    {
        StartCoroutine(WaitThenCallAction(2f, () => _animator.SetTrigger("RiseUp")));
    }

    /// <summary>�v���C���[���߂��ɂ��邩�ǂ���</summary>
    public bool IsPlayerClose()
    {
        // �v���C���[�Ƃ̋���
        float distance = Vector3.Distance(transform.position, _player.position);

        // ���G�����ƃv���C���[�Ƃ̋������r����
        return _searchDistance >= distance ? true : false;
    }

    public void MoveTowardPlayer()
    {
        Vector3.MoveTowards(transform.position, _player.position, 2f * Time.deltaTime);
    }
}
