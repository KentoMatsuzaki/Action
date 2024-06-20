using UnityEngine;
using Unity.TinyCharacterController.Control;

/// <summary>敵の制御</summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>アニメーター</summary>
    Animator _animator;

    /// <summary>攻撃コライダー</summary>
    [SerializeField, Header("攻撃コライダー")] Collider _attackCol;

    /// <summary>プレイヤー</summary>
    [SerializeField, Header("プレイヤーの位置")] Transform _player;

    /// <summary>体力</summary>
    [SerializeField, Header("敵のHP")] int _hp = 100;

    /// <summary>索敵距離</summary>
    [SerializeField, Header("索敵距離")] float _searchDistance;

    /// <summary>サウンド</summary>
    CriSoundManager _soundManager;

    /// <summary>最大HP</summary>
    int _maxHP;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _soundManager = CriSoundManager.Instance;
        _maxHP = _hp;
        Attack();
    }

    private void Update()
    {
        // プレイヤーの位置に向かって移動する
        Vector3 direction = _player.position - transform.position;

        // Y軸成分を無視する
        direction.y = 0f;

        // 方向ベクトルを正規化（長さを1にする）
        direction.Normalize();

        // プレイヤーの方向を向く
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }

        transform.position += transform.forward * 0.5f * Time.deltaTime;
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
                PlayDeadAnimation();

                // SEを再生
                PlayDeadSound();
            }

            // ダウンする場合
            else if (IsDown())
            {
                // ダウンアニメーションを再生
                PlayDownAnimation();

                // ダメージフラグをオンにする
                SetDamaged(true);

                // SEを再生
                PlayDamageSound();
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

                // SEを再生
                PlayDamageSound();
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
    private void PlayDeadAnimation() => _animator.Play("Died");

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

    /// <summary>攻撃イベント</summary>
    public void Attack()
    {
        if (CanAttack())
        {
            // アニメーションを再生
            PlayAttackAnimation();

            // SEを再生
            Invoke(nameof(PlayAttackSound), 0.1f);
        }
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
    // SEに関する処理
    //-------------------------------------------------------------------------------

    /// <summary>キュー名を取得</summary>
    private string GetCueName(int index) => _soundManager._enemyCueNames[index];

    /// <summary>SEを再生</summary>
    private void PlaySE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetCueName(index), volume);

    /// <summary>攻撃SEを再生</summary>
    private void PlayAttackSound() => PlaySE(0, 0.25f);

    /// <summary>ダメージSEを再生</summary>
    private void PlayDamageSound() => PlaySE(1, 1f);

    /// <summary>死亡SEを再生</summary>
    private void PlayDeadSound() => PlaySE(2, 0.5f);

    //-------------------------------------------------------------------------------
    // BehaviourTreeのノード
    //-------------------------------------------------------------------------------

    /// <summary>攻撃アクション</summary>
    public NodeStatus BTAttack()
    {
        // 既存のAttackメソッドを呼び出す
        Attack();

        // 一度の攻撃が完了したら成功を返す
        return NodeStatus.Success; 
    }

    /// <summary>追跡アクション</summary>
    public NodeStatus Chase(Transform player)
    {
        //MoveToPlayer(player);

        if (GetDistanceToPlayer() < 1.0f)
        {
            return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }

    /// <summary>プレイヤーが近くにいるか</summary>
    public bool IsPlayerClose() => GetDistanceToPlayer() <= _searchDistance;

    /// <summary>十分なHPがあるか</summary>
    public bool HasEnoughHP() => _hp >= _maxHP / 2;

    /// <summary>HPが不足しているか</summary>
    public bool DoesNotHaveEnoughHP() => _hp < _maxHP / 2;

    //-------------------------------------------------------------------------------
    // BehaviourTreeに関する処理
    //-------------------------------------------------------------------------------



    /// <summary>プレイヤーとの距離を算出する</summary>
    float GetDistanceToPlayer() =>
        Vector3.Distance(transform.position, _player.position);

    /// <summary>プレイヤーへの方向を算出する</summary>
    Vector3 GetDirectionToPlayer(Transform player)
        => player.position - transform.position;

    //private void MoveToPlayer(Transform player) =>
    //    (GetDirectionToPlayer(player).normalized * Time.deltaTime);
}
