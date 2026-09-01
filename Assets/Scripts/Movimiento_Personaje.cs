using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement_Player : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad;
    [SerializeField] private float multiplicadorVelocidad;

    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto;
    [SerializeField] private LayerMask capaDelSuelo;
    [SerializeField] private Transform CkeckCapaSuelo;
    [SerializeField] private float distanciaSuelo;

    [Header("Animaciones")]
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private PlayerMovement inputDeAcciones;
    private Vector2 inputDeMovimiento;
    private bool Corriendo;
    private bool EnSuelo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputDeAcciones = new PlayerMovement();
        animator= GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputDeAcciones.Player.Enable();

        inputDeAcciones.Player.Move.performed += OnMovePerformed;
        inputDeAcciones.Player.Move.canceled += OnMoveCanceled;

        inputDeAcciones.Player.Run.performed += OnRunPerformed;
        inputDeAcciones.Player.Run.canceled += OnRunCanceled;

        inputDeAcciones.Player.Jump.performed += OnJumpPerformed;

    }

    private void OnDisable()
    {
        inputDeAcciones.Player.Move.performed -= OnMovePerformed;
        inputDeAcciones.Player.Move.canceled -= OnMoveCanceled;

        inputDeAcciones.Player.Run.performed -= OnRunPerformed;
        inputDeAcciones.Player.Run.canceled -= OnRunCanceled;

        inputDeAcciones.Player.Jump.performed -= OnJumpPerformed;

        inputDeAcciones.Player.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        inputDeMovimiento = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        inputDeMovimiento = Vector2.zero;
    }

    private void OnRunPerformed(InputAction.CallbackContext context)
    {
        Corriendo = true;
    }

    private void OnRunCanceled(InputAction.CallbackContext context)
    {
        Corriendo = false;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (EnSuelo == true)
        {
            rb.AddForce(Vector3.up * fuerzaDeSalto, ForceMode.Impulse);
            animator.SetTrigger("Salto");
        }
    }

    void Update()
    {
        
        if (CkeckCapaSuelo != null)
        {
            EnSuelo = Physics.CheckSphere(CkeckCapaSuelo.position, distanciaSuelo, capaDelSuelo);
        }

        animator.SetBool("EnSuelo", EnSuelo);

        float targetAnimSpeed;

        if (inputDeMovimiento != Vector2.zero)
        {
            if(Corriendo)
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
        animator.SetFloat("velocidad", targetAnimSpeed, 0.1f, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        float currentMultiplier;
        if(Corriendo)
        {
            currentMultiplier = multiplicadorVelocidad;
        }
        else
        {
            currentMultiplier = 1f;
        }

        float currentSpeed = velocidad * (Corriendo ? multiplicadorVelocidad : velocidad);

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = (cameraForward * inputDeMovimiento.y + cameraRight * inputDeMovimiento.x).normalized;

        Vector3 targetVelocity = direction * currentSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
    }
}
