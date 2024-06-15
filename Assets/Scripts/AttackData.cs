using UnityEngine;

/// <summary>�U���f�[�^</summary>
public class AttackData : MonoBehaviour
{
    /// <summary>�U���_���[�W</summary>
    [SerializeField] private int _attackDamage;

    public int AttackDamage
    {
        get => _attackDamage;
        set
        {
            if (value < 0) return;
            else _attackDamage = value;
        }
    }
}
