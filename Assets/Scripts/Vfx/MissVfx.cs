using UnityEngine;
using TMPro;

public class MissVfx : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.6f;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private Vector3 startOffset = new Vector3(0f, 1f, 0f);

    private float timer;
    private TextMeshPro text;

    private void Awake()
    {
        text = GetComponent<TextMeshPro>();
        transform.position += startOffset;
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
