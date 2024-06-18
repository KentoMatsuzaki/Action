using UnityEngine;

/// <summary>エフェクト制御</summary>
public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    /// <summary>攻撃エフェクト</summary>
    [SerializeField] private ParticleSystem[] _attackEffects;

    /// <summary>ダメージエフェクト</summary>
    [SerializeField] private ParticleSystem[] _damageEffects;

    /// <summary>エフェクトの親</summary>
    [SerializeField] private Transform _effectParent;

    /// <summary>引数に指定した位置に攻撃エフェクトを表示する</summary>
    /// <param name="pos">エフェクトを表示させる位置</param>
    /// <param name="index">攻撃エフェクトの識別子</param>
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

    /// <summary>引数に指定した位置にダメージエフェクトを表示する</summary>
    /// <param name="pos">エフェクトを表示させる位置</param>
    /// <param name="index">ダメージエフェクトの識別子</param>
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
}
