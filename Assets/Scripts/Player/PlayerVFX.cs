using UnityEngine;
using DG.Tweening;

public class PlayerVFX : EntityVFX
{
    // テレポート
    [Header("Teleport VFX")]
    [SerializeField] private GameObject teleportVfx;
    [SerializeField] private Color teleportColor;

    [Header("Level Up VFX")]
    [SerializeField] private GameObject levelUpVfx;

    // アイテムバフ
    [Header("Sprite References")]
    [SerializeField] private SpriteRenderer mainSr;
    [SerializeField] private SpriteRenderer buffOverlaySr;

    [Header("Buff Overlay VFX")]
    [SerializeField] private float buffPulseDuration = 0.6f;
    [SerializeField, Range(0f, 1f)] private float buffMinAlpha = 0.15f;
    [SerializeField, Range(0f, 1f)] private float buffMaxAlpha = 0.45f;

    private Tween buffTween;

    protected override void Awake()
    {
        base.Awake();

        if (buffOverlaySr != null)
        {
            var c = buffOverlaySr.color;
            c.a = 0f;
            buffOverlaySr.color = c;
            buffOverlaySr.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if (mainSr == null || buffOverlaySr == null)
            return;

        if (!buffOverlaySr.gameObject.activeInHierarchy)
            return;

        // 本体アニメのスプライト形状に追従
        buffOverlaySr.sprite = mainSr.sprite;
        buffOverlaySr.flipX = mainSr.flipX;
        buffOverlaySr.flipY = mainSr.flipY;
    }


    public void CreateOnTeleportVfx(Transform target)
    {
        GameObject vfx = Instantiate(teleportVfx, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = teleportColor;

    }

    public void CreateOnLevelUpVfx(Transform target)
    {
        GameObject vfx = Instantiate(levelUpVfx, target.position, Quaternion.identity);
    }

    public void PlayBuffOverlay(Color color)
    {
        if (buffOverlaySr == null) return;

        buffOverlaySr.gameObject.SetActive(true);

        // まず色をセット（alphaは最大から開始）
        color.a = buffMaxAlpha;
        buffOverlaySr.color = color;

        buffTween?.Kill();
        buffTween = buffOverlaySr
            .DOFade(buffMinAlpha, buffPulseDuration)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopBuffOverlay()
    {
        if (buffOverlaySr == null)
            return;

        buffTween?.Kill();
        buffTween = null;

        // ふわっと消す
        buffOverlaySr.DOFade(0f, 0.15f).OnComplete(() =>
        {
            if (buffOverlaySr != null)
                buffOverlaySr.gameObject.SetActive(false);
        });
    }

}
