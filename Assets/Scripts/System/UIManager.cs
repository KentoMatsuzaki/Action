using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>UIを管理するクラス</summary>
public class UIManager : SingletonMonoBehaviour<UIManager>
{
    /// <summary>体力バー</summary>
    [SerializeField] private Image _hpBar;

    /// <summary>経験値バー</summary>
    [SerializeField] private Image _xpBar;

    /// <summary>レベル</summary>
    [SerializeField] private Text _level;

    [SerializeField] private PlayerController _player;

    [SerializeField] private TextMeshProUGUI _sushiText;

    [SerializeField] private TextMeshProUGUI _sushiCommandText;

    [SerializeField] public TextMeshProUGUI _resultText;

    int _currentLevel = 1;

    public int _sushiCount = 3;

    private void Start()
    {
        DOTween.Init();
        SetSushiText();
        Invoke(nameof(ResetSushiCommandText), 2.0f);
    }

    /// <summary>体力バーの値を設定する</summary>
    public void SetHPBar(float hp)
    {
        _hpBar.DOFillAmount(hp, 1.0f).SetEase(Ease.Linear).SetEase(Ease.OutCubic);
    }

    /// <summary>経験値バーの値を設定する</summary>
    public void SetXpBar(float xp)
    {
        _xpBar.DOFillAmount(xp, 1.0f).SetEase(Ease.Linear).SetEase(Ease.OutCubic);
        Invoke(nameof(CallLevelUp), 1.1f);
    }

    private void CallLevelUp()
    {
        if (_xpBar.fillAmount == 1)
        {
            _player.LevelUp();
            _currentLevel++;
            _level.text = _currentLevel.ToString();
            _xpBar.fillAmount = 0;
        }
    }

    /// <summary>経験値バーの値を取得する</summary>
    /// <returns></returns>
    public float GetXPBar()
    {
        return _xpBar.fillAmount;
    }

    public void SetSushiText()
    {
        _sushiText.text = $"まだ見ぬ寿司の数：{_sushiCount}";

        if(_sushiCount == 0)
        {
            _resultText.gameObject.SetActive(true);
            _resultText.text = "ゲームクリア！";
            Invoke(nameof(ResetScene), 3.0f);
        }
    }

    void ResetSushiCommandText()
    {
        _sushiCommandText.text = "";
    }

    void ResetScene()
    {
        SceneManager.LoadScene("BattleScene");
    }
}
