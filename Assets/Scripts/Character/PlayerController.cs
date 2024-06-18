using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Effect;
using UnityEngine.InputSystem.Interactions;

/// <summary>�v���C���[����</summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>�A�j���[�^�[</summary>
    Animator _animator;

    /// <summary>�ړ�����</summary>
    MoveControl _moveControl;

    /// <summary>�W�����v����</summary>
    JumpControl _jumpControl;

    /// <summary>��������</summary>
    ExtraForce _extraForce;

    /// <summary>�ڒn����</summary>
    GroundCheck _groundCheck;

    /// <summary>�U������</summary>
    [SerializeField, Header("�U������")] List<Collider> _cols = new List<Collider>();

    /// <summary>�ʏ펞�̑��x</summary>
    [SerializeField, Header("�ʏ펞�̈ړ����x")] float _normalSpeed = 1.2f;

    /// <summary>�X�v�����g���̑��x</summary>
    [SerializeField, Header("�X�v�����g���̈ړ����x")] float _sprintSpeed = 4.0f;

    /// <summary>�u�����N�̃N�[���^�C��</summary>
    [SerializeField, Header("�u�����N�̃N�[���^�C��")] float _dashCoolTime = 0.5f;

    /// <summary>�u�����N�̈ړ�����</summary>
    [SerializeField, Header("�u�����N�̈ړ�����")] float _dashDistance = 15f;

    /// <summary>�_���[�W�R���C�_�[</summary>
    Collider _passiveCol;

    /// <summary>�U���R���C�_�[�̎��ʎq</summary>
    int _currentColIndex;

    /// <summary>�G�t�F�N�g</summary>
    EffectManager _effect;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _moveControl = GetComponent<MoveControl>();
        _groundCheck = GetComponent<GroundCheck>();
        _jumpControl = GetComponent<JumpControl>();
        _extraForce = GetComponent<ExtraForce>();
        _passiveCol = GetComponent<Collider>();
        _effect = EffectManager.Instance;
    }
    private void Update()
    {
        // �X�s�[�h���X�V����
        SetSpeed();
        
        // �ڒn������X�V����
        SetIsOnGround();
    }

    /// <summary>�ړ��̕������Z���s��</summary>
    /// <summary>PlayerInput����Ă΂��</summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        // Move�A�N�V���������͂��ꂽ�ꍇ
        if (context.performed) _moveControl.Move(context.ReadValue<Vector2>()); 

        // Move�A�N�V�����������[�X���ꂽ�ꍇ
        else if (context.canceled) _moveControl.Move(Vector2.zero);
    }

    /// <summary>�ړ����x�̍X�V���s���i�A�j���[�V�����͑��x�ɉ����Ď����ŕύX�����j</summary>
    /// <summary>PlayerInput����Ă΂��</summary>
    public void OnSprint(InputAction.CallbackContext context)
    {
        // �X�v�����g�����͂��ꂽ�ꍇ
        if (context.performed) _moveControl.MoveSpeed = _sprintSpeed;

        // �X�v�����g�������[�X���ꂽ�ꍇ
        else if (context.canceled) _moveControl.MoveSpeed = _normalSpeed;
    }

    /// <summary>�W�����v�̕������Z���s��</summary>
    /// <summary>PlayerInput����Ă΂��</summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        // �W�����v�����͂��ꂽ�ꍇ
        if (context.performed) _jumpControl.Jump(true);
    }

    /// <summary>�W�����v�̃A�j���[�V�����Đ����s��</summary>
    /// <summary>JumpControl����Ă΂��</summary>
    public void OnJumpStart()
    {
        // ���݂̃W�����v�񐔂ƍő�W�����v�񐔂��r���ăW�����v��؂�ւ���
        _animator.Play
            (_jumpControl.AerialJumpCount >= _jumpControl.MaxAerialJumpCount ? 
                "Double Jump" : "Jump Up");
    }

    /// <summary>�_�b�V���̕������Z�ƃA�j���[�V�����Đ����s��</summary>
    /// <summary>PlayerInput����Ă΂��</summary>
    public void OnDash(InputAction.CallbackContext context)
    {
        // �_�b�V�������͂��ꂽ�ꍇ
        if (context.performed && _animator.GetBool("CanDash"))
        {
            _animator.SetBool("CanDash", false);
            _extraForce.AddForce(transform.forward * _dashDistance);
            _animator.Play("Dash Start");
            StartCoroutine(WaitThenCallAction(_dashCoolTime,
                () => _animator.SetBool("CanDash", true)));
        }
    }

    /// <summary>�U���̃A�j���[�V�����J�ڂ��s��</summary>
    /// <summary>PlayerInput����Ă΂��</summary>
    public void OnAttack(InputAction.CallbackContext context)
    {
        // �U���A�N�V���������������ꂽ�ꍇ
        if(context.interaction is HoldInteraction && context.performed)
        {
            _animator.SetTrigger("Long Attack");
        }
        // �U���A�N�V�������Z�������ꂽ�ꍇ
        else if(context.interaction is PressInteraction && context.performed)
        {
            _animator.SetTrigger("Short Attack");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �G�ɍU�����ꂽ�ꍇ
        if (other.gameObject.tag == "EnemyAttack")
        {
            SetIsDamagedTrue();
            Invoke(nameof(SetIsDamagedFalse), 0.1f);

            // �Փˈʒu���v�Z
            var hitPos = GetComponent<Collider>().
                ClosestPointOnBounds(other.transform.position);

            // �_���[�W�G�t�F�N�g���Đ�
            PlayDamageEffect(hitPos, 0);
        }
    }

    /// <summary>�ړ����x��ݒ肷��</summary>
    private void SetSpeed() => _animator.SetFloat("Speed", _moveControl.CurrentSpeed);

    /// <summary>�ڒn������擾����</summary>
    private bool IsOnGround() => _groundCheck.IsOnGround ? true : false;

    /// <summary>�ڒn�����ݒ肷��</summary>
    private void SetIsOnGround() => 
        _animator.SetBool("IsOnGround", IsOnGround() ? true : false);

    /// <summary>�_���[�W�t���O���擾����</summary>
    private bool IsDamaged() => _animator.GetBool("IsDamaged") ? true : false;

    /// <summary>�_���[�W�t���O��ݒ肷��</summary>
    private void SetIsDamaged() => 
        _animator.SetBool("IsDamaged", IsDamaged() ? false : true);

    /// <summary>�U����������u�ԂɌĂ΂��C�x���g</summary>
    /// <param name="currentColIndex">�U���R���C�_�[�̎��ʎq</param>
    /// <param name="attackEffectIndex">�U���G�t�F�N�g�̎��ʎq</param>
    public void AttackImpactEvent(int colliderIndex, int effectIndex)
    {
        EnableCol(colliderIndex);
        SetCurrentColIndex(colliderIndex);
        Invoke(nameof(DisableCol), 0.1f);
        PlayAttackEffect(effectIndex);
    }

    /// <summary>���ʎq�Ŏw�肵���U���R���C�_�[��L��������</summary>
    private void EnableCol(int index) => _cols[index].enabled = true;

    /// <summary>���ʎq�Ŏw�肵���U���R���C�_�[�𖳌�������</summary>
    private void DisableCol() => _cols[_currentColIndex].enabled = false;

    /// <summary>�U���R���C�_�[�̎��ʎq��ݒ肷��</summary>
    private void SetCurrentColIndex(int index) => _currentColIndex = index;

    /// <summary>�U�������ʒu�ɍU���G�t�F�N�g��\������</summary>
    private void PlayAttackEffect(int index) =>
        _effect.PlayAttackEffect(GetAttackPosition(), index);

    /// <summary>�U�����ꂽ�ʒu�Ƀ_���[�W�G�t�F�N�g��\������</summary>
    private void PlayDamageEffect(int index, Collider other) =>
        _effect.PlayDamageEffect(GetImpactPosition(other), index);

    /// <summary>�����̍U���R���C�_�[�̈ʒu��Ԃ�</summary>
    private Vector3 GetAttackPosition() => _cols[_currentColIndex].transform.position;

    /// <summary>����̍U���R���C�_�[�̈ʒu��Ԃ�</summary>
    private Vector3 GetDamagePosition(Collider other) => other.transform.position;

    /// <summary>����̍U���R���C�_�[����ł��߂��R���C�_�[��̈ʒu��Ԃ�</summary>
    private Vector3 GetImpactPosition(Collider other) 
        => _passiveCol.ClosestPoint(GetDamagePosition(other));
}
