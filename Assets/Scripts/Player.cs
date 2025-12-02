using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    // New Input System
    public PlayerInputSet input { get; private set; }

    // StateMachine
    // Playerの状態を別のコードでも見ることになるので、publicとしておくとよい
    public StateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; } // moveからidleに遷移するときなどに、参照するのでpublic
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerFallState fallState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerBasicAttackState basicAttackState { get; private set; }
    public PlayerAirAttackState airAttackState { get; private set; }


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
    public Vector2[] attackVelocities; // 攻撃時の前方向の加速度 技それぞれに持つ。
    public float attackVelocityDuration = .1f;
    public float comboResetTime = .5f; // 攻撃時、この時間だけ何もしなければcomboIndexが1に戻る。
    private Coroutine queuedAttackCo;
    [SerializeField] private float attackInputBufferTime = 0.2f; // 先行入力受付時間（秒）
    // 直近で攻撃ボタンが押された時間
    [HideInInspector] public float lastAttackInputTime = Mathf.NegativeInfinity;
    [SerializeField] private float airAttackFallSpeed = -1.5f; // 攻撃中にゆっくり落ちる速度（負の値）
    [SerializeField] private float airAttackVerticalAccel = 10f; // どのくらいの速さでtargetVyに近づけるか

    // 公開用変数等
    public float AttackInputBufferTime => attackInputBufferTime;
    public float AirAttackFallSpeed => airAttackFallSpeed;
    public float AirAttackVerticalAccel => airAttackVerticalAccel;


    protected override void Awake()
    {
        base.Awake();
        // New Input System
        input = new PlayerInputSet();


        // StateMachine
        // ※Components取得より手前に書くと、contruct上のrb割当等でnullになるので注意
        stateMachine = new StateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "idle");
        moveState = new PlayerMoveState(this, stateMachine, "move");
        jumpState = new PlayerJumpState(this, stateMachine, "jumpFall");
        fallState = new PlayerFallState(this, stateMachine, "jumpFall");
        dashState = new PlayerDashState(this, stateMachine, "dash");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, "wallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "jumpFall");
        basicAttackState = new PlayerBasicAttackState(this, stateMachine, "basicAttack");
        airAttackState = new PlayerAirAttackState(this, stateMachine, "airAttack");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState); // 初期状態の設定 + 入口処理
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.LogicUpdate(); // 状態中の処理
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        stateMachine.currentState.PhysicsUpdate();
    }

    // 攻撃モーションの終わりに呼び出すトリガー用メソッド
    // こちらをアニメに割り当てている
    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
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
