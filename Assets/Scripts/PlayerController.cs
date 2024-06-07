using UnityEngine;
using UnityEngine.InputSystem;
using Unity.TinyCharacterController.Control;

/// <summary>�v���C���[����N���X</summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>�v���C���[�̃A�j���[�^�[</summary>
    Animator _animator;

    /// <summary>�v���C���[�̈ړ�����</summary>
    MoveControl _moveControl;

    /// <summary>�ʏ펞�̑��x</summary>
    public float _normalSpeed = 1.2f;

    /// <summary>�X�v�����g���̑��x</summary>
    public float _sprintSpeed = 7.5f;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _moveControl = GetComponent<MoveControl>();
    }
    private void Update()
    {
        // �L�����N�^�[�̃X�s�[�h���X�V
        _animator.SetFloat("Speed", _moveControl.CurrentSpeed);
    }

    /// <summary>PlayerInput����Ă΂��C�x���g</summary>
    /// <param name="context">���͎��ɌĂяo�����R�[���o�b�N�֐��̏��</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // Move�A�N�V�����������ꂽ�ꍇ
        if (context.performed)
        {
            _moveControl.Move(context.ReadValue<Vector2>());
        }
        // Move�A�N�V�����������[�X���ꂽ�ꍇ
        else if (context.canceled)
        {
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
            _moveControl.MoveSpeed = _sprintSpeed;
            Debug.Log("Sprint is Pressed.");
        }
        // �X�v�����g�A�N�V�����������[�X���ꂽ�ꍇ
        else if (context.canceled)
        {
            _moveControl.MoveSpeed = _normalSpeed;
            Debug.Log("Sprint is Released.");
        }
    }
}
