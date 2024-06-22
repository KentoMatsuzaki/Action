using UnityEngine;
using UnityEngine.UI;

/// <summary>UIを管理するクラス</summary>
public class UIManager : SingletonMonoBehaviour<UIManager>
{
    /// <summary>体力バー</summary>
    [SerializeField] private Image _hpBar;

    /// <summary>経験値バー</summary>
    [SerializeField] private Image _XpBar;

    /// <summary>レベル</summary>
    [SerializeField] private Text _level;

    GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    /// <summary>体力バーの値を設定する</summary>
    public void SetHPBar(float hp)
    {
        _hpBar.fillAmount = hp;
    }

    /// <summary>経験値バーの値を設定する</summary>
    public void SetXpBar(float xp)
    {
        if(xp == 1)
        {
            _gameManager.OnLevelUp();
            _XpBar.fillAmount = 0;
        }
        _XpBar.fillAmount = xp;
    }

    /// <summary>経験値バーの値を取得する</summary>
    /// <returns></returns>
    public float GetXPBar()
    {
        return _XpBar.fillAmount;
    }
}
