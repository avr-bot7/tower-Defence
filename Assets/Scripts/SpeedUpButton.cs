using TMPro;
using UnityEngine;

public class SpeedUpButton : MonoBehaviour
{
    public TextMeshProUGUI label;

    bool _fast;

    public void Toggle()
    {
        _fast = !_fast;
        Time.timeScale = _fast ? 2f : 1f;

        if (label)
            label.text = _fast ? ">> 2x" : "> 1x";
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
