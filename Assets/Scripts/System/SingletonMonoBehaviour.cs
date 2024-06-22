using UnityEngine;

/// <summary>シングルトン</summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>インスタンス</summary>
    private static T _instance;

    /// <summary>プロパティ</summary>
    public static T Instance
    {
        get
        {
            // インスタンスが空の場合
            if (_instance == null)
            {
                // 適切なゲームオブジェクトが存在しない場合
                if (FindFirstObjectByType<T>() == null)
                {
                    // 適切なゲームオブジェクトを作成して登録する
                    GameObject singleton = new GameObject();
                    singleton.name = typeof(T).ToString();
                    _instance = singleton.AddComponent<T>();

                    // シーン変更時に破棄しないようにする
                    DontDestroyOnLoad(singleton);
                }
                // 適切なオブジェクトがシーン上に存在する場合、それを登録する
                else _instance = FindFirstObjectByType<T>();
            }
            return _instance;
        }
        set => _instance = value;
    }
}
