using System;
using System.Collections;
using UnityEngine;

// 敵味方共通で使用する仕組みをまとめる
public abstract class Entity : MonoBehaviour
{
    // ダメージ倍率,KBの割当てをするため、publicとする
    public EntityCombat EntityCombat;

    protected StateMachine stateMachine;

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
    // EnemyでPlayer検知時、壁ごしのチェックに使うのでprotectedとする
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;

    public bool groundDetected { get; private set; }
    public bool wallDetected { get; private set; }

    // ノックバック中かどうか
    private bool isKnockbacked;
    private Coroutine knockbackCo;

    // 反転したときに同時実行するアクションイベント
    public event Action OnFlipped;


    protected virtual void Awake()
    {
        EntityCombat = GetComponent<EntityCombat>();
        stateMachine = new StateMachine();

        // Components
        rb = GetComponent<Rigidbody2D>();
        if (!LogHelper.AssertNotNull(rb, nameof(rb), this))
            return;

        co = GetComponent<Collider2D>();
        if (!LogHelper.AssertNotNull(co, nameof(co), this))
            return;

        sr = GetComponentInChildren<SpriteRenderer>();
        if (!LogHelper.AssertNotNull(sr, nameof(sr), this))
            return;

        anim = GetComponentInChildren<Animator>();
        if (!LogHelper.AssertNotNull(anim, nameof(anim), this))
            return;

        // 設定されるべき値のチェック
        if (!LogHelper.AssertNotNull(groundCheck, nameof(groundCheck), this))
            return;
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.currentState.LogicUpdate();

    }
    protected virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    // Player, EnemyをStateから操作するのでpublic
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        // ダメージなどでノックバックしたときは、操作を受け付けない。
        if (isKnockbacked)
            return;

        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    public void ReceiveKnockback(Vector2 knockback, float duration)
    {
        if (knockbackCo != null)
            StopCoroutine(knockbackCo);

        knockbackCo = StartCoroutine(KnockbackCo(knockback, duration));
    }

    private IEnumerator KnockbackCo(Vector2 knockback, float duration)
    {
        isKnockbacked = true;
        rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector2.zero;
        isKnockbacked = false;


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

        // 例えば敵HPバーの反転は防ぎたいので、そういった関連eventの実行。
        OnFlipped?.Invoke();

    }

    // 地面, 壁判定チェック
    private void HandleCollisionDetection()
    {
        groundDetected = Physics2D.Raycast(
            groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround
        );

        wallDetected = Physics2D.Raycast(
            wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround
        );
    }

    // 攻撃モーションの終わりに呼び出すトリガー用メソッド
    // こちらをアニメに割り当てている
    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    public virtual void Death()
    {

    }


   // 各種判定チェックのGizmos
   protected virtual void OnDrawGizmos()
    {
        // 地面
        Gizmos.DrawLine(
            groundCheck.position,
            groundCheck.position + new Vector3(0, -groundCheckDistance)
        );

        // 壁
        Gizmos.DrawLine(
            wallCheck.position,
            wallCheck.position + new Vector3(wallCheckDistance * facingDir, 0)
        );
    }

}
