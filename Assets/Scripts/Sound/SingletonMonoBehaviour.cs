using UnityEngine;

/// <summary>�V���O���g��</summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>�C���X�^���X</summary>
    private static T _instance;

    /// <summary>�v���p�e�B</summary>
    public static T Instance
    {
        get
        {
            // �C���X�^���X����̏ꍇ
            if (_instance == null)
            {
                // �K�؂ȃQ�[���I�u�W�F�N�g�����݂��Ȃ��ꍇ
                if (FindFirstObjectByType<T>() == null)
                {
                    // �K�؂ȃQ�[���I�u�W�F�N�g���쐬���ēo�^����
                    GameObject singleton = new GameObject();
                    singleton.name = typeof(T).ToString();
                    _instance = singleton.AddComponent<T>();

                    // �V�[���ύX���ɔj�����Ȃ��悤�ɂ���
                    DontDestroyOnLoad(singleton);
                }
                // �K�؂ȃI�u�W�F�N�g���V�[����ɑ��݂���ꍇ�A�����o�^����
                else _instance = FindFirstObjectByType<T>();
            }
            return _instance;
        }
        set => _instance = value;
    }
}
