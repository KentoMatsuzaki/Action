using UnityEngine;
using System;
using System.Collections;

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

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーに攻撃された場合
        if (other.gameObject.tag == "PlayerAttack" && !_animator.GetBool("IsDamaged"))
        {
            // 被ダメージ処理
            GetDamage(other);

            // 死亡状態に遷移して処理を抜ける
            if (IsDied())
            {
                _animator.Play("Died");
                return;
            }
            // ダウン状態に遷移する
            else if (IsReadyForDown())
            {
                _animator.Play("Get Down");
                // ダメージフラグをオンにする
                SetIsDamagedTrue();
            }
            // やられ状態に遷移する
            else
            {
                // ダメージフラグをオンにする
                SetIsDamagedTrue();

                // ダメージフラグをオフにする
                Invoke(nameof(SetIsDamagedFalse), 0.1f);

                // ヒットカウントを1だけ増加させる
                AddHitCountByOne();
            }
        }
    }

    

    /// <summary>被ダメージ処理</summary>
    void GetDamage(Collider other)
    {
        // 被ダメージ量
        int damage = other.GetComponent<Attacker>().GetAttackDamage();
        
        // 体力を更新する
        if(_hp > 0) DecreaseHP(damage);
    }

    /// <summary>受けたダメージ分だけ体力を減少させる</summary>
    /// <param name="damage">被ダメージ量</param>
    void DecreaseHP(int damage) => _hp -= damage;

    /// <summary>死亡しているか</summary>
    bool IsDied() => _hp <= 0 ? true : false;

    /// <summary>次の攻撃を受けてダウンするか</summary>
    bool IsReadyForDown() => _animator.GetInteger("HitCount") == 4 ? true : false;

    /// <summary>アニメーターの「IsDamaged」フラグをオンにする</summary>
    public void SetIsDamagedTrue() => _animator.SetBool("IsDamaged", true);

    /// <summary>アニメーターの「IsDamaged」フラグをオフにする</summary>
    public void SetIsDamagedFalse() => _animator.SetBool("IsDamaged", false);

    /// <summary>ヒットカウントを1増加させる</summary>
    void AddHitCountByOne() => _animator.SetInteger("HitCount", (_animator.GetInteger("HitCount") + 1));

    /// <summary>ヒットカウントを0に設定する</summary>
    public void ResetHitCount() => _animator.SetInteger("HitCount", 0);

    /// <summary>攻撃状態に遷移する</summary>
    public void Attack()
    {
        if(CanAttack()) _animator.Play("Attack");
    }

    /// <summary>攻撃可能かどうか</summary>
    /// <returns>攻撃を受けていない：true / 攻撃を受けている：false</returns>
    public bool CanAttack() => _animator.GetBool("IsDamaged") ? false : true;

    /// <summary>攻撃の命中イベント</summary>
    public void AttackImpactEvent()
    {
        _attackCol.enabled = true;
        Invoke(nameof(AttackEndEvent), 0.1f);
    }

    /// <summary>攻撃の終了イベント</summary>
    public void AttackEndEvent() => _attackCol.enabled = false;

    /// <summary>待機してからアクションを実行する</summary>
    /// <param name="waitTime">待機時間</param>
    /// <param name="action">アクション</param>
    IEnumerator WaitThenCallAction(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    /// <summary>起き上がり状態に遷移する</summary>
    public void WaitForSecondsToRiseUp()
    {
        StartCoroutine(WaitThenCallAction(2f, () => _animator.SetTrigger("RiseUp")));
    }

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
