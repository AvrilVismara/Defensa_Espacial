using UnityEngine;
using UnityEngine.InputSystem;

public class Movement_Player : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed;
    [SerializeField] private float runMultiplier;

    [Header("Salto")]
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundlayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance;

    private Rigidbody rb;
    private PlayerMovement inputActions;
    private Vector2 moveInput;
    private bool isRuning;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerMovement();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;

        inputActions.Player.Run.performed += OnRunPerformed;
        inputActions.Player.Run.canceled += OnRunCanceled;

        inputActions.Player.Jump.performed += OnJumpPerformed;

    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;

        inputActions.Player.Run.performed -= OnRunPerformed;
        inputActions.Player.Run.canceled -= OnRunCanceled;

        inputActions.Player.Jump.performed -= OnJumpPerformed;

        inputActions.Player.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        isRuning = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        isRuning = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded == true)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }


    void Update()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundlayer);
        }

        float currentSpeed = speed * (isRuning ? runMultiplier : speed);

        Vector3 direction = new Vector3(moveInput.x, 0F, moveInput.y);
        transform.Translate(direction * currentSpeed * Time.deltaTime, Space.World);
    }

    private void FixedUpdate()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
