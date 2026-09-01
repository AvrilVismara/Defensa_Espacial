using UnityEditor.Experimental.GraphView;
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

    [Header("Animaciones")]
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private PlayerMovement inputActions;
    private Vector2 moveInput;
    private bool isRunning;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerMovement();
        animator= GetComponent<Animator>();
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
        isRunning = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        isRunning = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded == true)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
        }
    }

    void Update()
    {
        
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundlayer);
        }

        animator.SetBool("isGrounded", isGrounded);

        float targetAnimSpeed;

        if (moveInput != Vector2.zero)
        {
            if(isRunning)
            {
                targetAnimSpeed = 2F;
            }
            else
            {
                targetAnimSpeed = 1F;
            }

        }
        else
        {
            targetAnimSpeed = 0f;
        }
        animator.SetFloat("Speed", targetAnimSpeed, 0.1f, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        float currentMultiplier;
        if(isRunning)
        {
            currentMultiplier = runMultiplier;
        }
        else
        {
            currentMultiplier = 1f;
        }

        float currentSpeed = speed * (isRunning ? runMultiplier : speed);

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        Vector3 targetVelocity = direction * currentSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
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
