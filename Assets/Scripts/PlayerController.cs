using UnityEngine;
using UnityEngine.InputSystem;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Effect;

/// <summary>プレイヤー制御クラス</summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>プレイヤーのアニメーター</summary>
    Animator _animator;

    /// <summary>プレイヤーの移動制御</summary>
    MoveControl _moveControl;

    /// <summary>プレイヤーのジャンプ制御</summary>
    JumpControl _jumpControl;

    /// <summary>プレイヤーの物理制御</summary>
    ExtraForce _extraForce;

    /// <summary>プレイヤーの接地判定</summary>
    GroundCheck _groundCheck;

    /// <summary>通常時の速度</summary>
    public float _normalSpeed = 1.2f;

    /// <summary>スプリント時の速度</summary>
    public float _sprintSpeed = 4.0f;

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
        // スピードを更新
        SetSpeed();
        
        // 接地判定を更新
        SetIsOnGround();
    }

    /// <summary>PlayerInputから呼ばれるイベント</summary>
    /// <param name="context">入力時に呼び出されるコールバック関数の状態</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // Moveアクションが押された場合
        if (context.performed)
        {
            Debug.Log("Move is Pressed.");
            _moveControl.Move(context.ReadValue<Vector2>()); 
        }
        // Moveアクションがリリースされた場合
        else if (context.canceled)
        {
            Debug.Log("Move is Released.");
            _moveControl.Move(Vector2.zero);
        }
    }

    /// <summary>PlayerInputから呼ばれるイベント</summary>
    /// <param name="context">入力時に呼び出されるコールバック関数の状態</param>
    public void OnSprint(InputAction.CallbackContext context)
    {
        // スプリントアクションが押された場合
        if (context.performed)
        {
            Debug.Log("Sprint is Pressed.");
            _moveControl.MoveSpeed = _sprintSpeed;
        }
        // スプリントアクションがリリースされた場合
        else if (context.canceled)
        {
            Debug.Log("Sprint is Released.");
            _moveControl.MoveSpeed = _normalSpeed;
        }
    }

    /// <summary>PlayerInputから呼ばれるイベント</summary>
    /// <param name="context">入力時に呼び出されるコールバック関数の状態</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        // ジャンプアクションが押された場合
        if (context.performed)
        {
            Debug.Log("Jump is Pressed.");
            _jumpControl.Jump(true);
        }
    }

    /// <summary>JumpControlから呼ばれるイベント</summary>
    public void OnJumpStart()
    {
        if (_jumpControl.AerialJumpCount >= _jumpControl.MaxAerialJumpCount)
        {
            _animator.Play("Double Jump");
        }
        else
        {
            _animator.Play("Jump Up");
        }
    }

    /// <summary>PlayerInputから呼ばれるイベント</summary>
    /// <param name="context">入力時に呼び出されるコールバック関数の状態</param>
    public void OnDash(InputAction.CallbackContext context)
    {
        // ダッシュ可能なら
        if(context.performed)
        {
            Debug.Log("Dash is Pressed.");
            _animator.SetBool("CanDash", false);
            _extraForce.AddForce(transform.forward * 15f);
            _animator.Play("Dash Start");
        }
    }

    /// <summary>アニメーターの「Speed」パラメーターを更新する</summary>
    void SetSpeed()
    {
        _animator.SetFloat("Speed", _moveControl.CurrentSpeed);
    }

    /// <summary>アニメーターの「IsGround」パラメーターを更新する</summary>
    void SetIsOnGround()
    {
        _animator.SetBool("IsOnGround", _groundCheck.IsOnGround ? true : false);
    }
}
