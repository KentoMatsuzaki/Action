using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>UI���Ǘ�����N���X</summary>
public class UIManager : SingletonMonoBehaviour<UIManager>
{
    /// <summary>�̗̓o�[</summary>
    [SerializeField] private Image _hpBar;

    /// <summary>�o���l�o�[</summary>
    [SerializeField] private Image _xpBar;

    /// <summary>���x��</summary>
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

    /// <summary>�̗̓o�[�̒l��ݒ肷��</summary>
    public void SetHPBar(float hp)
    {
        _hpBar.DOFillAmount(hp, 1.0f).SetEase(Ease.Linear).SetEase(Ease.OutCubic);
    }

    /// <summary>�o���l�o�[�̒l��ݒ肷��</summary>
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

    /// <summary>�o���l�o�[�̒l���擾����</summary>
    /// <returns></returns>
    public float GetXPBar()
    {
        return _xpBar.fillAmount;
    }

    public void SetSushiText()
    {
        _sushiText.text = $"�܂����ʎ��i�̐��F{_sushiCount}";

        if(_sushiCount == 0)
        {
            _resultText.gameObject.SetActive(true);
            _resultText.text = "�Q�[���N���A�I";
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
