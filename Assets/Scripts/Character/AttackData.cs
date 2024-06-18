using UnityEngine;

/// <summary>攻撃データ</summary>
public class AttackData : MonoBehaviour
{
    /// <summary>攻撃力</summary>
    [SerializeField] private int _power;

    /// <summary>SEのインデックス</summary>
    [SerializeField] private int _index;

    /// <summary>SEのボリューム</summary>
    [SerializeField] private float _volume;

    /// <summary>サウンドマネージャー</summary>
    CriSoundManager _soundManager;

    private void Start()
    {
        _soundManager = CriSoundManager.Instance;
    }

    /// <summary>攻撃力を取得する</summary>
    public int GetAttackDamage() => _power;

    /// <summary>攻撃力を設定する</summary>
    public void SetAttackDamage(int power) => _power = power > 0 ? power : 0;

    /// <summary>プレイヤーのSEを再生する</summary>
    private void PlayCharacterSound(int index)
    {
        var cueName = _soundManager._playerCueNames[index];
        CriSoundManager.Instance.Play("CueSheet_0", cueName, _volume);
    }

    /// <summary>敵のSEを再生する</summary>
    private void PlayEnemySound(int index)
    {
        var cueName = _soundManager._playerCueNames[index];
        CriSoundManager.Instance.Play("CueSheet_0", cueName, _volume);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 敵の場合
        if (IsEnemy()) PlayEnemySound(_index);

        // プレイヤーの場合
        else PlayCharacterSound(_index);
    }

    /// <summary>敵かどうか</summary>
    private bool IsEnemy() => gameObject.tag == "EnemyAttack" ? true : false;
}
