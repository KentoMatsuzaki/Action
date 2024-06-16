using UnityEngine;

/// <summary>�G�t�F�N�g����</summary>
public class EffectController : MonoBehaviour 
{
    /// <summary>�C���X�^���X</summary>
    public static EffectController Instance { get; private set; }

    /// <summary>�U���G�t�F�N�g</summary>
    [SerializeField] private ParticleSystem[] _attackEffects;

    /// <summary>�_���[�W�G�t�F�N�g</summary>
    [SerializeField] private ParticleSystem[] _damageEffects;

    /// <summary>���S�G�t�F�N�g</summary>
    [SerializeField] private ParticleSystem[] _deadEffects;

    /// <summary>�G�t�F�N�g�̐e</summary>
    [SerializeField] private Transform _effectParent;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>���W���w�肵�čU���G�t�F�N�g��\������</summary>
    /// <param name="pos">�G�t�F�N�g��\����������W</param>
    /// <param name="index">�G�t�F�N�g�̃C���f�b�N�X</param>
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

    /// <summary>���W���w�肵�ă_���[�W�G�t�F�N�g��\������</summary>
    /// <param name="pos">�G�t�F�N�g��\����������W</param>
    /// <param name="index">�G�t�F�N�g�̃C���f�b�N�X</param>
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

    /// <summary>���W���w�肵�Ď��S�G�t�F�N�g��\������</summary>
    /// <param name="pos">�G�t�F�N�g��\����������W</param>
    /// <param name="index">�G�t�F�N�g�̃C���f�b�N�X</param>
    public void PlayDeadEffect(Vector3 pos, int index)
    {
        // �C���f�b�N�X���s�K�؂ł���ꍇ�͔�����
        if (index < 0 || index >= _deadEffects.Length)
        {
            Debug.Log($"This index ({index}) is invalid.");
            return;
        }
        Instantiate(_deadEffects[index], pos, Quaternion.identity, _effectParent);
    }
}
