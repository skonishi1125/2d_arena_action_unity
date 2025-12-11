using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private float speed;
    private float shootDir; // 1 or -1 撃った時点での敵の向き
    private Entity owner; // 誰が撃ったか

    [SerializeField] private float lifeTime = 3f;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Fire(float shootDir, float speed, Entity owner)
    {
        this.shootDir = shootDir;
        this.speed = speed;
        this.owner = owner;
        timer = lifeTime;
    }

    private void Update()
    {
        // entity.facingDirを使うと、
        // entityが反転したとき、弾も同時に反転してしまう
        rb.linearVelocity = Vector2.right * speed * shootDir;

        timer -= Time.deltaTime;
        if (timer <= 0f)
            Destroy(gameObject);
    }

    // 弾のダメージ処理
    // EntityCombatは、近接攻撃のダメージ処理担当
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 発射主と同じ陣営には当てない
        if (other.GetComponent<Entity>() == owner)
            return;

        IDamagable d = other.GetComponent<IDamagable>();
        if (d != null)
        {
            d.TakeDamage(5, owner.transform); // ダメージ値は後で調整 or SO で管理
            Destroy(gameObject);
        }
    }


}
