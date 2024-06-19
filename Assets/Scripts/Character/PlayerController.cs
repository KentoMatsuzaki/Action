using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
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

    /// <summary>攻撃判定</summary>
    [SerializeField, Header("攻撃判定")] List<Collider> _cols = new List<Collider>();

    /// <summary>通常時の速度</summary>
    [SerializeField, Header("通常時の移動速度")] float _normalSpeed = 1.2f;

    /// <summary>スプリント時の速度</summary>
    [SerializeField, Header("スプリント時の移動速度")] float _sprintSpeed = 4.0f;

    /// <summary>ブリンクのクールタイム</summary>
    [SerializeField, Header("ブリンクのクールタイム")] float _dashCoolTime = 0.5f;

    /// <summary>ブリンクの移動距離</summary>
    [SerializeField, Header("ブリンクの移動距離")] float _dashDistance = 15f;

    /// <summary>SEの識別子</summary>
    [SerializeField, Header("SEのインデックス")] int _soundIndex;

    /// <summary>SEの識別子</summary>
    [SerializeField, Header("SEのボリューム")] float _volume;

    /// <summary>ダメージコライダー</summary>
    Collider _passiveCol;

    /// <summary>攻撃コライダーの識別子</summary>
    int _currentColIndex;

    /// <summary>エフェクト</summary>
    EffectManager _effect;

    /// <summary>サウンド</summary>
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
        // 移動速度を更新する
        SetSpeed();
        
        // 接地判定を更新する
        SetIsOnGround();
    }

    //-------------------------------------------------------------------------------
    // Update()内で呼ばれる処理
    //-------------------------------------------------------------------------------

    /// <summary>移動速度を設定</summary>
    private void SetSpeed() => _animator.SetFloat("Speed", _moveControl.CurrentSpeed);

    /// <summary>接地判定を取得</summary>
    private bool GetIsOnGround() => _groundCheck.IsOnGround ? true : false;

    /// <summary>接地判定を設定</summary>
    private void SetIsOnGround() =>
        _animator.SetBool("IsOnGround", GetIsOnGround() ? true : false);

    //-------------------------------------------------------------------------------
    // 移動のコールバックイベント
    //-------------------------------------------------------------------------------

    /// <summary>移動の物理演算を行う</summary>
    /// <summary>PlayerInputから呼ばれる</summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        // Moveアクションが入力された場合
        if (context.performed) _moveControl.Move(context.ReadValue<Vector2>()); 

        // Moveアクションがリリースされた場合
        else if (context.canceled) _moveControl.Move(Vector2.zero);
    }

    //-------------------------------------------------------------------------------
    // スプリントのコールバックイベント
    //-------------------------------------------------------------------------------

    /// <summary>移動速度の更新を行う（アニメーションは速度に応じて自動で変更される）</summary>
    /// <summary>PlayerInputから呼ばれる</summary>
    public void OnSprint(InputAction.CallbackContext context)
    {
        // スプリントが入力された場合
        if (context.performed) SetMoveSpeed(_sprintSpeed);

        // スプリントがリリースされた場合
        else if (context.canceled) SetMoveSpeed(_normalSpeed);
    }

    //-------------------------------------------------------------------------------
    // スプリントに関する処理
    //-------------------------------------------------------------------------------

    private void SetMoveSpeed(float speed) => _moveControl.MoveSpeed = speed;

    //-------------------------------------------------------------------------------
    // ジャンプのコールバックイベント
    //-------------------------------------------------------------------------------

    /// <summary>ジャンプの物理演算を行う</summary>
    /// <summary>PlayerInputから呼ばれる</summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        // ジャンプが入力された場合
        if (context.performed) Jump();
    }

    /// <summary>ジャンプのアニメーション再生を行う</summary>
    /// <summary>JumpControlから呼ばれる</summary>
    public void OnJumpStart() => PlayJumpAnimation();

    //-------------------------------------------------------------------------------
    // ジャンプに関する処理
    //-------------------------------------------------------------------------------

    /// <summary>物理演算</summary>
    private void Jump() => _jumpControl.Jump(true);

    /// <summary>1回目のジャンプを終えているか</summary>
    private bool CanJumpOneMore() => 
        _jumpControl.MaxAerialJumpCount <= _jumpControl.AerialJumpCount ? true : false;

    /// <summary>アニメーションを再生する</summary>
    private void PlayJumpAnimation() => 
        _animator.Play(CanJumpOneMore() ? "Double Jump" : "Jump Up");

    //-------------------------------------------------------------------------------
    // ダッシュのコールバックイベント
    //-------------------------------------------------------------------------------

    /// <summary>ダッシュの物理演算とアニメーション再生を行う</summary>
    /// <summary>PlayerInputから呼ばれる</summary>
    public void OnDash(InputAction.CallbackContext context)
    {
        // ダッシュが入力された場合
        if (context.performed && GetCanDash())
        {
            // フラグをオフにする
            SetCanDash();

            // アニメーションを再生する
            PlayDashAnimation();

            // 物理演算をする
            Dash();

            // フラグをオンにする
            Invoke(nameof(SetCanDash), _dashCoolTime);
        }
    }

    //-------------------------------------------------------------------------------
    // ダッシュに関する処理
    //-------------------------------------------------------------------------------

    /// <summary>フラグを取得</summary>
    private bool GetCanDash() => _animator.GetBool("CanDash");

    /// <summary>フラグを設定</summary>
    private void SetCanDash() =>
        _animator.SetBool("CanDash", ! GetCanDash());

    /// <summary>物理演算</summary>
    void Dash() => _extraForce.AddForce(transform.forward * _dashDistance);

    /// <summary>アニメーションを再生</summary>
    void PlayDashAnimation() => _animator.Play("Dash Start");

    //-------------------------------------------------------------------------------
    // 攻撃のコールバックイベント
    //-------------------------------------------------------------------------------

    /// <summary>攻撃のアニメーション遷移を行う</summary>
    /// <summary>PlayerInputから呼ばれる</summary>
    public void OnAttack(InputAction.CallbackContext context)
    {
        // 攻撃アクションが長押しされた場合
        if (context.interaction is HoldInteraction && context.performed) 
            SetSpecialAttackTrigger();

        // 攻撃アクションが押された場合
        else if (context.interaction is PressInteraction && context.performed)
            SetNormalAttackTrigger();
    }

    /// <summary>攻撃が当たる瞬間に呼ばれるイベント</summary>
    /// <param name="currentColIndex">攻撃コライダーの識別子</param>
    /// <param name="attackEffectIndex">攻撃エフェクトの識別子</param>
    public void AttackImpactEvent(int colliderIndex)
    {
        // 攻撃コライダーを有効化
        EnableCol(colliderIndex);

        // コライダーの識別子を設定
        SetCurrentColIndex(colliderIndex);

        // 攻撃コライダーを無効化
        Invoke(nameof(DisableCol), 0.1f);

        // SEを再生
        PlaySE(_soundIndex, _volume);
    }

    //-------------------------------------------------------------------------------
    // 攻撃に関する処理
    //-------------------------------------------------------------------------------

    /// <summary>通常攻撃のトリガーをオン</summary>
    private void SetNormalAttackTrigger() => _animator.SetTrigger("Normal Attack");

    /// <summary>特殊攻撃のトリガーをオン</summary>
    private void SetSpecialAttackTrigger() => _animator.SetTrigger("Special Attack");

    /// <summary>識別子で指定した攻撃コライダーを有効化する</summary>
    private void EnableCol(int index) => _cols[index].enabled = true;

    /// <summary>識別子で指定した攻撃コライダーを無効化する</summary>
    private void DisableCol() => _cols[_currentColIndex].enabled = false;

    /// <summary>攻撃コライダーの識別子を設定する</summary>
    private void SetCurrentColIndex(int index) => _currentColIndex = index;

    

    //-------------------------------------------------------------------------------
    // 被ダメージ時のコールバックイベント
    //-------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // 敵に攻撃された場合
        if (other.gameObject.tag == "EnemyAttack")
        {
            // ダメージフラグをオン
            SetIsDamagedTrue();

            // ダメージフラグをオフ
            Invoke(nameof(SetIsDamagedFalse), 0.1f);

            // ダメージエフェクトを表示
            PlayDamageEffectOnClosestDamagePos(0, other);
        }
    }

    //-------------------------------------------------------------------------------
    // 被ダメージ時に関する処理
    //-------------------------------------------------------------------------------

    /// <summary>フラグをオンに設定する</summary>
    private void SetIsDamagedTrue() =>
        _animator.SetBool("IsDamaged", true);

    /// <summary>フラグをオフに設定する</summary>
    private void SetIsDamagedFalse() => 
        _animator.SetBool("IsDamaged", false);

    /// <summary>攻撃された位置にダメージエフェクトを表示する</summary>
    private void PlayDamageEffectOnClosestDamagePos(int index, Collider other) =>
        _effect.PlayDamageEffect(GetImpactPosition(other), index);

    /// <summary>相手の攻撃コライダーの位置を返す</summary>
    private Vector3 GetDamagePosition(Collider other) => other.transform.position;

    /// <summary>相手の攻撃コライダーから最も近いコライダー上の位置を返す</summary>
    private Vector3 GetImpactPosition(Collider other) 
        => _passiveCol.ClosestPoint(GetDamagePosition(other));

    //-------------------------------------------------------------------------------
    // SEの再生に関する処理
    //-------------------------------------------------------------------------------

    /// <summary>キュー名を取得</summary>
    private string GetCueName(int index) => _soundManager._playerCueNames[index];

    /// <summary>SEを再生</summary>
    private void PlaySE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetCueName(index), volume);
}
