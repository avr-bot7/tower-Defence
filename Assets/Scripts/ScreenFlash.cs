using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlash : MonoBehaviour
{
    public static ScreenFlash I;

    Image _image;

    void Awake()
    {
        I = this;
        _image = GetComponent<Image>();
    }

    public void Flash(Color color, float duration = 0.3f)
    {
        StopAllCoroutines();
        StartCoroutine(DoFlash(color, duration));
    }

    IEnumerator DoFlash(Color color, float duration)
    {
        if (_image == null) yield break;

        color.a = 0.6f;
        _image.color = color;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0.6f, 0f, t / duration);
            _image.color = color;
            yield return null;
        }

        color.a = 0f;
        _image.color = color;
    }
}
