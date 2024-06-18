using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Effect;
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
        // スピードを更新する
        SetSpeed();
        
        // 接地判定を更新する
        SetIsOnGround();
    }

    /// <summary>移動の物理演算を行う</summary>
    /// <summary>PlayerInputから呼ばれる</summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        // Moveアクションが入力された場合
        if (context.performed) _moveControl.Move(context.ReadValue<Vector2>()); 

        // Moveアクションがリリースされた場合
        else if (context.canceled) _moveControl.Move(Vector2.zero);
    }

    /// <summary>移動速度の更新を行う（アニメーションは速度に応じて自動で変更される）</summary>
    /// <summary>PlayerInputから呼ばれる</summary>
    public void OnSprint(InputAction.CallbackContext context)
    {
        // スプリントが入力された場合
        if (context.performed) _moveControl.MoveSpeed = _sprintSpeed;

        // スプリントがリリースされた場合
        else if (context.canceled) _moveControl.MoveSpeed = _normalSpeed;
    }

    /// <summary>ジャンプの物理演算を行う</summary>
    /// <summary>PlayerInputから呼ばれる</summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        // ジャンプが入力された場合
        if (context.performed) _jumpControl.Jump(true);
    }

    /// <summary>ジャンプのアニメーション再生を行う</summary>
    /// <summary>JumpControlから呼ばれる</summary>
    public void OnJumpStart()
    {
        // 現在のジャンプ回数と最大ジャンプ回数を比較してジャンプを切り替える
        _animator.Play
            (_jumpControl.AerialJumpCount >= _jumpControl.MaxAerialJumpCount ? 
                "Double Jump" : "Jump Up");
    }

    /// <summary>ダッシュの物理演算とアニメーション再生を行う</summary>
    /// <summary>PlayerInputから呼ばれる</summary>
    public void OnDash(InputAction.CallbackContext context)
    {
        // ダッシュが入力された場合
        if (context.performed && _animator.GetBool("CanDash"))
        {
            _animator.SetBool("CanDash", false);
            _extraForce.AddForce(transform.forward * _dashDistance);
            _animator.Play("Dash Start");
            StartCoroutine(WaitThenCallAction(_dashCoolTime,
                () => _animator.SetBool("CanDash", true)));
        }
    }

    /// <summary>攻撃のアニメーション遷移を行う</summary>
    /// <summary>PlayerInputから呼ばれる</summary>
    public void OnAttack(InputAction.CallbackContext context)
    {
        // 攻撃アクションが長押しされた場合
        if(context.interaction is HoldInteraction && context.performed)
        {
            _animator.SetTrigger("Long Attack");
        }
        // 攻撃アクションが短く押された場合
        else if(context.interaction is PressInteraction && context.performed)
        {
            _animator.SetTrigger("Short Attack");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 敵に攻撃された場合
        if (other.gameObject.tag == "EnemyAttack")
        {
            SetIsDamagedTrue();
            Invoke(nameof(SetIsDamagedFalse), 0.1f);

            // 衝突位置を計算
            var hitPos = GetComponent<Collider>().
                ClosestPointOnBounds(other.transform.position);

            // ダメージエフェクトを再生
            PlayDamageEffect(hitPos, 0);
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

    /// <summary>アニメーターの「IsDamaged」フラグをオンにする</summary>
    void SetIsDamagedTrue()
    {
        _animator.SetBool("IsDamaged", true);
    }

    /// <summary>アニメーターの「IsDamaged」フラグをオフにする</summary>
    void SetIsDamagedFalse()
    {
        _animator.SetBool("IsDamaged", false);
    }

    /// <summary>引数で指定した時間だけ待機してアクションを呼ぶ</summary>
    /// <param name="waitTime">待ち時間</param>
    /// <param name="action">待機後に実行するアクション</param>
    IEnumerator WaitThenCallAction(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    /// <summary>右手のコライダーを有効化し、攻撃エフェクトを表示する</summary>
    /// <summary>アニメーションイベントから呼ばれる</summary>
    public void EnableRightHandCol()
    {
        //Debug.Log("RightHandCol Enabled.");
        _rightHandCol.enabled = true;
        Invoke(nameof(DisableRightHandCol), 0.1f);
        PlayAttackEffect(_rightHandCol.transform.position, 0);
    }

    /// <summary>右手のコライダーを無効化</summary>
    /// <summary>アニメーションイベントから呼ばれる</summary>
    public void DisableRightHandCol()
    {
        //Debug.Log("RightHandCol Disabled.");
        _rightHandCol.enabled = false;
    }

    /// <summary>左手のコライダーを有効化し、攻撃エフェクトを表示する</summary>
    /// <summary>アニメーションイベントから呼ばれる</summary>
    public void EnableLeftHandCol()
    {
        //Debug.Log("LeftHandCol Enabled.");
        _leftHandCol.enabled = true;
        Invoke(nameof(DisableLeftHandCol), 0.1f);
        PlayAttackEffect(_leftHandCol.transform.position, 0);
    }

    /// <summary>左手のコライダーを無効化</summary>
    /// <summary>アニメーションイベントから呼ばれる</summary>

    public void DisableLeftHandCol()
    {
        //Debug.Log("LeftHandCol Disabled.");
        _leftHandCol.enabled= false;
    }

    /// <summary>右足のコライダーを有効化し、攻撃エフェクトを表示する</summary>
    /// <summary>アニメーションイベントから呼ばれる</summary>
    public void EnableRightFootCol()
    {
        //Debug.Log("RightFootCol Enabled.");
        _rightFootCol.enabled = true;
        Invoke(nameof(DisableRightFootCol), 0.1f);
        PlayAttackEffect(_rightFootCol.transform.position, 0);
    }

    /// <summary>右足のコライダーを無効化</summary>
    /// <summary>アニメーションイベントから呼ばれる</summary>
    public void DisableRightFootCol()
    {
        //Debug.Log("RightFootCol Disabled.");
        _rightFootCol.enabled = false;
    }

    /// <summary>左足のコライダーを有効化し、攻撃エフェクトを表示する</summary>
    /// <summary>アニメーションイベントから呼ばれる</summary>
    public void EnableLeftFootCol()
    {
        //Debug.Log("LeftFootCol Enabled.");
        _leftFootCol.enabled = true;
        Invoke(nameof(DisableLeftFootCol), 0.1f);
        PlayAttackEffect(_leftFootCol.transform.position, 0);
    }

    /// <summary>左足のコライダーを無効化</summary>
    /// <summary>アニメーションイベントから呼ばれる</summary>
    public void DisableLeftFootCol()
    {
        //Debug.Log("LeftFootCol Disabled.");
        _leftFootCol.enabled= false;
    }

    /// <summary>座標を指定して攻撃エフェクトを表示する</summary>
    /// <param name="pos">エフェクトを表示させる座標</param>
    /// <param name="index">エフェクトのインデックス</param>
    private void PlayAttackEffect(Vector3 pos, int index)
    {
        EffectController.Instance.PlayAttackEffect(pos, index);
    }

    /// <summary>座標を指定してダメージエフェクトを表示する</summary>
    /// <param name="pos">エフェクトを表示させる座標</param>
    /// <param name="index">エフェクトのインデックス</param>
    private void PlayDamageEffect(Vector3 pos, int index)
    {
        EffectController.Instance.PlayDamageEffect(pos, index);
    }

    /// <summary>座標を指定して死亡エフェクトを表示する</summary>
    /// <param name="pos">エフェクトを表示させる座標</param>
    /// <param name="index">エフェクトのインデックス</param>
    private void PlayDeadEffect(Vector3 pos, int index)
    {
        EffectController.Instance.PlayDeadEffect(pos, index);
    }
}
