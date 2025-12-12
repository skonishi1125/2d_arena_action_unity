using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    // New Input System
    public PlayerInputSet input { get; private set; }

    // GameManagerなど、Playerを持つObjectが参照するために使う
    public PlayerHealth Health { get; private set; }
    public PlayerLevel Level { get; private set; }
    public PlayerSkillController Skill {  get; private set; }
    public PlayerVFX Vfx { get; private set; }

    // StateMachine
    // Playerの状態を別のコードでも見ることになるので、publicとしておくとよい
    //public StateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; } // moveからidleに遷移するときなどに、参照するのでpublic
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerBasicAttackState basicAttackState { get; private set; }
    public PlayerAirAttackState airAttackState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    public PlayerKnockbackAttackState knockbackAttackState { get; private set; }
    public PlayerMagicBoltState magicBoltState { get; private set; }
    public PlayerTeleportState teleportState { get; private set; }

    // MagicBoltStateや、その他の弾スキルで作ったダメージ情報を保持する場所
    // State -> player -> Trigger(spawn())として、情報を仲介してやる。
    public ProjectileDamageContext PendingProjectileCtx { get; private set; }
    public bool HasPendingProjectileCtx { get; private set; }


    [Header("Input Settings")]
    public Vector2 moveInput { get; private set; } // InputSystemのdigital -1,0,1
    public float moveSpeed = 5f; // moveState等が扱うので public
    public float jumpForce = 5f;
    public int maxJumps = 2; // 多段ジャンプ
    [HideInInspector] public int jumpCount = 0;
    [Range(0, 1)] // 空中移動補正
    public float inAirMoveMultiplier = .8f;
    [Range(0, 1)] // ダッシュ全体時間
    public float dashDuration = .25f;
    public float dashSpeed = 20f;
    public float wallSlideSlowMultiplier = .5f; // 壁張り付き中落下速度
    public Vector2 wallJumpDir; // 壁ジャンプ時の初期ベクトル

    [Header("Attack details")]
    public float attackVelocityDuration = .1f;
    public float comboResetTime = .5f; // 攻撃時、この時間だけ何もしなければcomboIndexが1に戻る。
    private Coroutine queuedAttackCo;
    [SerializeField] private float attackInputBufferTime = 0.2f; // 先行入力受付時間（秒）
    // 直近で攻撃ボタンが押された時間
    [HideInInspector] public float lastAttackInputTime = Mathf.NegativeInfinity;
    [SerializeField] private float airAttackFallSpeed = -1.5f; // 攻撃中にゆっくり落ちる速度（負の値）
    [SerializeField] private float airAttackVerticalAccel = 10f; // どのくらいの速さでtargetVyに近づけるか

    [Header("Basic Attack Details")]
    public Vector2[] basicAttackVelocities; // 3コンボ攻撃の各種x加速度 技それぞれに持つ。
    public float[] basicAttackDamageMultipliers; // 3コンボ攻撃の攻撃倍率
    public Vector2[] basicAttackKnockbackPowers; // 3コンボ攻撃の吹っ飛ばし力
    public float[] basicAttackKnockbackDurations; // 3コンボ攻撃のふっとばし時間

    [Header("Air Attack Details")]
    public float airAttackDamageMultiplier;
    public Vector2 airAttackKnockbackPower;
    public float airAttackKnockbackDuration;

    [Header("Teleport")]
    public float teleportDistance = 4f;
    public float teleportCheckStep = 0.25f; // 安全位置探索の刻み
    public float teleportRadius = 0.2f;     // 埋まりチェック用


    // 公開用変数等
    public float AttackInputBufferTime => attackInputBufferTime;
    public float AirAttackFallSpeed => airAttackFallSpeed;
    public float AirAttackVerticalAccel => airAttackVerticalAccel;

    //// Action Event
    //public static event Action OnPlayerDeath;


    protected override void Awake()
    {
        base.Awake();
        // New Input System
        input = new PlayerInputSet();

        // StateMachine
        // ※Components取得より後に書くと、contruct上のrb割当等でnullになるので注意
        //stateMachine = new StateMachine(); は Entityに書いているので不要
        idleState = new PlayerIdleState(this, stateMachine, "idle");
        moveState = new PlayerMoveState(this, stateMachine, "move");
        jumpState = new PlayerJumpState(this, stateMachine, "jumpFall");
        fallState = new PlayerFallState(this, stateMachine, "jumpFall");
        dashState = new PlayerDashState(this, stateMachine, "dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "jumpFall");
        basicAttackState = new PlayerBasicAttackState(this, stateMachine, "basicAttack");
        airAttackState = new PlayerAirAttackState(this, stateMachine, "airAttack");
        knockbackAttackState = new PlayerKnockbackAttackState(this, stateMachine, "knockbackAttack");
        magicBoltState = new PlayerMagicBoltState(this, stateMachine, "magicBolt");
        deadState = new PlayerDeadState(this, stateMachine, "dead");

        teleportState = new PlayerTeleportState(this, stateMachine, "none");

        // 必要なcomponentの取得
        Health = GetComponent<PlayerHealth>();
        if (!LogHelper.AssertNotNull(Health, nameof(Health), this))
            return;

        Level = GetComponent<PlayerLevel>();
        if (!LogHelper.AssertNotNull(Level, nameof(Level), this))
            return;

        Skill = GetComponent<PlayerSkillController>();
        if (!LogHelper.AssertNotNull(Skill, nameof(Skill), this))
            return;

        Vfx = GetComponent<PlayerVFX>();
        if (!LogHelper.AssertNotNull(Vfx, nameof(Vfx), this))
            return;

        // イベント購読
        Health.OnDied += HandleDied;

    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState); // 初期状態の設定 + 入口処理
    }


    public void EnterAttackStateWithDelay()
    {
        if(queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    // 先行入力のAttackState -> AttackStateへの切り替えの時、
    // フレーム単位で待機しないとエラーが発生する
    // そのため、CoroutineとしてAttackStateへの切り替え処理を作成する
    // CoroutineはMonobehaviourが必要なので、このクラスで実装。
    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }

    // Health.Dieとの違い
    // Player死亡時の自身の振る舞いの管理をする。自身のStateを変化させる。
    public override void Death()
    {
        base.Death();
        stateMachine.ChangeState(deadState);
    }

    // イベントへの入口をはっきりさせるため、Deathと分けておく
    private void HandleDied()
    {
        Death();
    }

    // MagicBoltState等で作った弾ダメージのセット
    public void SetPendingProjectileCtx(ProjectileDamageContext ctx)
    {
        PendingProjectileCtx = ctx;
        HasPendingProjectileCtx = true;
    }

    public bool TryConsumePendingProjectileCtx(out ProjectileDamageContext ctx)
    {
        if (!HasPendingProjectileCtx)
        {
            ctx = default;
            return false;
        }
        ctx = PendingProjectileCtx;
        HasPendingProjectileCtx = false;
        return true;
    }


    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCanceled;
    }

    private void OnDisable()
    {
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCanceled;
        input.Disable();

    }

    private void OnMovementPerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }



}
