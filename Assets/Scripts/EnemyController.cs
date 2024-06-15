using UnityEngine;

/// <summary>敵の制御</summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>アニメーター</summary>
    Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーと接触し、ダメージを受けている最中ではない場合
        if (other.gameObject.tag == "Player" && _animator.GetBool("IsDamaged") == false)
        {
            Debug.Log("a");
            SetIsDamagedTrue();
        }
    }

    public void SetIsDamagedTrue()
    {
        _animator.SetBool("IsDamaged", true);
    }
    public void SetIsDamagedFalse()
    {
        _animator.SetBool("IsDamaged", false);
    }
}
