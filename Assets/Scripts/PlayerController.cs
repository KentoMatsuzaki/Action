using UnityEngine;
using UnityEngine.InputSystem;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Effect;
using System.Collections;
using System;
using UnityEngine.InputSystem.Interactions;

/// <summary>プレイヤー制御</summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>アニメーター</summary>
    Animator _animator;

    /// <summary>移動制御</summary>
    MoveControl _moveControl;

    /// <summary>ジャンプ制御</summary>
    JumpControl _jumpControl;

    /// <summary>物理制御</summary>
    ExtraForce _extraForce;

    /// <summary>接地判定</summary>
    GroundCheck _groundCheck;

    /// <summary>右手の攻撃判定用コライダー</summary>
    [SerializeField, Header("右手の攻撃判定用コライダー")] Collider _rightHandCol;

    /// <summary>左手の攻撃判定用コライダー</summary>
    [SerializeField, Header("左手の攻撃判定用コライダー")] Collider _leftHandCol;

    /// <summary>右足の攻撃判定用コライダー</summary>
    [SerializeField, Header("右足の攻撃判定用コライダー")] Collider _rightFootCol;

    /// <summary>左足の攻撃判定用コライダー</summary>
    [SerializeField, Header("左足の攻撃判定用コライダー")] Collider _leftFootCol;

    /// <summary>通常時の速度</summary>
    [SerializeField, Header("通常時の移動速度")] float _normalSpeed = 1.2f;

    /// <summary>スプリント時の速度</summary>
    [SerializeField, Header("スプリント時の移動速度")] float _sprintSpeed = 4.0f;

    /// <summary>ブリンクのクールタイム</summary>
    [SerializeField, Header("ブリンクのクールタイム")] float _dashCoolTime = 0.5f;

    /// <summary>ブリンクの移動距離</summary>
    [SerializeField, Header("ブリンクの移動距離")] float _dashDistance = 15f;

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
    /// <param name="context">ボタン入力時に呼び出されるコールバック</param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // 入力のログを出力する
        Debug.Log("Move is Pressed.");

        // Moveアクションが押された場合
        if (context.performed) _moveControl.Move(context.ReadValue<Vector2>()); 

        // Moveアクションがリリースされた場合
        else if (context.canceled) _moveControl.Move(Vector2.zero);
    }

    /// <summary>PlayerInputから呼ばれるイベント</summary>
    /// <param name="context">ボタン入力時に呼び出されるコールバック</param>
    public void OnSprint(InputAction.CallbackContext context)
    {
        // 入力のログを出力する
        Debug.Log("Sprint is Pressed.");

        // スプリントが押された場合
        if (context.performed) _moveControl.MoveSpeed = _sprintSpeed;

        // スプリントがリリースされた場合
        else if (context.canceled) _moveControl.MoveSpeed = _normalSpeed;
    }

    /// <summary>PlayerInputから呼ばれるイベント</summary>
    /// <param name="context">ボタン入力時に呼び出されるコールバック</param>
    public void OnJump(InputAction.CallbackContext context)
    {
        // 入力のログを出力する
        Debug.Log("Jump is Pressed.");

        // ジャンプが押された場合
        if (context.performed) _jumpControl.Jump(true);
    }

    /// <summary>JumpControlから呼ばれるコールバック</summary>
    public void OnJumpStart()
    {
        // 呼び出しのログを出力する
        Debug.Log("Jump Start is Called");

        // 現在のジャンプ回数と最大ジャンプ回数を比較してジャンプを切り替える
        _animator.Play
            (_jumpControl.AerialJumpCount >= _jumpControl.MaxAerialJumpCount ? 
                "Double Jump" : "Jump Up");
    }

    /// <summary>PlayerInputから呼ばれるイベント</summary>
    /// <param name="context">ボタン入力時に呼び出されるコールバック</param>
    public void OnDash(InputAction.CallbackContext context)
    {
        // 入力のログを出力する
        Debug.Log("Dash is Pressed.");

        // ダッシュ可能ができる場合
        if (context.performed && _animator.GetBool("CanDash"))
        {
            _animator.SetBool("CanDash", false);
            _extraForce.AddForce(transform.forward * _dashDistance);
            _animator.Play("Dash Start");
            StartCoroutine(Wait(_dashCoolTime,
                () => _animator.SetBool("CanDash", true)));
        }
    }


    /// <summary>PlayerInputから呼ばれるイベント</summary>
    /// <param name="context">ボタン入力時に呼び出されるコールバック</param>
    public void OnAttack(InputAction.CallbackContext context)
    {
        // 入力のログを出力する
        Debug.Log("Attack is Pressed.");

        // 攻撃アクションが長押しされた場合
        if(context.interaction is HoldInteraction && context.performed)
        {
            _animator.SetTrigger("Long Attack");
        }

        // 攻撃アクションが短く押された場合
        else if(context.interaction is PressInteraction && context.performed)
        {
            Debug.Log("Set Par");
            _animator.SetTrigger("Short Attack");
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

    /// <summary>クールタイム用のコルーチン</summary>
    /// <param name="time">待ち時間</param>
    /// <param name="action">待機後に実行する処理</param>
    IEnumerator Wait(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }
}
