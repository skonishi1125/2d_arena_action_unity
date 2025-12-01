using UnityEngine;

public abstract class Entity : MonoBehaviour
{

    [Header("Components")]
    public Rigidbody2D rb { get; private set; } // moveStateがrbを使って速度を弄るため。
    public Collider2D co { get; private set; }
    public SpriteRenderer sr { get; private set; }

    [Header("Common Movement Detail")]
    private bool facingRight = true;

    [Header("Collision Detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    public bool groundDetected { get; private set; }



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
        HandleCollisionDetection();
    }
    protected virtual void FixedUpdate()
    {
    }

    // Player, EnemyをStateから操作するのでpublic
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    // 右を向いているときに右を向いても反転させず、
    // 左を向いているときに左を向いても反転させないようにする制御処理
    private void HandleFlip(float xVelocity)
    {
        // →入力 かつ 右を向いていないとき
        if (xVelocity > 0 && facingRight == false)
            Flip();
        // ←入力 かつ 右を向いているとき
        else if (xVelocity < 0 && facingRight == true)
            Flip();
    }

    // 反転自体の処理
    private void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    // 地面判定チェック
    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(
            transform.position, Vector2.down, groundCheckDistance, whatIsGround
        );
        Debug.Log(groundDetected);
    }

    // 地面判定チェックのGizmos
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(
            transform.position,
            transform.position + new Vector3(0, -groundCheckDistance, groundCheckDistance)
        );
    }

}
