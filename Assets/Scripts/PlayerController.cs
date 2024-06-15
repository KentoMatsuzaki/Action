using UnityEngine;
using UnityEngine.InputSystem;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Effect;
using System.Collections;
using System;
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

    /// <summary>�E��̍U������p�R���C�_�[</summary>
    [SerializeField, Header("�E��̍U������p�R���C�_�[")] Collider _rightHandCol;

    /// <summary>����̍U������p�R���C�_�[</summary>
    [SerializeField, Header("����̍U������p�R���C�_�[")] Collider _leftHandCol;

    /// <summary>�E���̍U������p�R���C�_�[</summary>
    [SerializeField, Header("�E���̍U������p�R���C�_�[")] Collider _rightFootCol;

    /// <summary>�����̍U������p�R���C�_�[</summary>
    [SerializeField, Header("�����̍U������p�R���C�_�[")] Collider _leftFootCol;

    /// <summary>�ʏ펞�̑��x</summary>
    [SerializeField, Header("�ʏ펞�̈ړ����x")] float _normalSpeed = 1.2f;

    /// <summary>�X�v�����g���̑��x</summary>
    [SerializeField, Header("�X�v�����g���̈ړ����x")] float _sprintSpeed = 4.0f;

    /// <summary>�u�����N�̃N�[���^�C��</summary>
    [SerializeField, Header("�u�����N�̃N�[���^�C��")] float _dashCoolTime = 0.5f;

    /// <summary>�u�����N�̈ړ�����</summary>
    [SerializeField, Header("�u�����N�̈ړ�����")] float _dashDistance = 15f;

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
        //Debug.Log("Move is Pressed.");

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
        //Debug.Log("Sprint is Pressed.");

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
        //Debug.Log("Jump is Pressed.");

        // �W�����v�������ꂽ�ꍇ
        if (context.performed) _jumpControl.Jump(true);
    }

    /// <summary>JumpControl����Ă΂��R�[���o�b�N</summary>
    public void OnJumpStart()
    {
        // �Ăяo���̃��O���o�͂���
        //Debug.Log("Jump Start is Called");

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
        //Debug.Log("Dash is Pressed.");

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
        //Debug.Log("Attack is Pressed.");

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

    /// <summary>�����Ŏw�肵�����Ԃ����ҋ@���ăA�N�V�������Ă�</summary>
    /// <param name="waitTime">�҂�����</param>
    /// <param name="action">�ҋ@��Ɏ��s����A�N�V����</param>
    IEnumerator Wait(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    /// <summary>�E��̃R���C�_�[��L����</summary>
    public void EnableRightHandCol()
    {
        //Debug.Log("RightHandCol Enabled.");
        _rightHandCol.enabled = true;
        Invoke(nameof(DisableRightHandCol), 0.1f);
    }

    /// <summary>�E��̃R���C�_�[�𖳌���</summary>
    public void DisableRightHandCol()
    {
        //Debug.Log("RightHandCol Disabled.");
        _rightHandCol.enabled = false;
    }

    /// <summary>����̃R���C�_�[��L����</summary>
    public void EnableLeftHandCol()
    {
        //Debug.Log("LeftHandCol Enabled.");
        _leftHandCol.enabled = true;
        Invoke(nameof(DisableLeftHandCol), 0.1f);
    }

    /// <summary>����̃R���C�_�[�𖳌���</summary>

    public void DisableLeftHandCol()
    {
        //Debug.Log("LeftHandCol Disabled.");
        _leftHandCol.enabled= false;
    }

    /// <summary>�E���̃R���C�_�[��L����</summary>
    public void EnableRightFootCol()
    {
        //Debug.Log("RightFootCol Enabled.");
        _rightFootCol.enabled = true;
        Invoke(nameof(DisableRightFootCol), 0.1f);
    }

    /// <summary>�E���̃R���C�_�[�𖳌���</summary>
    public void DisableRightFootCol()
    {
        //Debug.Log("RightFootCol Disabled.");
        _rightFootCol.enabled = false;
    }

    /// <summary>�����̃R���C�_�[��L����</summary>
    public void EnableLeftFootCol()
    {
        //Debug.Log("LeftFootCol Enabled.");
        _leftFootCol.enabled = true;
        Invoke(nameof(DisableLeftFootCol), 0.1f);
    }

    /// <summary>�����̃R���C�_�[�𖳌���</summary>
    public void DisableLeftFootCol()
    {
        //Debug.Log("LeftFootCol Disabled.");
        _leftFootCol.enabled= false;
    }
}
