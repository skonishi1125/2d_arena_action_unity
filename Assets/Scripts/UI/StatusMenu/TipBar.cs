using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TipBar : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private Button closeButton;

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 0.25f;

    [Header("Dismiss")]
    [SerializeField] private bool disableAfterDismissInThisScene = true;
    private static int cachedSceneHandle = -1;
    private static bool dismissedInThisScene = false;

    Coroutine routine;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (tipText == null)
            tipText = GetComponentInChildren<TextMeshProUGUI>(true);

        ResetDismissIfSceneChanged();
        if (closeButton != null)
            closeButton.onClick.AddListener(OnCloseClicked);

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(OnCloseClicked);
    }

    private static void ResetDismissIfSceneChanged()
    {
        int handle = SceneManager.GetActiveScene().handle;
        if (cachedSceneHandle != handle)
        {
            cachedSceneHandle = handle;
            dismissedInThisScene = false;
        }
    }

    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        tipText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Show(float autoHideSeconds = -1f)
    {
        ResetDismissIfSceneChanged();
        if (disableAfterDismissInThisScene && dismissedInThisScene)
            return;

        gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;

        if (routine != null)
            StopCoroutine(routine);
        routine = StartCoroutine(ShowRoutine(autoHideSeconds));
    }

    public void Hide()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FadeOutAndDisable());
    }

    private void OnCloseClicked()
    {
        ResetDismissIfSceneChanged();
        dismissedInThisScene = true;
        Hide();
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
