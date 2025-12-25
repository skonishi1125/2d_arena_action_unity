using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RainbowTint : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float hueSpeed = 0.25f; // 1.0で1秒=1周

    [Header("HSV")]
    [Range(0f, 1f)][SerializeField] private float saturation = 0.9f;
    [Range(0f, 1f)][SerializeField] private float value = 1.0f;

    [Header("Optional")]
    [SerializeField] private bool useUnscaledTime = false;
    [SerializeField] private bool keepOriginalAlpha = true;

    private SpriteRenderer sr;
    private float hue;
    private float originalAlpha;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalAlpha = sr.color.a;
        hue = Random.value; // 個体差（全員同期で同じ色になるのを避ける）
    }

    private void Update()
    {
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        hue = Mathf.Repeat(hue + hueSpeed * dt, 1f);

        Color c = Color.HSVToRGB(hue, saturation, value);
        if (keepOriginalAlpha) c.a = originalAlpha;

        sr.color = c;
    }
}
