using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public static MinimapController Instance { get; private set; }

    public enum IconType
    {
        Player,
        Enemy,
        Objective
    }

    [Header("References")]
    [SerializeField] private RectTransform minimapRect;     // 黒い四角のRectTransform
    [SerializeField] private RectTransform iconsRoot;       // アイコン配置先
    [SerializeField] private Image dotPrefab;               // 小さい点のPrefab
    [SerializeField] private Transform objective;           // ミニマップ中心
    [SerializeField] private Transform player;              // プレイヤー
    [SerializeField] private Collider2D stageBoundsCollider; // ステージ範囲（BoxCollider2Dなど）

    [Header("Icon Settings")]
    [SerializeField] private float playerSize = 6f;
    [SerializeField] private float enemySize = 4f;
    [SerializeField] private float objectiveSize = 7f;

    [SerializeField] private float edgePadding = 4f; // 枠の内側に収める余白

    private readonly Dictionary<Transform, RectTransform> _icons = new();
    private readonly Dictionary<Transform, IconType> _types = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 最低限：Player / Objective を自動登録
        if (objective != null) Register(objective, IconType.Objective);
        if (player != null) Register(player, IconType.Player);
    }

    public void Register(Transform target, IconType type)
    {
        if (target == null || dotPrefab == null || iconsRoot == null) return;
        if (_icons.ContainsKey(target)) return;

        var img = Instantiate(dotPrefab, iconsRoot);
        var rt = img.rectTransform;

        // 色とサイズ
        ApplyVisual(img, type);

        _icons[target] = rt;
        _types[target] = type;
    }

    public void Unregister(Transform target)
    {
        if (target == null) return;
        if (_icons.TryGetValue(target, out var rt) && rt != null)
            Destroy(rt.gameObject);

        _icons.Remove(target);
        _types.Remove(target);
    }

    private void LateUpdate()
    {
        if (objective == null || stageBoundsCollider == null || minimapRect == null) return;

        // ステージ範囲を毎フレーム取得（サイズ変更に追従）
        Bounds b = stageBoundsCollider.bounds;

        // Objective中心で、左右上下どこまで映すか（Objectiveから端までの距離）
        float halfWorldW = Mathf.Max(Mathf.Abs(objective.position.x - b.min.x), Mathf.Abs(b.max.x - objective.position.x));
        float halfWorldH = Mathf.Max(Mathf.Abs(objective.position.y - b.min.y), Mathf.Abs(b.max.y - objective.position.y));

        // 0割回避
        halfWorldW = Mathf.Max(halfWorldW, 0.0001f);
        halfWorldH = Mathf.Max(halfWorldH, 0.0001f);

        float halfUIW = minimapRect.rect.width * 0.5f - edgePadding;
        float halfUIH = minimapRect.rect.height * 0.5f - edgePadding;

        // null除去しながら更新
        _tmpRemove.Clear();
        foreach (var kv in _icons)
        {
            var target = kv.Key;
            var iconRt = kv.Value;

            if (target == null || iconRt == null)
            {
                _tmpRemove.Add(target);
                continue;
            }

            Vector3 delta = target.position - objective.position;

            // 正規化してUIにマッピング
            float nx = Mathf.Clamp(delta.x / halfWorldW, -1f, 1f);
            float ny = Mathf.Clamp(delta.y / halfWorldH, -1f, 1f);

            iconRt.anchoredPosition = new Vector2(nx * halfUIW, ny * halfUIH);
        }

        for (int i = 0; i < _tmpRemove.Count; i++)
            Unregister(_tmpRemove[i]);
    }

    private readonly List<Transform> _tmpRemove = new();

    private void ApplyVisual(Image img, IconType type)
    {
        switch (type)
        {
            case IconType.Player:
                img.color = Color.white;
                img.rectTransform.sizeDelta = Vector2.one * playerSize;
                break;

            case IconType.Enemy:
                img.color = Color.red;
                img.rectTransform.sizeDelta = Vector2.one * enemySize;
                break;

            case IconType.Objective:
                img.color = Color.cyan; // 青寄り
                img.rectTransform.sizeDelta = Vector2.one * objectiveSize;
                break;
        }
    }

    // 外部から呼ぶ簡易API
    public void RegisterEnemy(Transform enemy) => Register(enemy, IconType.Enemy);

}
