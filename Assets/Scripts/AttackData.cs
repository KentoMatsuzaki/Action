using UnityEngine;

/// <summary>攻撃データ</summary>
public class AttackData : MonoBehaviour
{
    /// <summary>攻撃ダメージ</summary>
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
