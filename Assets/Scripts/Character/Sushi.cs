using UnityEngine;

public class Sushi : MonoBehaviour
{
    [SerializeField] UIManager _ui;
    [SerializeField] GameObject[] effect;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            _ui._sushiCount--;
            _ui.SetSushiText();
            CriSoundManager.Instance.Play("cueSheet_0", "ŽõŽi", 1.0f);
            foreach(GameObject obj in effect)
            {
                obj.SetActive(true);
            }
            Invoke(nameof(DisableSelf), 0.2f);
        }
    }

    void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    void DisableAllEff()
    {
        foreach (GameObject obj in effect)
        {
            obj.SetActive(false);
        }
    }
}
