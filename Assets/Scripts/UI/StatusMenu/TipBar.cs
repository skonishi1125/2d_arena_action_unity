using System.Collections;
using TMPro;
using UnityEngine;

public class TipBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI tipText;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.25f;

    Coroutine routine;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (tipText == null)
            tipText = GetComponentInChildren<TextMeshProUGUI>(true);

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        tipText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Show(string message, float autoHideSeconds = -1f)
    {
        tipText.text = message;
        gameObject.SetActive(true);

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShowRoutine(autoHideSeconds));
    }

    public void Hide()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FadeOutAndDisable());
    }

    IEnumerator ShowRoutine(float autoHideSeconds)
    {
        yield return FadeTo(1f);

        if (autoHideSeconds > 0f)
        {
            yield return new WaitForSecondsRealtime(autoHideSeconds);
            yield return FadeOutAndDisable();
        }
    }

    IEnumerator FadeOutAndDisable()
    {
        yield return FadeTo(0f);
        gameObject.SetActive(false);
    }

    IEnumerator FadeTo(float target)
    {
        float start = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime; // メニュー中に止めてもOK
            canvasGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = target;
    }

}
