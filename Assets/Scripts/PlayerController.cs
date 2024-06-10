using UnityEngine;
using UnityEngine.InputSystem;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Effect;
using System.Collections;
using System;
using UnityEngine.InputSystem.Interactions;

/// <summary>�v���C���[����N���X</summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>�v���C���[�̃A�j���[�^�[</summary>
    Animator _animator;

    /// <summary>�v���C���[�̈ړ�����</summary>
    MoveControl _moveControl;

    /// <summary>�v���C���[�̃W�����v����</summary>
    JumpControl _jumpControl;

    /// <summary>�v���C���[�̕�������</summary>
    ExtraForce _extraForce;

    /// <summary>�v���C���[�̐ڒn����</summary>
    GroundCheck _groundCheck;

    /// <summary>�ʏ펞�̑��x</summary>
    [SerializeField] float _normalSpeed = 1.2f;

    /// <summary>�X�v�����g���̑��x</summary>
    [SerializeField] float _sprintSpeed = 4.0f;

    /// <summary>�u�����N�̃N�[���^�C��</summary>
    [SerializeField] float _dashCoolTime = 0.5f;

    /// <summary>�u�����N�̈ړ�����</summary>
    [SerializeField] float _dashDistance = 15f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _moveControl = GetComponent<MoveControl>();
        _groundCheck = GetComponent<GroundCheck>();
        _jumpControl = GetComponent<JumpControl>();
        _extraForce = GetComponent<ExtraForce>();
    }
    private void Update()
    {
        // �X�s�[�h���X�V
        SetSpeed();
        
        // �ڒn������X�V
        SetIsOnGround();
    }

    /// <summary>PlayerInput����Ă΂��C�x���g</summary>
    /// <param name="context">�{�^�����͎��ɌĂяo�����R�[���o�b�N</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // ���͂̃��O���o�͂���
        Debug.Log("Move is Pressed.");

        // Move�A�N�V�����������ꂽ�ꍇ
        if (context.performed) _moveControl.Move(context.ReadValue<Vector2>()); 

        // Move�A�N�V�����������[�X���ꂽ�ꍇ
        else if (context.canceled) _moveControl.Move(Vector2.zero);
    }

    /// <summary>PlayerInput����Ă΂��C�x���g</summary>
    /// <param name="context">�{�^�����͎��ɌĂяo�����R�[���o�b�N</param>
    public void OnSprint(InputAction.CallbackContext context)
    {
        // ���͂̃��O���o�͂���
        Debug.Log("Sprint is Pressed.");

        // �X�v�����g�������ꂽ�ꍇ
        if (context.performed) _moveControl.MoveSpeed = _sprintSpeed;

        // �X�v�����g�������[�X���ꂽ�ꍇ
        else if (context.canceled) _moveControl.MoveSpeed = _normalSpeed;
    }

    /// <summary>PlayerInput����Ă΂��C�x���g</summary>
    /// <param name="context">�{�^�����͎��ɌĂяo�����R�[���o�b�N</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        // ���͂̃��O���o�͂���
        Debug.Log("Jump is Pressed.");

        // �W�����v�������ꂽ�ꍇ
        if (context.performed) _jumpControl.Jump(true);
    }

    /// <summary>JumpControl����Ă΂��R�[���o�b�N</summary>
    public void OnJumpStart()
    {
        // �Ăяo���̃��O���o�͂���
        Debug.Log("Jump Start is Called");

        // ���݂̃W�����v�񐔂ƍő�W�����v�񐔂��r���ăW�����v��؂�ւ���
        _animator.Play
            (_jumpControl.AerialJumpCount >= _jumpControl.MaxAerialJumpCount ? 
                "Double Jump" : "Jump Up");
    }

    /// <summary>PlayerInput����Ă΂��C�x���g</summary>
    /// <param name="context">�{�^�����͎��ɌĂяo�����R�[���o�b�N</param>
    public void OnDash(InputAction.CallbackContext context)
    {
        // ���͂̃��O���o�͂���
        Debug.Log("Dash is Pressed.");

        // �_�b�V���\���ł���ꍇ
        if (context.performed && _animator.GetBool("CanDash"))
        {
            _animator.SetBool("CanDash", false);
            _extraForce.AddForce(transform.forward * _dashDistance);
            _animator.Play("Dash Start");
            StartCoroutine(Wait(_dashCoolTime,
                () => _animator.SetBool("CanDash", true)));
        }
    }


    /// <summary>PlayerInput����Ă΂��C�x���g</summary>
    /// <param name="context">�{�^�����͎��ɌĂяo�����R�[���o�b�N</param>
    public void OnAttack(InputAction.CallbackContext context)
    {
        // ���͂̃��O���o�͂���
        Debug.Log("Attack is Pressed");

        // �U���A�N�V���������������ꂽ�ꍇ
        if(context.interaction is HoldInteraction && context.performed)
        {
            _animator.SetTrigger("LongAtk");
        }

        // �U���A�N�V�������Z�������ꂽ�ꍇ
        if(context.interaction is PressInteraction && context.performed)
        {
            _animator.SetTrigger("ShortAtk");
        }

    }

    /// <summary>�A�j���[�^�[�́uSpeed�v�p�����[�^�[���X�V����</summary>
    void SetSpeed()
    {
        _animator.SetFloat("Speed", _moveControl.CurrentSpeed);
    }

    /// <summary>�A�j���[�^�[�́uIsGround�v�p�����[�^�[���X�V����</summary>
    void SetIsOnGround()
    {
        _animator.SetBool("IsOnGround", _groundCheck.IsOnGround ? true : false);
    }

    /// <summary>�N�[���^�C���p�̃R���[�`��</summary>
    /// <param name="time">�҂�����</param>
    /// <param name="action">�ҋ@��Ɏ��s���鏈��</param>
    IEnumerator Wait(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
}
