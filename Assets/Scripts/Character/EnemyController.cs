using UnityEngine;

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

    /// <summary>サウンド</summary>
    CriSoundManager _soundManager;

    /// <summary>UI</summary>
    GameManager _gameManager;

    /// <summary>最大HP</summary>
    int _maxHP;

    /// <summary>ダメージフラグ</summary>
    private bool _isDamaged = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _soundManager = CriSoundManager.Instance;
        _gameManager = GameManager.Instance;
        _maxHP = _hp;
        //_point = GetRandomPatrolPoint(_range);
    }

    private void Update()
    {
        //BTFlee(_player, previousPosition, 0.5f, _fleeRange);
        //BTChase(_player, previousPosition, 0.5f, _chaseRange);
        //Patrol(_point, previousPosition, 0.5f, _range);
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

                // 経験値バーを更新
                CallSetXPBar();

                _isDamaged = true;

                // 死亡エフェクトを再生
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

                _isDamaged = true;
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

                _isDamaged = true;
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
    public bool IsDead() => _hp <= 0;

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

    public void DamageCoolDown()
    {
        Invoke(nameof(UnsetDamageFlag), 1.0f);
    }
    public void UnsetDamageFlag()
    {
        _isDamaged = false;
    }

    private void CallSetXPBar()
    {
        _gameManager.SetXPBar();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void InvokeDestroy()
    {
        Invoke(nameof(DestroySelf), 1.5f);
    }

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
    public void BTAttack()
    {
        // 既存のAttackメソッドを呼び出す
        Attack();
    }

    /// <summary>追跡アクション</summary>
    public NodeStatus BTChase(Transform player, Vector3 previous, float speed, float chaseRange)
    {
        if (GetDistanceToPlayer(player) >= chaseRange)
        {
            // プレイヤーの方を向く
            RotateTowardsPlayer(player);

            // 前方に移動する
            MoveForward(speed);

            // 移動量を速度に変換して設定する
            SetSpeedParameter(GetSpeed(GetPositionDiff(previous)));

            // 座標を更新する
            previous = transform.position;
        }
        else
        {
            // 速度を0にする
            SetSpeedParameter(0);
        }

        if (GetDistanceToPlayer(player) < chaseRange)
        {
            return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }

    /// <summary>逃走アクション</summary>
    public NodeStatus BTFlee(Transform player, Vector3 previous, float speed, float fleeRange)
    {
        if (GetDistanceToPlayer(player) <= fleeRange)
        {
            // プレイヤーの方を向く
            RotateAwayFromPlayer(player);

            // 前方に移動する
            MoveForward(speed);

            // 移動量を速度に変換して設定する
            SetSpeedParameter(GetSpeed(GetPositionDiff(previous)));

            // 座標を更新する
            previous = transform.position;
        }
        else
        {
            // 速度を0にする
            SetSpeedParameter(0);
        }

        if (GetDistanceToPlayer(player) > fleeRange)
        {
            return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }

    /// <summary>巡回アクション</summary>
    public NodeStatus BTPatrol(Transform point, Vector3 previous, float speed, float range)
    {
        // 目的地に着いた場合
        if(GetDistanceToPoint(point) < 0.5f)
        {
            // 目的地を更新
            point = GetRandomPatrolPoint(range);
            return NodeStatus.Success;
        }

        // 巡回処理

        // プレイヤーの方を向く
        RotateTowardsPoint(point);

        // 前方に移動する
        MoveForward(speed);

        // 移動量を速度に変換して設定する
        SetSpeedParameter(GetSpeed(GetPositionDiff(previous)));

        // 座標を更新する
        previous = transform.position;

        return NodeStatus.Running;
    }

    /// <summary>プレイヤーが近くにいるか</summary>
    public bool IsPlayerClose(Transform player, float range) 
        => GetDistanceToPlayer(player) <= range;

    /// <summary>プレイヤーが遠くにいるか</summary>
    public bool IsPlayerAway(Transform player, float range) 
        => GetDistanceToPlayer(player) > range;

    /// <summary>十分な体力があるか</summary>
    public bool HasEnoughHP() => _hp >= _maxHP / 2;

    /// <summary>体力が不足しているか</summary>
    public bool DoesNotHaveEnoughHP() => _hp < _maxHP / 2;

    /// <summary>ダメージを受けているか</summary>
    public bool IsNotDamaged() => !_isDamaged;

    //-------------------------------------------------------------------------------
    // BehaviourTreeに関する処理
    //-------------------------------------------------------------------------------

    /// <summary>プレイヤーとの距離を算出する</summary>
    private float GetDistanceToPlayer(Transform player) =>
        Vector3.Distance(transform.position, player.position);

    /// <summary>プレイヤーへの方向を算出する</summary>
    private Vector3 GetDirectionToPlayer(Transform player)
        => player.position - transform.position;
 
    /// <summary>プレイヤーへの最適化された方向を算出する</summary>
    private Vector3 GetOptimizedDirectionToPlayer(Transform player) 
        => new Vector3(GetDirectionToPoint(player).x, 0f, GetDirectionToPoint(player).z).normalized;

    /// <summary>プレイヤーへの回転方向を取得する</summary>
    private Quaternion GetRotationToPlayer(Vector3 dir) => Quaternion.LookRotation(dir);

    /// <summary>プレイヤーの方を向く</summary>
    private void RotateTowardsPlayer(Transform player) => 
        transform.rotation = Quaternion.Slerp(transform.rotation, 
            GetRotationToPlayer(GetOptimizedDirectionToPlayer(player)), Time.deltaTime * 10f);

    /// <summary>プレイヤーの逆を向く</summary>
    private void RotateAwayFromPlayer(Transform player) =>
        transform.rotation = Quaternion.Slerp(transform.rotation,
            GetRotationToPlayer(-GetOptimizedDirectionToPlayer(player)), Time.deltaTime * 10f);

    /// <summary>前方に移動する</summary>
    private void MoveForward(float moveSpeed) => 
        transform.position += transform.forward * moveSpeed * Time.deltaTime;

    /// <summary>座標の差分を求める</summary>
    private Vector3 GetPositionDiff(Vector3 previous) => transform.position - previous;

    /// <summary>座標の差分から移動速度を求める</summary>
    private float GetSpeed(Vector3 diff) => diff.magnitude;

    /// <summary>移動速度を設定する</summary>
    private void SetSpeedParameter(float speed) => _animator.SetFloat("Speed", speed);

    /// <summary>ランダムな巡回地点を取得する</summary>
    public Transform GetRandomPatrolPoint(float range)
    {
        float randomX = Random.Range(-range, range);
        float randomZ = Random.Range(-range, range);
        var randomPos = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(GameObject.Find("PatrolTarget") == null)
        {
            GameObject targetObject = new GameObject("PatrolTarget");
            targetObject.transform.position = randomPos;
            return targetObject.transform;
        }
        else
        {
            var target = GameObject.Find("PatrolTarget");
            target.transform.position = randomPos;
            return target.transform;
        }
    }

    /// <summary>巡回地点までの距離を算出する</summary>
    private float GetDistanceToPatrolPoint(Transform point) =>
        Vector3.Distance(transform.position, _player.position);

    /// <summary>ポイントとの距離を算出する</summary>
    private float GetDistanceToPoint(Transform point) =>
        Vector3.Distance(transform.position, point.position);

    /// <summary>ポイントへの方向を算出する</summary>
    private Vector3 GetDirectionToPoint(Transform point)
        => point.position - transform.position;

    /// <summary>ポイントへの最適化された方向を算出する</summary>
    private Vector3 GetOptimizedDirectionToPoint(Transform point)
        => new Vector3(GetDirectionToPoint(point).x, 0f, GetDirectionToPoint(point).z).normalized;

    /// <summary>ポイントへの回転方向を取得する</summary>
    private Quaternion GetRotationToPoint(Vector3 dir) => Quaternion.LookRotation(dir);

    /// <summary>ポイントの方を向く</summary>
    private void RotateTowardsPoint(Transform point) =>
        transform.rotation = Quaternion.Slerp(transform.rotation,
            GetRotationToPoint(GetOptimizedDirectionToPoint(point)), Time.deltaTime * 10f);
}
