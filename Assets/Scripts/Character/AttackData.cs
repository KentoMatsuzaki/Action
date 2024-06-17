using UnityEngine;

/// <summary>�U���f�[�^</summary>
public class AttackData : MonoBehaviour
{
    /// <summary>�U���_���[�W</summary>
    [SerializeField] private int _attackDamage;

    /// <summary>�U���͂��擾����</summary>
    public int GetAttackDamage()
    {
        return _attackDamage;
    }

    /// <summary>�U���͂�ݒ肷��</summary>
    /// <param name="damage">�V�����U����</param>
    public void SetAttackDamage(int damage)
    {
        // �U���͂��}�C�i�X�̏ꍇ�͔�����
        if (damage < 0)
        {
            Debug.Log("Warning : Attack Damage must be positive number.");
            return;
        }

        // �U���͂��X�V
        else _attackDamage = damage;
    }
}
