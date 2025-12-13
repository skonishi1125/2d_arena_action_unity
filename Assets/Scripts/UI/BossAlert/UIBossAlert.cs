using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIBossAlert : MonoBehaviour
{
    [Header("Text Setting")]
    [SerializeField] private GameObject rootBossAlert;
    [SerializeField] private TextMeshProUGUI alertText;
    [SerializeField] private float displayTime;
    [SerializeField] private float blinkTime = .5f;
    private bool isDisplayed;

    [Header("Red Screen Setting")]
    [SerializeField] private Image redScreen;
    [SerializeField] private float maxAlpha = .4f;
    [SerializeField] private float minAlpha = 0f;
    [SerializeField] private float performTime;
    [SerializeField] private int loopCount = 3;

    private Coroutine playingRoutine;

    private void Awake()
    {
        rootBossAlert.SetActive(false);
    }

    public void Play()
    {
        if (playingRoutine != null)
            return;

        alertText.text = "WARNING!!";
        playingRoutine = StartCoroutine(PlayAlertCo());
    }

    // 簡単な処理なので、試しにDOTweenを使わず書いてみる
    private IEnumerator PlayAlertCo()
    {
        PlayRedFlash();

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

    private void PlayRedFlash()
    {
        redScreen.gameObject.SetActive(true);

        // alphaを0にしておく
        redScreen.color = new Color(
            redScreen.color.r,
            redScreen.color.g,
            redScreen.color.b,
            0f
        );

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true); // TimeScale 影響なし（スロー中でもOK）

        seq.Append(redScreen.DOFade(maxAlpha, performTime))  // フェードイン
           .Append(redScreen.DOFade(minAlpha, performTime));  // フェードアウト

        // 演出を指定の回数繰り返す
        seq.SetLoops(loopCount, LoopType.Restart);

        seq.OnComplete(() =>
        {
            redScreen.gameObject.SetActive(false);  // 完全に終わったら非表示
        });

    }


}
