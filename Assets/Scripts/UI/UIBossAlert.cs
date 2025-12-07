using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBossAlert : MonoBehaviour
{
    [SerializeField] private GameObject rootBossAlert;
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private Image redScreen;
    [SerializeField] private float displayTime;
    [SerializeField] private float blinkTime = .5f;
    private bool isDisplayed;

    private Coroutine playingRoutine;

    private void Awake()
    {
        rootBossAlert.SetActive(false);
    }

    private void Start()
    {
        Play();
    }

    public void Play()
    {
        if (playingRoutine != null)
            return;

        alertText.text = "WARNING!!";
        playingRoutine = StartCoroutine(PlayAlertCo());
    }

    private IEnumerator PlayAlertCo()
    {
        isDisplayed = true;
        rootBossAlert.SetActive(true);

        float elapsed = 0f;

        while (elapsed < displayTime)
        {
            // ON/OFF トグル
            isDisplayed = !isDisplayed;
            rootBossAlert.SetActive(isDisplayed);

            yield return new WaitForSecondsRealtime(blinkTime);
            elapsed += blinkTime;
        }

        // 最終どうであれ、falseで締める
        isDisplayed = false;
        rootBossAlert.SetActive(false);

        playingRoutine = null;

    }

}
