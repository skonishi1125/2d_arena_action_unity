using DG.Tweening;
using UnityEngine;

public class ItemFloatMotion : MonoBehaviour
{
    [Header("Spawn Hop")]
    [SerializeField] private float hopHeight = 0.4f;
    [SerializeField] private float hopDuration = 0.25f;
    [SerializeField] private float randomX = 0.4f;

    [Header("Hover")]
    [SerializeField] private float hoverAmplitude = 0.12f;
    [SerializeField] private float hoverHalfPeriod = 0.45f;

    private Tween jumpTween;
    private Tween hoverTween;
    private Vector3 anchorPos;

    private void OnEnable()
    {
        Play();
    }

    public void Play()
    {
        // 多重起動防止
        jumpTween?.Kill();
        hoverTween?.Kill();

        var start = transform.position;
        anchorPos = start + new Vector3(Random.Range(-randomX, randomX), 0f, 0f);

        // ぴょん（1回）
        jumpTween = transform
            .DOJump(anchorPos, hopHeight, 1, hopDuration)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        // ふわふわ（ループ）
        jumpTween.OnComplete(() =>
        {
            hoverTween = transform
                .DOMoveY(anchorPos.y + hoverAmplitude, hoverHalfPeriod)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        });
    }

}
