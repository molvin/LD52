using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    public float fadeTime;
    public IEnumerator FadeAlpha(float targetAlpha)
    {
        float time = 0;
        Image im = GetComponent<Image>();
        Color start = new Color(0f, 0f, 0f, 1f-targetAlpha);
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float factor = time / fadeTime;
            im.color = Color.Lerp(start, new Color(0f, 0f, 0f, targetAlpha), factor);
            yield return null;
        }
        yield return null;
    }
}
