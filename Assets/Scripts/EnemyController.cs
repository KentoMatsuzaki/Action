using UnityEngine;
using System;
using System.Collections;

/// <summary>敵の制御</summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>アニメーター</summary>
    Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーと接触した場合
        if (other.gameObject.tag == "Player")
        {
            if(_animator.GetInteger("HitCount") == 4)
            {
                _animator.Play("Get Down");
            }
            if(_animator.GetBool("IsDamaged") == false)
            {
                // ダメージフラグをオンにする
                SetIsDamagedTrue();

                // 0.1f後にダメージフラグをオフにする
                Invoke(nameof(SetIsDamagedFalse), 0.1f);

                // ヒットカウントを増加させる
                _animator.SetInteger("HitCount", (_animator.GetInteger("HitCount") + 1));
            }
            
        }
    }

    /// <summary>引数で指定した時間だけ待機してアクションを呼ぶ</summary>
    /// <param name="waitTime">待ち時間</param>
    /// <param name="action">待機後に実行するアクション</param>
    IEnumerator Wait(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    public void SetIsDamagedTrue()
    {
        _animator.SetBool("IsDamaged", true);
    }
    public void SetIsDamagedFalse()
    {
        _animator.SetBool("IsDamaged", false);
    }

    public void ResetHitCount()
    {
        _animator.SetInteger("HitCount", 0);
    }

    public void WaitForSecondsToRiseUp()
    {
        StartCoroutine(Wait(2f, () => _animator.SetTrigger("RiseUp")));
    }
}
