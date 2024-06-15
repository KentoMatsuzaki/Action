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
        // �v���C���[�ƐڐG�����ꍇ
        if (other.gameObject.tag == "Player")
        {
            // �A����4��̍U�����󂯂Ă���ꍇ
            if(_animator.GetInteger("HitCount") == 4)
            {
                _animator.Play("Get Down");
                SetIsDamagedTrue();
                
            }
            // �_���[�W���󂯂Ă���Œ��ł͂Ȃ��ꍇ
            else if(_animator.GetBool("IsDamaged") == false)
            {
                // �_���[�W�t���O���I���ɂ���
                SetIsDamagedTrue();

                // 0.1f��Ƀ_���[�W�t���O���I�t�ɂ���
                Invoke(nameof(SetIsDamagedFalse), 0.1f);

                // �q�b�g�J�E���g��1��������������
                _animator.SetInteger("HitCount", (_animator.GetInteger("HitCount") + 1));
            }
        }
    }

    /// <summary>�U���̊J�n����</summary>

    public void Attack()
    {
        // �U���\�ȏꍇ
        if(CanAttack())
        {
            // �U���g���K�[���I��
            _animator.Play("Attack");
        }
    }

    /// <summary>�U���\���ǂ�����Ԃ�</summary>
    /// <returns>�_���[�W���󂯂Ă���Ffalse �_���[�W���󂯂Ă��Ȃ��Ftrue</returns>
    public bool CanAttack()
    {
        return _animator.GetBool("IsDamaged") ? false : true;
    }

    /// <summary>�U���̏Ռ��C�x���g</summary>
    public void AttackImpactEvent()
    {
        _attackCol.enabled = true;
        Invoke(nameof(AttackEndEvent), 0.1f);
    }

    /// <summary>�U���̏I���C�x���g</summary>
    public void AttackEndEvent()
    {
        _attackCol.enabled = false;
    }

    /// <summary>�����Ŏw�肵�����Ԃ����ҋ@���ăA�N�V�������Ă�</summary>
    /// <param name="waitTime">�҂�����</param>
    /// <param name="action">�ҋ@��Ɏ��s����A�N�V����</param>
    IEnumerator Wait(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    /// <summary>�A�j���[�^�[�́uIsDamaged�v�t���O���I���ɂ���</summary>
    public void SetIsDamagedTrue()
    {
        _animator.SetBool("IsDamaged", true);
    }

    /// <summary>�A�j���[�^�[�́uIsDamaged�v�t���O���I�t�ɂ���</summary>
    public void SetIsDamagedFalse()
    {
        _animator.SetBool("IsDamaged", false);
    }

    /// <summary>�A�j���[�^�[�̃q�b�g�J�E���g��0�ŏ���������</summary>
    public void ResetHitCount()
    {
        _animator.SetInteger("HitCount", 0);
    }

    /// <summary>2f�ҋ@���Ă���N���オ��A�j���[�V�������Đ�����</summary>
    public void WaitForSecondsToRiseUp()
    {
        StartCoroutine(Wait(2f, () => _animator.SetTrigger("RiseUp")));
    }

    /// <summary>�v���C���[���߂��ɂ��邩�ǂ�����Ԃ�</summary>
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
