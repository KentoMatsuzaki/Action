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
        // プレイヤーと接触した場合
        if (other.gameObject.tag == "Player")
        {
            // 被ダメージ処理＋死亡判定
            GetDamaged(other);

            // ダウンする場合
            if(IsDown())
            {
                _animator.Play("Get Down");
                SetIsDamagedTrue();
                // アニメーションイベントでFalseにする
            }
            // 攻撃を受けていない場合
            if(!_animator.GetBool("IsDamaged"))
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
    void GetDamaged(Collider other)
    {
        // 被ダメージ量
        int damage = other.GetComponent<AttackData>().GetAttackDamage();
        
        // 体力を更新する
        SetHP(damage);

        // 死亡判定
        if(IsDied()) _animator.SetTrigger("IsDied");
    }

    /// <summary>体力を更新する</summary>
    /// <param name="damage">被ダメージ量</param>
    void SetHP(int damage)
    {
        // 体力を被ダメージ量だけ減らす
        _hp -= damage;
    }

    /// <summary>死亡しているかどうか</summary>
    bool IsDied()
    {
        return _hp <= 0 ? true : false;
    }

    /// <summary>ダウンするかどうか</summary>
    bool IsDown()
    {
        // 攻撃を4回連続で受けているかどうか
        return _animator.GetInteger("HitCount") == 4 ? true : false;
    }

    /// <summary>アニメーターの「IsDamaged」フラグをオンにする</summary>
    public void SetIsDamagedTrue()
    {
        _animator.SetBool("IsDamaged", true);
    }

    /// <summary>アニメーターの「IsDamaged」フラグをオフにする</summary>
    public void SetIsDamagedFalse()
    {
        _animator.SetBool("IsDamaged", false);
    }

    /// <summary>ヒットカウントを1増加させる</summary>
    void AddHitCountByOne()
    {
        _animator.SetInteger("HitCount", (_animator.GetInteger("HitCount") + 1));
    }

    /// <summary>ヒットカウントを0に設定する</summary>
    public void ResetHitCount()
    {
        _animator.SetInteger("HitCount", 0);
    }

    /// <summary>攻撃の開始処理</summary>
    public void Attack()
    {
        // 攻撃可能な場合
        if(CanAttack())
        {
            // 攻撃トリガーをオン
            _animator.Play("Attack");
        }
    }

    /// <summary>攻撃可能かどうかを返す</summary>
    /// <returns>ダメージを受けている：false ダメージを受けていない：true</returns>
    public bool CanAttack()
    {
        return _animator.GetBool("IsDamaged") ? false : true;
    }

    /// <summary>攻撃の衝撃イベント</summary>
    public void AttackImpactEvent()
    {
        _attackCol.enabled = true;
        Invoke(nameof(AttackEndEvent), 0.1f);
    }

    /// <summary>攻撃の終了イベント</summary>
    public void AttackEndEvent()
    {
        _attackCol.enabled = false;
    }

    /// <summary>引数で指定した時間だけ待機してアクションを呼ぶ</summary>
    /// <param name="waitTime">待ち時間</param>
    /// <param name="action">待機後に実行するアクション</param>
    IEnumerator Wait(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    /// <summary>2f待機してから起き上がりアニメーションを再生する</summary>
    public void WaitForSecondsToRiseUp()
    {
        StartCoroutine(Wait(2f, () => _animator.SetTrigger("RiseUp")));
    }

    /// <summary>プレイヤーが近くにいるかどうかを返す</summary>
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
