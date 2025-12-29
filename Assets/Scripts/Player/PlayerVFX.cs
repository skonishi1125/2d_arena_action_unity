using UnityEngine;
using DG.Tweening;
using TMPro; // 追加

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

    // アイテム取得文字
    [Header("Item VFX")]
    [SerializeField] private GameObject itemGetVfx;

    [SerializeField] private Transform itemVfxAnchor;
    [SerializeField] private Vector3 itemTextOffset = new(0f, 1.2f, 0f);

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
        //Debug.Log($"[LVUP_VFX] spawned name={vfx.name} active={vfx.activeInHierarchy} pos={vfx.transform.position} layer={vfx.layer}");

        //var r = vfx.GetComponentInChildren<Renderer>(true);
        //if (r != null)
        //{
        //    Debug.Log($"[LVUP_VFX] renderer={r.GetType().Name} enabled={r.enabled} sortingLayer={r.sortingLayerName} order={r.sortingOrder}");
        //}
        //else
        //{
        //    Debug.LogWarning("[LVUP_VFX] Renderer not found");
        //}

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

    public void SpawnItemGetText(string text, Color color)
    {
        if (itemGetVfx == null)
            return;

        var anchor = itemVfxAnchor != null ? itemVfxAnchor : transform;
        var go = Instantiate(itemGetVfx, anchor.position + itemTextOffset, Quaternion.identity);

        // TextMeshPro(Text) / TextMeshProUGUI どちらでも拾えるよう TMP_Text で取得
        var tmp = go.GetComponentInChildren<TMP_Text>(true);
        if (tmp != null)
        {
            tmp.text = text;
            tmp.color = color;
        }
    }

}
