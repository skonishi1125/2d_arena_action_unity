using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    // New Input System
    private PlayerInputSet input;

    // StateMachine
    // Playerの状態を別のコードでも見ることになるので、publicとしておくとよい
    public StateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; } // moveからidleに遷移するときなどに、参照するのでpublic
    public PlayerMoveState moveState { get; private set; }


    [Header("Input Settings")]
    public Vector2 moveInput { get; private set; } // InputSystemのdigital -1,0,1
    public float moveSpeed = 5f; // moveState等が扱うので public


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
    }

    protected override void Start()
    {
        stateMachine.Initialize(idleState); // 初期状態の設定 + 入口処理
    }

    protected override void Update()
    {
        stateMachine.currentState.LogicUpdate(); // 状態中の処理
    }

    protected override void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
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
