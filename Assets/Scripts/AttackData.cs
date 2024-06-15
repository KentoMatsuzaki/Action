using UnityEngine;

/// <summary>攻撃データ</summary>
public class AttackData : MonoBehaviour
{
    /// <summary>攻撃ダメージ</summary>
    [SerializeField] private int _attackDamage;

    /// <summary>攻撃力を取得する</summary>
    public int GetAttackDamage()
    {
        return _attackDamage;
    }

    /// <summary>攻撃力を設定する</summary>
    /// <param name="damage">新しい攻撃力</param>
    public void SetAttackDamage(int damage)
    {
        // 攻撃力がマイナスの場合は抜ける
        if (damage < 0)
        {
            Debug.Log("Warning : Attack Damage must be positive number.");
            return;
        }

        // 攻撃力を更新
        else _attackDamage = damage;
    }
}
