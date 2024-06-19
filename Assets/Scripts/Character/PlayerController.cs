using UnityEngine;
using UnityEngine.InputSystem;
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

    /// <summary>SE�̎��ʎq</summary>
    [SerializeField, Header("SE�̃C���f�b�N�X")] int _soundIndex;

    /// <summary>SE�̎��ʎq</summary>
    [SerializeField, Header("SE�̃{�����[��")] float _volume;

    /// <summary>�_���[�W�R���C�_�[</summary>
    Collider _passiveCol;

    /// <summary>�U���R���C�_�[�̎��ʎq</summary>
    int _currentColIndex;

    /// <summary>�G�t�F�N�g</summary>
    EffectManager _effect;

    /// <summary>�T�E���h</summary>
    CriSoundManager _soundManager;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _moveControl = GetComponent<MoveControl>();
        _groundCheck = GetComponent<GroundCheck>();
        _jumpControl = GetComponent<JumpControl>();
        _extraForce = GetComponent<ExtraForce>();
        _passiveCol = GetComponent<Collider>();
        _effect = EffectManager.Instance;
        _soundManager = CriSoundManager.Instance;
    }

    private void Update()
    {
        // �ړ����x���X�V����
        SetSpeed();
        
        // �ڒn������X�V����
        SetIsOnGround();
    }

    //-------------------------------------------------------------------------------
    // Update()���ŌĂ΂�鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�ړ����x��ݒ�</summary>
    private void SetSpeed() => _animator.SetFloat("Speed", _moveControl.CurrentSpeed);

    /// <summary>�ڒn������擾</summary>
    private bool GetIsOnGround() => _groundCheck.IsOnGround ? true : false;

    /// <summary>�ڒn�����ݒ�</summary>
    private void SetIsOnGround() =>
        _animator.SetBool("IsOnGround", GetIsOnGround() ? true : false);

    //-------------------------------------------------------------------------------
    // �ړ��̃R�[���o�b�N�C�x���g
    //-------------------------------------------------------------------------------

    /// <summary>�ړ��̕������Z���s��</summary>
    /// <summary>PlayerInput����Ă΂��</summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        // Move�A�N�V���������͂��ꂽ�ꍇ
        if (context.performed) _moveControl.Move(context.ReadValue<Vector2>()); 

        // Move�A�N�V�����������[�X���ꂽ�ꍇ
        else if (context.canceled) _moveControl.Move(Vector2.zero);
    }

    //-------------------------------------------------------------------------------
    // �X�v�����g�̃R�[���o�b�N�C�x���g
    //-------------------------------------------------------------------------------

    /// <summary>�ړ����x�̍X�V���s���i�A�j���[�V�����͑��x�ɉ����Ď����ŕύX�����j</summary>
    /// <summary>PlayerInput����Ă΂��</summary>
    public void OnSprint(InputAction.CallbackContext context)
    {
        // �X�v�����g�����͂��ꂽ�ꍇ
        if (context.performed) SetMoveSpeed(_sprintSpeed);

        // �X�v�����g�������[�X���ꂽ�ꍇ
        else if (context.canceled) SetMoveSpeed(_normalSpeed);
    }

    //-------------------------------------------------------------------------------
    // �X�v�����g�Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    private void SetMoveSpeed(float speed) => _moveControl.MoveSpeed = speed;

    //-------------------------------------------------------------------------------
    // �W�����v�̃R�[���o�b�N�C�x���g
    //-------------------------------------------------------------------------------

    /// <summary>�W�����v�̕������Z���s��</summary>
    /// <summary>PlayerInput����Ă΂��</summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        // �W�����v�����͂��ꂽ�ꍇ
        if (context.performed) Jump();
    }

    /// <summary>�W�����v�̃A�j���[�V�����Đ����s��</summary>
    /// <summary>JumpControl����Ă΂��</summary>
    public void OnJumpStart() => PlayJumpAnimation();

    //-------------------------------------------------------------------------------
    // �W�����v�Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�������Z</summary>
    private void Jump() => _jumpControl.Jump(true);

    /// <summary>1��ڂ̃W�����v���I���Ă��邩</summary>
    private bool CanJumpOneMore() => 
        _jumpControl.MaxAerialJumpCount <= _jumpControl.AerialJumpCount ? true : false;

    /// <summary>�A�j���[�V�������Đ�����</summary>
    private void PlayJumpAnimation() => 
        _animator.Play(CanJumpOneMore() ? "Double Jump" : "Jump Up");

    //-------------------------------------------------------------------------------
    // �_�b�V���̃R�[���o�b�N�C�x���g
    //-------------------------------------------------------------------------------

    /// <summary>�_�b�V���̕������Z�ƃA�j���[�V�����Đ����s��</summary>
    /// <summary>PlayerInput����Ă΂��</summary>
    public void OnDash(InputAction.CallbackContext context)
    {
        // �_�b�V�������͂��ꂽ�ꍇ
        if (context.performed && GetCanDash())
        {
            // �t���O���I�t�ɂ���
            SetCanDash();

            // �A�j���[�V�������Đ�����
            PlayDashAnimation();

            // �������Z������
            Dash();

            // �t���O���I���ɂ���
            Invoke(nameof(SetCanDash), _dashCoolTime);
        }
    }

    //-------------------------------------------------------------------------------
    // �_�b�V���Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�t���O���擾</summary>
    private bool GetCanDash() => _animator.GetBool("CanDash");

    /// <summary>�t���O��ݒ�</summary>
    private void SetCanDash() =>
        _animator.SetBool("CanDash", ! GetCanDash());

    /// <summary>�������Z</summary>
    void Dash() => _extraForce.AddForce(transform.forward * _dashDistance);

    /// <summary>�A�j���[�V�������Đ�</summary>
    void PlayDashAnimation() => _animator.Play("Dash Start");

    //-------------------------------------------------------------------------------
    // �U���̃R�[���o�b�N�C�x���g
    //-------------------------------------------------------------------------------

    /// <summary>�U���̃A�j���[�V�����J�ڂ��s��</summary>
    /// <summary>PlayerInput����Ă΂��</summary>
    public void OnAttack(InputAction.CallbackContext context)
    {
        // �U���A�N�V���������������ꂽ�ꍇ
        if (context.interaction is HoldInteraction && context.performed) 
            SetSpecialAttackTrigger();

        // �U���A�N�V�����������ꂽ�ꍇ
        else if (context.interaction is PressInteraction && context.performed)
            SetNormalAttackTrigger();
    }

    /// <summary>�U����������u�ԂɌĂ΂��C�x���g</summary>
    /// <param name="currentColIndex">�U���R���C�_�[�̎��ʎq</param>
    /// <param name="attackEffectIndex">�U���G�t�F�N�g�̎��ʎq</param>
    public void AttackImpactEvent(int colliderIndex)
    {
        // �U���R���C�_�[��L����
        EnableCol(colliderIndex);

        // �R���C�_�[�̎��ʎq��ݒ�
        SetCurrentColIndex(colliderIndex);

        // �U���R���C�_�[�𖳌���
        Invoke(nameof(DisableCol), 0.1f);

        // SE���Đ�
        PlaySE(_soundIndex, _volume);
    }

    //-------------------------------------------------------------------------------
    // �U���Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�ʏ�U���̃g���K�[���I��</summary>
    private void SetNormalAttackTrigger() => _animator.SetTrigger("Normal Attack");

    /// <summary>����U���̃g���K�[���I��</summary>
    private void SetSpecialAttackTrigger() => _animator.SetTrigger("Special Attack");

    /// <summary>���ʎq�Ŏw�肵���U���R���C�_�[��L��������</summary>
    private void EnableCol(int index) => _cols[index].enabled = true;

    /// <summary>���ʎq�Ŏw�肵���U���R���C�_�[�𖳌�������</summary>
    private void DisableCol() => _cols[_currentColIndex].enabled = false;

    /// <summary>�U���R���C�_�[�̎��ʎq��ݒ肷��</summary>
    private void SetCurrentColIndex(int index) => _currentColIndex = index;

    

    //-------------------------------------------------------------------------------
    // ��_���[�W���̃R�[���o�b�N�C�x���g
    //-------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // �G�ɍU�����ꂽ�ꍇ
        if (other.gameObject.tag == "EnemyAttack")
        {
            // �_���[�W�t���O���I��
            SetIsDamagedTrue();

            // �_���[�W�t���O���I�t
            Invoke(nameof(SetIsDamagedFalse), 0.1f);

            // �_���[�W�G�t�F�N�g��\��
            PlayDamageEffectOnClosestDamagePos(0, other);
        }
    }

    //-------------------------------------------------------------------------------
    // ��_���[�W���Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�t���O���I���ɐݒ肷��</summary>
    private void SetIsDamagedTrue() =>
        _animator.SetBool("IsDamaged", true);

    /// <summary>�t���O���I�t�ɐݒ肷��</summary>
    private void SetIsDamagedFalse() => 
        _animator.SetBool("IsDamaged", false);

    /// <summary>�U�����ꂽ�ʒu�Ƀ_���[�W�G�t�F�N�g��\������</summary>
    private void PlayDamageEffectOnClosestDamagePos(int index, Collider other) =>
        _effect.PlayDamageEffect(GetImpactPosition(other), index);

    /// <summary>����̍U���R���C�_�[�̈ʒu��Ԃ�</summary>
    private Vector3 GetDamagePosition(Collider other) => other.transform.position;

    /// <summary>����̍U���R���C�_�[����ł��߂��R���C�_�[��̈ʒu��Ԃ�</summary>
    private Vector3 GetImpactPosition(Collider other) 
        => _passiveCol.ClosestPoint(GetDamagePosition(other));

    //-------------------------------------------------------------------------------
    // SE�̍Đ��Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�L���[�����擾</summary>
    private string GetCueName(int index) => _soundManager._playerCueNames[index];

    /// <summary>SE���Đ�</summary>
    private void PlaySE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetCueName(index), volume);
}
