using UnityEngine;
using UnityEngine.UI;

/// <summary>UI���Ǘ�����N���X</summary>
public class UIManager : SingletonMonoBehaviour<UIManager>
{
    /// <summary>�̗̓o�[</summary>
    [SerializeField] private Image _hpBar;

    /// <summary>�o���l�o�[</summary>
    [SerializeField] private Image _XpBar;

    /// <summary>���x��</summary>
    [SerializeField] private Text _level;

    [SerializeField] private PlayerController _player;

    int _currentLevel = 1;

    GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameManager.Instance;
    }

    /// <summary>�̗̓o�[�̒l��ݒ肷��</summary>
    public void SetHPBar(float hp)
    {
        _hpBar.fillAmount = hp;
    }

    /// <summary>�o���l�o�[�̒l��ݒ肷��</summary>
    public void SetXpBar(float xp)
    {
        if(xp == 1)
        {
            _player.LevelUp();
            _currentLevel++;
            _level.text = _currentLevel.ToString();
            _XpBar.fillAmount = 0;
        }
        else
        {
            _XpBar.fillAmount = xp;
        }
    }

    /// <summary>�o���l�o�[�̒l���擾����</summary>
    /// <returns></returns>
    public float GetXPBar()
    {
        return _XpBar.fillAmount;
    }
}
