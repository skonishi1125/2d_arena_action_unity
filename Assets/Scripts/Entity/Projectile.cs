using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float speed;
    private Vector2 direction;
    private Entity owner; // 誰が撃ったか

    [SerializeField] private float lifeTime = 3f;
    private float timer;

    public void Fire(Vector2 dir, float speed, Entity owner)
    {
        this.direction = dir;
        this.speed = speed;
        this.owner = owner;
        timer = lifeTime;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        timer -= Time.deltaTime;
        if (timer <= 0f)
            Destroy(gameObject);
    }
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
