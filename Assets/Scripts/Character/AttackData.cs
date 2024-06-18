using UnityEngine;

/// <summary>�U���f�[�^</summary>
public class AttackData : MonoBehaviour
{
    /// <summary>�U����</summary>
    [SerializeField] private int _power;

    /// <summary>SE�̃C���f�b�N�X</summary>
    [SerializeField] private int _index;

    /// <summary>SE�̃{�����[��</summary>
    [SerializeField] private float _volume;

    /// <summary>�T�E���h�}�l�[�W���[</summary>
    CriSoundManager _soundManager;

    private void Start()
    {
        _soundManager = CriSoundManager.Instance;
    }

    /// <summary>�U���͂��擾����</summary>
    public int GetAttackDamage() => _power;

    /// <summary>�U���͂�ݒ肷��</summary>
    public void SetAttackDamage(int power) => _power = power > 0 ? power : 0;

    /// <summary>�v���C���[��SE���Đ�����</summary>
    private void PlayCharacterSound(int index)
    {
        var cueName = _soundManager._playerCueNames[index];
        CriSoundManager.Instance.Play("CueSheet_0", cueName, _volume);
    }

    /// <summary>�G��SE���Đ�����</summary>
    private void PlayEnemySound(int index)
    {
        var cueName = _soundManager._playerCueNames[index];
        CriSoundManager.Instance.Play("CueSheet_0", cueName, _volume);
    }

    private void OnTriggerEnter(Collider other)
    {
        // �G�̏ꍇ
        if (IsEnemy()) PlayEnemySound(_index);

        // �v���C���[�̏ꍇ
        else PlayCharacterSound(_index);
    }

    /// <summary>�G���ǂ���</summary>
    private bool IsEnemy() => gameObject.tag == "EnemyAttack" ? true : false;
}
