using UnityEngine;

/// <summary>攻撃データ</summary>
public class Attacker : MonoBehaviour
{
    /// <summary>攻撃力</summary>
    [SerializeField] private int _power;

    /// <summary>SEの識別子</summary>
    [SerializeField] private int _index;

    /// <summary>SEの音量</summary>
    [SerializeField] private float _volume;

    /// <summary>サウンド</summary>
    CriSoundManager _soundManager;

    private void Start()
    {
        _soundManager = CriSoundManager.Instance;
    }

    //-------------------------------------------------------------------------------
    // 攻撃のコールバックイベント
    //-------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // 敵
        if (IsEnemy()) PlayEnemySE(_index, _volume);

        // プレイヤー
        else PlayPlayerSE(_index, _volume);
    }

    //-------------------------------------------------------------------------------
    // 攻撃に関する処理
    //-------------------------------------------------------------------------------

    /// <summary>攻撃力を取得する</summary>
    public int GetAttackDamage() => _power;

    /// <summary>攻撃力を設定する</summary>
    public void SetAttackDamage(int power) => _power = power > 0 ? power : 0;

    /// <summary>敵かどうか</summary>
    private bool IsEnemy() => gameObject.tag == "EnemyAttack" ? true : false;

    //-------------------------------------------------------------------------------
    // SEに関する処理
    //-------------------------------------------------------------------------------

    /// <summary>プレイヤーSEのキュー名を取得</summary>
    private string GetPlayerCueName(int index) => _soundManager._playerCueNames[index];

    /// <summary>敵SEのキュー名を取得</summary>
    private string GetEnemyCueName(int index) => _soundManager._playerCueNames[index];

    /// <summary>プレイヤーのSEを再生</summary>
    private void PlayPlayerSE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetPlayerCueName(index), volume);

    /// <summary>敵のSEを再生</summary>
    private void PlayEnemySE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetEnemyCueName(index), volume);
}
