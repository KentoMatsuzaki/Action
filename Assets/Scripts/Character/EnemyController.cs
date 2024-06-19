using System;
using System.Collections;
using UnityEngine;

/// <summary>敵の制御</summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>アニメーター</summary>
    Animator _animator;

    /// <summary>攻撃用コライダー</summary>
    [SerializeField] Collider _attackCol;

    /// <summary>プレイヤー</summary>
    [SerializeField] Transform _player;

    /// <summary>体力</summary>
    [SerializeField] int _hp = 100;

    /// <summary>索敵距離</summary>
    [SerializeField] float _searchDistance;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        Attack();
    }

    //-------------------------------------------------------------------------------
    // 被ダメージ時のコールバックイベント
    //-------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーに攻撃を受けた場合 && ダメージを受けていない場合
        if (other.gameObject.tag == "PlayerAttack" && ! IsDamaged())
        {
            // 生きている場合
            if (IsAlive())
            {
                // ダメージ処理
                TakeDamage(other);
            }

            // 死んでいる場合
            if (IsDead())
            {
                // 死亡アニメーションを再生
                PlayDeathAnimation();
            }

            // ダウンする場合
            else if (IsDown())
            {
                // ダウンアニメーションを再生
                PlayDownAnimation();

                // ダメージフラグをオンにする
                SetDamaged(true);
            }

            // 通常時
            else
            {
                // ダメージフラグをオンにする
                SetDamaged(true);

                // ダメージフラグをオフにする
                Invoke(nameof(UnsetDamaged), 0.1f);

                // ヒットカウントを1だけ増加させる
                IncrementHitCount();
            }
        }
    }

    //-------------------------------------------------------------------------------
    // 被ダメージ時に関する処理
    //-------------------------------------------------------------------------------

    /// <summary>攻撃を受けている最中か</summary>
    private bool IsDamaged() => _animator.GetBool("IsDamaged");

    /// <summary>生きているか</summary>
    private bool IsAlive() => _hp > 0;

    /// <summary>死んでいるか</summary>
    private bool IsDead() => _hp <= 0;

    /// <summary>ダウンするか</summary>
    bool IsDown() => _animator.GetInteger("HitCount") == 4 ? true : false;

    /// <summary>ダメージ量を算出</summary>
    private int CalDamage(Collider other) => other.GetComponent<Attacker>().Power;

    /// <summary>体力を設定</summary>
    private void SetHP(int damage) => _hp -= damage;

    /// <summary>ダメージ処理</summary>
    private void TakeDamage(Collider other) => SetHP(CalDamage(other));

    /// <summary>死亡アニメーションを再生</summary>
    private void PlayDeathAnimation() => _animator.Play("Died");

    /// <summary>ダウンアニメーションを再生</summary>
    private void PlayDownAnimation() => _animator.Play("Get Down");

    /// <summary>ダメージフラグを設定</summary>
    private void SetDamaged(bool value) => _animator.SetBool("IsDamaged", value);

    /// <summary>ダメージフラグを解除</summary>
    private void UnsetDamaged() => SetDamaged(false);

    /// <summary>ヒットカウントを1増加させる</summary>
    private void IncrementHitCount() => 
        _animator.SetInteger("HitCount", (_animator.GetInteger("HitCount") + 1));

    /// <summary>ヒットカウントを0に設定する</summary>
    public void ResetHitCount() => _animator.SetInteger("HitCount", 0);

    /// <summary>少し待ってから起き上がりトリガーをオンにする</summary>
    public void WaitSetRiseTrigger()
        => Invoke(nameof(SetRiseTrigger), 2.0f);

    /// <summary>起き上がりトリガーをオンにする</summary>
    private void SetRiseTrigger() => _animator.SetTrigger("Rise");

    //-------------------------------------------------------------------------------
    // 攻撃のイベント
    //-------------------------------------------------------------------------------

    /// <summary>攻撃のアニメーション再生を行う</summary>
    public void Attack()
    {
        if(CanAttack()) PlayAttackAnimation();
    }

    /// <summary>攻撃の命中イベント</summary>
    public void AttackImpactEvent()
    {
        // 攻撃コライダーを有効化
        EnableAttackCol();

        // 攻撃コライダーを無効化
        Invoke(nameof(DisableAttackCol), 0.1f);
    }

    //-------------------------------------------------------------------------------
    // 攻撃に関する処理
    //-------------------------------------------------------------------------------

    /// <summary>攻撃可能かどうか</summary>
    private bool CanAttack() => _animator.GetBool("IsDamaged") ? false : true;

    private void PlayAttackAnimation() => _animator.Play("Attack");

    /// <summary>攻撃コライダーを有効化</summary>
    private void EnableAttackCol() => _attackCol.enabled = true;

    /// <summary>攻撃コライダーを無効化</summary>
    private void DisableAttackCol() => _attackCol.enabled = false;

    //-------------------------------------------------------------------------------
    // 移動に関する処理
    //-------------------------------------------------------------------------------

    /// <summary>プレイヤーが近くにいるかどうか</summary>
    public bool IsPlayerClose()
    {
        // プレイヤーとの距離
        float distance = Vector3.Distance(transform.position, _player.position);

        // 索敵距離とプレイヤーとの距離を比較する
        return _searchDistance >= distance ? true : false;
    }

    public void MoveTowardPlayer()
    {
        Vector3.MoveTowards(transform.position, _player.position, 2f * Time.deltaTime);
    }
}
