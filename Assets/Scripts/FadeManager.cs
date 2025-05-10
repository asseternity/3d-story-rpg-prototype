using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public Image blackScreen; // Assign in Inspector (full-screen black UI Image)
    public float fadeDuration = 1f;
    public float waitDuration = 2f;

    public IEnumerator FadeScreen(UnityAction callback)
    {
        // activate panel
        blackScreen.gameObject.SetActive(true);

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        // Wait
        yield return new WaitForSeconds(waitDuration);

        // activate callback
        callback();

        // Fade back in
        yield return StartCoroutine(Fade(1f, 0f));

        // deactivate panel
        blackScreen.gameObject.SetActive(false);
    }

    public IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = blackScreen.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            blackScreen.color = color;
            yield return null;
        }

        color.a = endAlpha;
        blackScreen.color = color;
    }
}
