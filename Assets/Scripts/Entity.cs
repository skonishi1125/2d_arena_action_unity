using UnityEngine;

// 敵味方共通で使用する仕組みをまとめる
public abstract class Entity : MonoBehaviour
{

    [Header("Components")]
    public Rigidbody2D rb { get; private set; } // moveStateがrbを使って速度を弄るため。
    public Collider2D co { get; private set; }
    public SpriteRenderer sr { get; private set; }
    public Animator anim { get; private set; } // Stateで切り替え対応するために使う。

    [Header("Common Movement Detail")]
    public bool facingRight { get; private set; } = true;
    public int facingDir { get; private set; } = 1; // 向いている方向 右: 1 左: -1

    [Header("Collision Detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }



    protected virtual void Awake()
    {
        // Components
        rb = GetComponent<Rigidbody2D>();
        co = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
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
    // WallSlideで、着地時（Exit)に反転処理を使うのでpublicとした
    public void Flip()
    {
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        facingDir = facingDir * -1;
    }

    // 地面, 壁判定チェック
    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(
            transform.position, Vector2.down, groundCheckDistance, whatIsGround
        );

        wallDetected = Physics2D.Raycast(
            transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround
        );
    }

    // 各種判定チェックのGizmos
    private void OnDrawGizmos()
    {
        // 地面
        Gizmos.DrawLine(
            transform.position,
            transform.position + new Vector3(0, -groundCheckDistance)
        );

        // 壁
        Gizmos.DrawLine(
            transform.position,
            transform.position + new Vector3(wallCheckDistance * facingDir, 0)
        );
    }

}
