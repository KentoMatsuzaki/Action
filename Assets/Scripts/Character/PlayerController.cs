using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
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

    /// <summary>�A�j���[�^�[�́uIsDamaged�v�t���O���I���ɂ���</summary>
    void SetIsDamagedTrue()
    {
        _animator.SetBool("IsDamaged", true);
    }

    /// <summary>�A�j���[�^�[�́uIsDamaged�v�t���O���I�t�ɂ���</summary>
    void SetIsDamagedFalse()
    {
        _animator.SetBool("IsDamaged", false);
    }

    /// <summary>�����Ŏw�肵�����Ԃ����ҋ@���ăA�N�V�������Ă�</summary>
    /// <param name="waitTime">�҂�����</param>
    /// <param name="action">�ҋ@��Ɏ��s����A�N�V����</param>
    IEnumerator WaitThenCallAction(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    /// <summary>�E��̃R���C�_�[��L�������A�U���G�t�F�N�g��\������</summary>
    /// <summary>�A�j���[�V�����C�x���g����Ă΂��</summary>
    public void EnableRightHandCol()
    {
        //Debug.Log("RightHandCol Enabled.");
        _rightHandCol.enabled = true;
        Invoke(nameof(DisableRightHandCol), 0.1f);
        PlayAttackEffect(_rightHandCol.transform.position, 0);
    }

    /// <summary>�E��̃R���C�_�[�𖳌���</summary>
    /// <summary>�A�j���[�V�����C�x���g����Ă΂��</summary>
    public void DisableRightHandCol()
    {
        //Debug.Log("RightHandCol Disabled.");
        _rightHandCol.enabled = false;
    }

    /// <summary>����̃R���C�_�[��L�������A�U���G�t�F�N�g��\������</summary>
    /// <summary>�A�j���[�V�����C�x���g����Ă΂��</summary>
    public void EnableLeftHandCol()
    {
        //Debug.Log("LeftHandCol Enabled.");
        _leftHandCol.enabled = true;
        Invoke(nameof(DisableLeftHandCol), 0.1f);
        PlayAttackEffect(_leftHandCol.transform.position, 0);
    }

    /// <summary>����̃R���C�_�[�𖳌���</summary>
    /// <summary>�A�j���[�V�����C�x���g����Ă΂��</summary>

    public void DisableLeftHandCol()
    {
        //Debug.Log("LeftHandCol Disabled.");
        _leftHandCol.enabled= false;
    }

    /// <summary>�E���̃R���C�_�[��L�������A�U���G�t�F�N�g��\������</summary>
    /// <summary>�A�j���[�V�����C�x���g����Ă΂��</summary>
    public void EnableRightFootCol()
    {
        //Debug.Log("RightFootCol Enabled.");
        _rightFootCol.enabled = true;
        Invoke(nameof(DisableRightFootCol), 0.1f);
        PlayAttackEffect(_rightFootCol.transform.position, 0);
    }

    /// <summary>�E���̃R���C�_�[�𖳌���</summary>
    /// <summary>�A�j���[�V�����C�x���g����Ă΂��</summary>
    public void DisableRightFootCol()
    {
        //Debug.Log("RightFootCol Disabled.");
        _rightFootCol.enabled = false;
    }

    /// <summary>�����̃R���C�_�[��L�������A�U���G�t�F�N�g��\������</summary>
    /// <summary>�A�j���[�V�����C�x���g����Ă΂��</summary>
    public void EnableLeftFootCol()
    {
        //Debug.Log("LeftFootCol Enabled.");
        _leftFootCol.enabled = true;
        Invoke(nameof(DisableLeftFootCol), 0.1f);
        PlayAttackEffect(_leftFootCol.transform.position, 0);
    }

    /// <summary>�����̃R���C�_�[�𖳌���</summary>
    /// <summary>�A�j���[�V�����C�x���g����Ă΂��</summary>
    public void DisableLeftFootCol()
    {
        //Debug.Log("LeftFootCol Disabled.");
        _leftFootCol.enabled= false;
    }

    /// <summary>���W���w�肵�čU���G�t�F�N�g��\������</summary>
    /// <param name="pos">�G�t�F�N�g��\����������W</param>
    /// <param name="index">�G�t�F�N�g�̃C���f�b�N�X</param>
    private void PlayAttackEffect(Vector3 pos, int index)
    {
        EffectController.Instance.PlayAttackEffect(pos, index);
    }

    /// <summary>���W���w�肵�ă_���[�W�G�t�F�N�g��\������</summary>
    /// <param name="pos">�G�t�F�N�g��\����������W</param>
    /// <param name="index">�G�t�F�N�g�̃C���f�b�N�X</param>
    private void PlayDamageEffect(Vector3 pos, int index)
    {
        EffectController.Instance.PlayDamageEffect(pos, index);
    }

    /// <summary>���W���w�肵�Ď��S�G�t�F�N�g��\������</summary>
    /// <param name="pos">�G�t�F�N�g��\����������W</param>
    /// <param name="index">�G�t�F�N�g�̃C���f�b�N�X</param>
    private void PlayDeadEffect(Vector3 pos, int index)
    {
        EffectController.Instance.PlayDeadEffect(pos, index);
    }
}
