using UnityEngine;

/// <summary>�U���f�[�^</summary>
public class Attacker : MonoBehaviour
{
    /// <summary>�U����</summary>
    [SerializeField] private int _power;

    /// <summary>�G�t�F�N�g�̎��ʎq</summary>
    [SerializeField] public int _effectIndex;

    /// <summary>SE�̎��ʎq</summary>
    [SerializeField] private int _soundIndex;

    /// <summary>SE�̉���</summary>
    [SerializeField] private float _volume;

    /// <summary>�v���C���[</summary>
    [SerializeField] private PlayerController _player;

    /// <summary>�G�t�F�N�g</summary>
    EffectManager _effect;

    /// <summary>�T�E���h</summary>
    CriSoundManager _soundManager;

    /// <summary>�U���͂̃v���p�e�B</summary>
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
    // �U���̃R�[���o�b�N�C�x���g
    //-------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // �v���C���[���U������ꍇ�̏���
        if (IsPlayer())
        {
            // SE���Đ�
            PlayPlayerSE(_soundIndex, _volume);

            if(_player._level >= 2)
            PlayAttackEffectOnAttackPos(_effectIndex);
        }

        // �G���U������ꍇ�̏���
        else
        {
            
        }
    }

    //-------------------------------------------------------------------------------
    // �U���Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�v���C���[���ǂ���</summary>
    private bool IsPlayer() => gameObject.tag == "PlayerAttack" ? true : false;

    /// <summary>�����̍U���R���C�_�[�̈ʒu��Ԃ�</summary>
    private Vector3 GetAttackPosition() => transform.position;

    /// <summary>�U�������ʒu�ɍU���G�t�F�N�g��\������</summary>
    private void PlayAttackEffectOnAttackPos(int index) =>
        _effect.PlayAttackEffect(GetAttackPosition(), index);

    //-------------------------------------------------------------------------------
    // SE�Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�v���C���[SE�̃L���[�����擾</summary>
    private string GetPlayerCueName(int index) => _soundManager._playerCueNames[index];

    /// <summary>�GSE�̃L���[�����擾</summary>
    private string GetEnemyCueName(int index) => _soundManager._enemyCueNames[index];

    /// <summary>�v���C���[��SE���Đ�</summary>
    private void PlayPlayerSE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetPlayerCueName(index), volume);

    /// <summary>�G��SE���Đ�</summary>
    private void PlayEnemySE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetEnemyCueName(index), volume);
}
