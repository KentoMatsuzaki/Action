using UnityEngine;

/// <summary>エフェクト制御</summary>
public class EffectController : MonoBehaviour 
{
    /// <summary>インスタンス</summary>
    public static EffectController Instance { get; private set; }

    /// <summary>攻撃エフェクト</summary>
    [SerializeField] private ParticleSystem[] _attackEffects;

    /// <summary>ダメージエフェクト</summary>
    [SerializeField] private ParticleSystem[] _damageEffects;

    /// <summary>死亡エフェクト</summary>
    [SerializeField] private ParticleSystem[] _deadEffects;

    /// <summary>エフェクトの親</summary>
    [SerializeField] private Transform _effectParent;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>座標を指定して攻撃エフェクトを表示する</summary>
    /// <param name="pos">エフェクトを表示させる座標</param>
    /// <param name="index">エフェクトのインデックス</param>
    public void PlayAttackEffect(Vector3 pos, int index)
    {
        // インデックスが不適切である場合は抜ける
        if (index < 0 || index >= _attackEffects.Length)
        {
            Debug.Log($"This index ({index}) is invalid.");
            return;
        }
        Instantiate(_attackEffects[index], pos, Quaternion.identity, _effectParent);
    }

    /// <summary>座標を指定してダメージエフェクトを表示する</summary>
    /// <param name="pos">エフェクトを表示させる座標</param>
    /// <param name="index">エフェクトのインデックス</param>
    public void PlayDamageEffect(Vector3 pos, int index)
    {
        // インデックスが不適切である場合は抜ける
        if (index < 0 || index >= _damageEffects.Length)
        {
            Debug.Log($"This index ({index}) is invalid.");
            return;
        }
        Instantiate(_damageEffects[index], pos, Quaternion.identity, _effectParent);
    }

    /// <summary>座標を指定して死亡エフェクトを表示する</summary>
    /// <param name="pos">エフェクトを表示させる座標</param>
    /// <param name="index">エフェクトのインデックス</param>
    public void PlayDeadEffect(Vector3 pos, int index)
    {
        // インデックスが不適切である場合は抜ける
        if (index < 0 || index >= _deadEffects.Length)
        {
            Debug.Log($"This index ({index}) is invalid.");
            return;
        }
        Instantiate(_deadEffects[index], pos, Quaternion.identity, _effectParent);
    }
}
