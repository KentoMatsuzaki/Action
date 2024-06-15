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
        // �v���C���[�ƐڐG���A�_���[�W���󂯂Ă���Œ��ł͂Ȃ��ꍇ
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
