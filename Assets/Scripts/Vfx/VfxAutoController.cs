using UnityEngine;

// ヒット時の斬撃エフェクトなどの色を変えたり、破棄したりする
// VFXにComponentとして割り当てて使う。
public class VfxAutoController : MonoBehaviour
{
    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1f;

    // 斬撃モーションなどが単調にならないように、適度にランダム回転させる
    [Header("Random Position")]
    [SerializeField] private bool randomOffset = true;
    [SerializeField] private bool randomRotation = true;
    [SerializeField] private float xMinOffset = -.3f;
    [SerializeField] private float xMaxOffset = .3f;
    [Space]
    [SerializeField] private float yMinOffset = -.3f;
    [SerializeField] private float yMaxOffset = .3f;
    [Space]
    [SerializeField] private float minRotate = 270f;
    [SerializeField] private float maxRotate = 360f;


    private void Start()
    {
        ApplyRandomOffset();
        ApplyRandomRotation();

        if (autoDestroy)
            Destroy(gameObject, destroyDelay);

    }

    private void ApplyRandomOffset()
    {
        if (randomOffset == false)
            return;

        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);

        transform.position = transform.position + new Vector3(xOffset, yOffset);
    }

    private void ApplyRandomRotation()
    {
        if (randomRotation == false)
            return;

        float zRotation = Random.Range(minRotate, maxRotate);
        transform.Rotate(0,0, zRotation);
    }

}
