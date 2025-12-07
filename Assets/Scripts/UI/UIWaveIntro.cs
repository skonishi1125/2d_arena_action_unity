using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIWaveIntro : MonoBehaviour
{
    [SerializeField] private GameObject rootWaveIntro;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int countdownTime;

    public event Action OnFinished;

    private void Awake()
    {
        rootWaveIntro.SetActive(false);
    }

    public void Play()
    {
        rootWaveIntro.SetActive(true);
        StartCoroutine(PlayCountdownCo());
    }

    private IEnumerator PlayCountdownCo()
    {
        for (int i = countdownTime; i >= 0; i--)
        {
            if (i == 0)
                countdownText.text = "GO!";
            else
                countdownText.text = i.ToString();

            yield return new WaitForSecondsRealtime(1f);
        }
        rootWaveIntro.SetActive(false);
        OnFinished?.Invoke();
    }


}
