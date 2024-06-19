using UnityEngine;

/// <summary>�U���f�[�^</summary>
public class Attacker : MonoBehaviour
{
    /// <summary>�U����</summary>
    [SerializeField] private int _power;

    /// <summary>SE�̎��ʎq</summary>
    [SerializeField] private int _index;

    /// <summary>SE�̉���</summary>
    [SerializeField] private float _volume;

    /// <summary>�T�E���h</summary>
    CriSoundManager _soundManager;

    private void Start()
    {
        _soundManager = CriSoundManager.Instance;
    }

    //-------------------------------------------------------------------------------
    // �U���̃R�[���o�b�N�C�x���g
    //-------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        // �G
        if (IsEnemy()) PlayEnemySE(_index, _volume);

        // �v���C���[
        else PlayPlayerSE(_index, _volume);
    }

    //-------------------------------------------------------------------------------
    // �U���Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�U���͂��擾����</summary>
    public int GetAttackDamage() => _power;

    /// <summary>�U���͂�ݒ肷��</summary>
    public void SetAttackDamage(int power) => _power = power > 0 ? power : 0;

    /// <summary>�G���ǂ���</summary>
    private bool IsEnemy() => gameObject.tag == "EnemyAttack" ? true : false;

    //-------------------------------------------------------------------------------
    // SE�Ɋւ��鏈��
    //-------------------------------------------------------------------------------

    /// <summary>�v���C���[SE�̃L���[�����擾</summary>
    private string GetPlayerCueName(int index) => _soundManager._playerCueNames[index];

    /// <summary>�GSE�̃L���[�����擾</summary>
    private string GetEnemyCueName(int index) => _soundManager._playerCueNames[index];

    /// <summary>�v���C���[��SE���Đ�</summary>
    private void PlayPlayerSE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetPlayerCueName(index), volume);

    /// <summary>�G��SE���Đ�</summary>
    private void PlayEnemySE(int index, float volume)
        => CriSoundManager.Instance.Play("CueSheet_0", GetEnemyCueName(index), volume);
}
