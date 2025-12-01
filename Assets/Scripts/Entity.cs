using UnityEngine;

public abstract class Entity : MonoBehaviour
{

    [Header("Components")]
    public Rigidbody2D rb { get; private set; } // moveStateがrbを使って速度を弄るため。
    public Collider2D co { get; private set; }
    public SpriteRenderer sr { get; private set; }

    protected virtual void Awake()
    {
        // Components
        rb = GetComponent<Rigidbody2D>();
        co = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }
    protected virtual void FixedUpdate()
    {
    }

    // Player, EnemyをStateから操作するのでpublic
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
    }

}
