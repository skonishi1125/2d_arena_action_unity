using TMPro;
using UnityEngine;

public class DamageNumberVfx : MonoBehaviour
{
    [SerializeField] private float lifeTime = .5f;
    [SerializeField] private float floatSpeed = .2f;
    [SerializeField] private Vector3 startOffset = new Vector3(0f, 1f, 0f);

    private float timer;
    private TextMeshPro text;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        transform.position += startOffset;
    }

    public void Init(float damage, Color color)
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshPro>(true);

        text.text = Mathf.CeilToInt(damage).ToString(); // 表示仕様に合わせて
        text.color = color;
    }

    private void Update()
    {
        // スパイクで一瞬で終わらないように上限を付ける
        float dt = Mathf.Min(Time.deltaTime, 1f / 30f);

        timer += dt;

        transform.position += Vector3.up * floatSpeed * dt;

        float t = Mathf.Clamp01(timer / lifeTime);

        if (text != null)
        {
            var c = text.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            text.color = c;
        }

        if (timer >= lifeTime)
            Destroy(gameObject);
    }
}
