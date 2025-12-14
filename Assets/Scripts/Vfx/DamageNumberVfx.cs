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
        timer += Time.deltaTime;

        // 上方向に移動
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // 経過割合 (0〜1)
        float t = timer / lifeTime;

        // アルファを徐々に0に
        if (text != null)
        {
            var c = text.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            text.color = c;
        }

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
