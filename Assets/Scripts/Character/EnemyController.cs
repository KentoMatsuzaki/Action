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

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーと接触した場合
        if (other.gameObject.tag == "Player")
        {
            // 連続で4回の攻撃を受けている場合
            if(_animator.GetInteger("HitCount") == 4)
            {
                _animator.Play("Get Down");
                SetIsDamagedTrue();
            }
            // ダメージを受けている最中ではない場合
            else if(_animator.GetBool("IsDamaged") == false)
            {
                // ダメージフラグをオンにする
                SetIsDamagedTrue();

                // 0.1f後にダメージフラグをオフにする
                Invoke(nameof(SetIsDamagedFalse), 0.1f);

                // ヒットカウントを1だけ増加させる
                _animator.SetInteger("HitCount", (_animator.GetInteger("HitCount") + 1));
            }
        }
    }

    //public bool CanAttack()
    //{

    //}

    public void Attack()
    {
        
    }

    /// <summary>攻撃の衝撃イベント</summary>
    public void AttackImpactEvent()
    {
        _attackCol.enabled = true;
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

    /// <summary>アニメーターのヒットカウントを0で初期化する</summary>
    public void ResetHitCount()
    {
        _animator.SetInteger("HitCount", 0);
    }

    /// <summary>2f待機してから起き上がりアニメーションを再生する</summary>
    public void WaitForSecondsToRiseUp()
    {
        StartCoroutine(Wait(2f, () => _animator.SetTrigger("RiseUp")));
    }
}
