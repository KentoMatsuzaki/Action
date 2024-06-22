using UnityEngine;

/// <summary>攻撃データ</summary>
public class Attacker : MonoBehaviour
{
    /// <summary>攻撃力</summary>
    [SerializeField] private int _power;

    /// <summary>エフェクトの識別子</summary>
    [SerializeField] public int _effectIndex;

    /// <summary>SEの識別子</summary>
    [SerializeField] private int _soundIndex;

    /// <summary>SEの音量</summary>
    [SerializeField] private float _volume;

    /// <summary>プレイヤー</summary>
    [SerializeField] private PlayerController _player;

    /// <summary>エフェクト</summary>
    EffectManager _effect;

    /// <summary>サウンド</summary>
    CriSoundManager _soundManager;

    /// <summary>攻撃力のプロパティ</summary>
    public int Power
    {
        get => _power;
        set => _power = value > 0 ? value : 0;
    }

    private void Start()
    {
        _effect = EffectManager.Instance;
        _soundManager = CriSoundManager.Instance;
    }

    //-------------------------------------------------------------------------------
    // 攻撃のコールバックイベント
    //-------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーが攻撃する場合の処理
        if (IsPlayer())
        {
            // SEを再生
            PlayPlayerSE(_soundIndex, _volume);

            if(_player._level >= 2)
            PlayAttackEffectOnAttackPos(_effectIndex);
        }

        // 敵が攻撃する場合の処理
        else
        {
            
        }
    }

    //-------------------------------------------------------------------------------
    // 攻撃に関する処理
    //-------------------------------------------------------------------------------

    /// <summary>プレイヤーかどうか</summary>
    private bool IsPlayer() => gameObject.tag == "PlayerAttack" ? true : false;

    /// <summary>自分の攻撃コライダーの位置を返す</summary>
    private Vector3 GetAttackPosition() => transform.position;

    /// <summary>攻撃した位置に攻撃エフェクトを表示する</summary>
    private void PlayAttackEffectOnAttackPos(int index) =>
        _effect.PlayAttackEffect(GetAttackPosition(), index);

    //-------------------------------------------------------------------------------
    // SEに関する処理
    //-------------------------------------------------------------------------------

    /// <summary>プレイヤーSEのキュー名を取得</summary>
    private string GetPlayerCueName(int index) => _soundManager._playerCueNames[index];

    /// <summary>敵SEのキュー名を取得</summary>
    private string GetEnemyCueName(int index) => _soundManager._enemyCueNames[index];

    /// <summary>プレイヤーのSEを再生</summary>
    private void PlayPlayerSE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetPlayerCueName(index), volume);

    /// <summary>敵のSEを再生</summary>
    private void PlayEnemySE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetEnemyCueName(index), volume);
}
