using UnityEngine;

/// <summary>�G�t�F�N�g����</summary>
public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    /// <summary>�U���G�t�F�N�g</summary>
    [SerializeField] private ParticleSystem[] _attackEffects;

    /// <summary>�_���[�W�G�t�F�N�g</summary>
    [SerializeField] private ParticleSystem[] _damageEffects;

    /// <summary>�G�t�F�N�g�̐e</summary>
    [SerializeField] private Transform _effectParent;

    /// <summary>�����Ɏw�肵���ʒu�ɍU���G�t�F�N�g��\������</summary>
    /// <param name="pos">�G�t�F�N�g��\��������ʒu</param>
    /// <param name="index">�U���G�t�F�N�g�̎��ʎq</param>
    public void PlayAttackEffect(Vector3 pos, int index)
    {
        // �C���f�b�N�X���s�K�؂ł���ꍇ�͔�����
        if (index < 0 || index >= _attackEffects.Length)
        {
            Debug.Log($"This index ({index}) is invalid.");
            return;
        }
        Instantiate(_attackEffects[index], pos, Quaternion.identity, _effectParent);
    }

    /// <summary>�����Ɏw�肵���ʒu�Ƀ_���[�W�G�t�F�N�g��\������</summary>
    /// <param name="pos">�G�t�F�N�g��\��������ʒu</param>
    /// <param name="index">�_���[�W�G�t�F�N�g�̎��ʎq</param>
    public void PlayDamageEffect(Vector3 pos, int index)
    {
        // �C���f�b�N�X���s�K�؂ł���ꍇ�͔�����
        if (index < 0 || index >= _damageEffects.Length)
        {
            Debug.Log($"This index ({index}) is invalid.");
            return;
        }
        Instantiate(_damageEffects[index], pos, Quaternion.identity, _effectParent);
    }
}
