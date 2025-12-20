using UnityEngine;

public class Chest : MonoBehaviour, IDamagable
{
    private EntityVFX entityVfx;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;

    [SerializeField] private Vector2 openKnockback;

    [Header("Drop")]
    [SerializeField] private Transform dropPoint;
    [SerializeField] private GameObject[] itemPrefabs;

    private bool opened;
    private bool dropped;

    private void Awake()
    {
        entityVfx = GetComponent<EntityVFX>();

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
    }
    public void TakeDamage(DamageContext ctx)
    {
        if (opened)
            return;
        opened = true;

        if (anim != null)
            anim.SetBool("chestOpen", true);

        if (rb != null)
        {
            rb.linearVelocity = openKnockback;
            rb.angularVelocity = Random.Range(-200, 200);
        }

        entityVfx.PlayOnDamageVfx();
    }

    // Animation Eventから呼ぶ。アイテムスポーン処理
    public void SpawnRandomItem()
    {
        if (dropped) return;
        dropped = true;

        if (itemPrefabs == null || itemPrefabs.Length == 0) return;

        var prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
        if (prefab == null) return;

        Vector3 pos = (dropPoint != null) ? dropPoint.position : transform.position;
        var go = Instantiate(prefab, pos, Quaternion.identity);

        // アイテム飛び出しモーション
        var motion = go.GetComponent<ItemFloatMotion>();
        if (motion != null)
            motion.Play();
    }

    // Animation Event から呼ぶ（アニメ終端）
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

}
