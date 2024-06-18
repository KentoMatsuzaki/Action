using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
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

    /// <summary>ダメージコライダー</summary>
    Collider _passiveCol;

    /// <summary>攻撃コライダーの識別子</summary>
    int _currentColIndex;

    /// <summary>エフェクト</summary>
    EffectManager _effect;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _moveControl = GetComponent<MoveControl>();
        _groundCheck = GetComponent<GroundCheck>();
        _jumpControl = GetComponent<JumpControl>();
        _extraForce = GetComponent<ExtraForce>();
        _passiveCol = GetComponent<Collider>();
        _effect = EffectManager.Instance;
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

    /// <summary>移動速度を設定する</summary>
    private void SetSpeed() => _animator.SetFloat("Speed", _moveControl.CurrentSpeed);

    /// <summary>接地判定を取得する</summary>
    private bool IsOnGround() => _groundCheck.IsOnGround ? true : false;

    /// <summary>接地判定を設定する</summary>
    private void SetIsOnGround() => 
        _animator.SetBool("IsOnGround", IsOnGround() ? true : false);

    /// <summary>ダメージフラグを取得する</summary>
    private bool IsDamaged() => _animator.GetBool("IsDamaged") ? true : false;

    /// <summary>ダメージフラグを設定する</summary>
    private void SetIsDamaged() => 
        _animator.SetBool("IsDamaged", IsDamaged() ? false : true);

    /// <summary>攻撃が当たる瞬間に呼ばれるイベント</summary>
    /// <param name="currentColIndex">攻撃コライダーの識別子</param>
    /// <param name="attackEffectIndex">攻撃エフェクトの識別子</param>
    public void AttackImpactEvent(int colliderIndex, int effectIndex)
    {
        EnableCol(colliderIndex);
        SetCurrentColIndex(colliderIndex);
        Invoke(nameof(DisableCol), 0.1f);
        PlayAttackEffect(effectIndex);
    }

    /// <summary>識別子で指定した攻撃コライダーを有効化する</summary>
    private void EnableCol(int index) => _cols[index].enabled = true;

    /// <summary>識別子で指定した攻撃コライダーを無効化する</summary>
    private void DisableCol() => _cols[_currentColIndex].enabled = false;

    /// <summary>攻撃コライダーの識別子を設定する</summary>
    private void SetCurrentColIndex(int index) => _currentColIndex = index;

    /// <summary>攻撃した位置に攻撃エフェクトを表示する</summary>
    private void PlayAttackEffect(int index) =>
        _effect.PlayAttackEffect(GetAttackPosition(), index);

    /// <summary>攻撃された位置にダメージエフェクトを表示する</summary>
    private void PlayDamageEffect(int index, Collider other) =>
        _effect.PlayDamageEffect(GetImpactPosition(other), index);

    /// <summary>自分の攻撃コライダーの位置を返す</summary>
    private Vector3 GetAttackPosition() => _cols[_currentColIndex].transform.position;

    /// <summary>相手の攻撃コライダーの位置を返す</summary>
    private Vector3 GetDamagePosition(Collider other) => other.transform.position;

    /// <summary>相手の攻撃コライダーから最も近いコライダー上の位置を返す</summary>
    private Vector3 GetImpactPosition(Collider other) 
        => _passiveCol.ClosestPoint(GetDamagePosition(other));
}
