using UnityEngine;

public class EndlessHorizontalLayer : MonoBehaviour
{
    [Header("Segments (2枚以上)")]
    [SerializeField] private SpriteRenderer[] segments;

    [Header("Scroll")]
    [SerializeField] private float speed = 0.5f; // world units per second
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Optional: sprite cycle (A/Bの絵を順に差し替えたい場合)")]
    [SerializeField] private Sprite[] spriteCycle;

    [Header("Pixel Art (任意)")]
    [SerializeField] private bool snapToPixel = false;
    [SerializeField] private float pixelsPerUnit = 64f;

    private Camera cam;
    private int nextSpriteIndex = 0;

    private void Awake()
    {
        cam = Camera.main;

        Debug.Log(cam);

        if (segments == null || segments.Length < 2)
            Debug.LogError($"{name}: segments は2枚以上指定してください。");

        // spriteCycleが指定されているなら初期配布
        if (spriteCycle != null && spriteCycle.Length > 0)
        {
            for (int i = 0; i < segments.Length; i++)
                segments[i].sprite = spriteCycle[i % spriteCycle.Length];

            nextSpriteIndex = segments.Length % spriteCycle.Length;
        }
    }

    private void Start()
    {
        ArrangeLeftToRight();
    }

    private void Update()
    {
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float dx = speed * dt;

        // 左へ移動
        for (int i = 0; i < segments.Length; i++)
        {
            var t = segments[i].transform;
            t.position += Vector3.left * dx;

            if (snapToPixel)
                t.position = SnapPositionToPixel(t.position);
        }

        RecycleIfNeeded();
    }

    private void ArrangeLeftToRight()
    {
        // 現在位置のx昇順で並べ直してから、隙間なく右へ連結
        System.Array.Sort(segments, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        for (int i = 1; i < segments.Length; i++)
        {
            var prev = segments[i - 1];
            var cur = segments[i];

            float targetLeft = prev.bounds.max.x;     // 前の右端
            float delta = targetLeft - cur.bounds.min.x; // 現在の左端を合わせる差分

            cur.transform.position += new Vector3(delta, 0f, 0f);

            if (snapToPixel)
                cur.transform.position = SnapPositionToPixel(cur.transform.position);
        }
    }

    private void RecycleIfNeeded()
    {
        float camLeft = GetCameraLeftX(planeZ: 0f);
        float margin = 0.05f; // ちらつき防止の余白

        for (int i = 0; i < segments.Length; i++)
        {
            var sr = segments[i];

            // 右端がカメラ左端より十分左へ行ったら、右端へ付け替え
            if (sr.bounds.max.x < camLeft - margin)
            {
                var rightMost = FindRightMost();
                float targetLeft = rightMost.bounds.max.x;
                float delta = targetLeft - sr.bounds.min.x;

                sr.transform.position += new Vector3(delta, 0f, 0f);

                // 次の絵へ差し替え（任意）
                if (spriteCycle != null && spriteCycle.Length > 0)
                {
                    sr.sprite = spriteCycle[nextSpriteIndex];
                    nextSpriteIndex = (nextSpriteIndex + 1) % spriteCycle.Length;
                }

                if (snapToPixel)
                    sr.transform.position = SnapPositionToPixel(sr.transform.position);
            }
        }
    }

    private SpriteRenderer FindRightMost()
    {
        SpriteRenderer rightMost = segments[0];
        float maxX = rightMost.bounds.max.x;

        for (int i = 1; i < segments.Length; i++)
        {
            float x = segments[i].bounds.max.x;
            if (x > maxX)
            {
                maxX = x;
                rightMost = segments[i];
            }
        }
        return rightMost;
    }

    private float GetCameraLeftX(float planeZ)
    {
        if (cam == null) cam = Camera.main;
        float zDist = planeZ - cam.transform.position.z; // カメラZが -10 なら planeZ(0) まで 10
        var p = cam.ViewportToWorldPoint(new Vector3(0f, 0f, zDist));
        return p.x;
    }

    private Vector3 SnapPositionToPixel(Vector3 pos)
    {
        float unitPerPixel = 1f / Mathf.Max(1f, pixelsPerUnit);
        pos.x = Mathf.Round(pos.x / unitPerPixel) * unitPerPixel;
        pos.y = Mathf.Round(pos.y / unitPerPixel) * unitPerPixel;
        return pos;
    }
}
