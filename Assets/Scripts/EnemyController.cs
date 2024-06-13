using UnityEngine;

/// <summary>�G�̐���</summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>�A�j���[�^�[</summary>
    Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _animator.SetTrigger("Get Hit");
    }
}
