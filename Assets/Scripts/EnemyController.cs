using UnityEngine;
using System;
using System.Collections;

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
        // �v���C���[�ƐڐG�����ꍇ
        if (other.gameObject.tag == "Player")
        {
            if(_animator.GetInteger("HitCount") == 4)
            {
                _animator.Play("Get Down");
            }
            if(_animator.GetBool("IsDamaged") == false)
            {
                // �_���[�W�t���O���I���ɂ���
                SetIsDamagedTrue();

                // 0.1f��Ƀ_���[�W�t���O���I�t�ɂ���
                Invoke(nameof(SetIsDamagedFalse), 0.1f);

                // �q�b�g�J�E���g�𑝉�������
                _animator.SetInteger("HitCount", (_animator.GetInteger("HitCount") + 1));
            }
            
        }
    }

    /// <summary>�����Ŏw�肵�����Ԃ����ҋ@���ăA�N�V�������Ă�</summary>
    /// <param name="waitTime">�҂�����</param>
    /// <param name="action">�ҋ@��Ɏ��s����A�N�V����</param>
    IEnumerator Wait(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action?.Invoke();
    }

    public void SetIsDamagedTrue()
    {
        _animator.SetBool("IsDamaged", true);
    }
    public void SetIsDamagedFalse()
    {
        _animator.SetBool("IsDamaged", false);
    }

    public void ResetHitCount()
    {
        _animator.SetInteger("HitCount", 0);
    }

    public void WaitForSecondsToRiseUp()
    {
        StartCoroutine(Wait(2f, () => _animator.SetTrigger("RiseUp")));
    }
}
