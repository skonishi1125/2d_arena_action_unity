using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // New Input System
    private PlayerInputSet input;

    [Header("Components")]
    private Rigidbody2D rb;
    private Collider2D co;
    private SpriteRenderer sr;

    [Header("Input Settings")]
    public Vector2 moveInput { get; private set; } // InputSystem‚Ìdigital -1,0,1
    [SerializeField] private float moveSpeed = 5f;


    private void Awake()
    {
        input = new PlayerInputSet();
        rb = GetComponent<Rigidbody2D>();
        co = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
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
