using UnityEngine;
using UnityEngine.InputSystem;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Check;

/// <summary>�v���C���[����N���X</summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>�v���C���[�̃A�j���[�^�[</summary>
    Animator _animator;

    /// <summary>�v���C���[�̈ړ�����</summary>
    MoveControl _moveControl;

    /// <summary>�v���C���[�̃W�����v����</summary>
    JumpControl _jumpControl;

    /// <summary>�v���C���[�̐ڒn����</summary>
    GroundCheck _groundCheck;

    /// <summary>�ʏ펞�̑��x</summary>
    public float _normalSpeed = 1.2f;

    /// <summary>�X�v�����g���̑��x</summary>
    public float _sprintSpeed = 7.5f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _moveControl = GetComponent<MoveControl>();
        _groundCheck = GetComponent<GroundCheck>();
        _jumpControl = GetComponent<JumpControl>();
    }
    private void Update()
    {
        // �X�s�[�h���X�V
        SetSpeed();
        
        // �ڒn������X�V
        SetIsOnGround();
    }

    /// <summary>PlayerInput����Ă΂��C�x���g</summary>
    /// <param name="context">���͎��ɌĂяo�����R�[���o�b�N�֐��̏��</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // Move�A�N�V�����������ꂽ�ꍇ
        if (context.performed)
        {
            Debug.Log("Move is Pressed.");
            _moveControl.Move(context.ReadValue<Vector2>()); 
        }
        // Move�A�N�V�����������[�X���ꂽ�ꍇ
        else if (context.canceled)
        {
            Debug.Log("Move is Released.");
            _moveControl.Move(Vector2.zero);
        }
    }

    /// <summary>PlayerInput����Ă΂��C�x���g</summary>
    /// <param name="context">���͎��ɌĂяo�����R�[���o�b�N�֐��̏��</param>
    public void OnSprint(InputAction.CallbackContext context)
    {
        // �X�v�����g�A�N�V�����������ꂽ�ꍇ
        if (context.performed)
        {
            Debug.Log("Sprint is Pressed.");
            _moveControl.MoveSpeed = _sprintSpeed;
        }
        // �X�v�����g�A�N�V�����������[�X���ꂽ�ꍇ
        else if (context.canceled)
        {
            Debug.Log("Sprint is Released.");
            _moveControl.MoveSpeed = _normalSpeed;
        }
    }

    /// <summary>PlayerInput����Ă΂��C�x���g</summary>
    /// <param name="context">���͎��ɌĂяo�����R�[���o�b�N�֐��̏��</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        // �W�����v�A�N�V�����������ꂽ�ꍇ
        if (context.performed)
        {
            Debug.Log("Jump is Pressed.");
            _animator.Play("Jump");
        }
    }

    /// <summary>Jump�A�j���[�V�����̃A�j���[�V�����C�x���g����Ă΂����ۂ̃W�����v����</summary>
    public void JumpUp()
    {
        _jumpControl.Jump(true);
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
}
