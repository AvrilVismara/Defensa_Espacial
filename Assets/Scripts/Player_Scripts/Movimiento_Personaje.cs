using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movimiento_Personaje : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;            // Velocidad base al caminar
    [SerializeField] private float multiplicadorVelocidad = 1.5f; // Factor de multiplicación al correr
    [SerializeField] private float velocidadRotacion = 25f;    // Velocidad con la que el cuerpo se alinea a la cámara

    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto = 5f;        // Fuerza vertical aplicada para saltar
    [SerializeField] private LayerMask capaDelSuelo;          // Máscara para filtrar qué objetos cuentan como suelo
    [SerializeField] private Transform CkeckCapaSuelo;        // Objeto vacío en la base de los pies para el detector
    [SerializeField] private float distanciaSuelo = 0.2f;     // Radio del detector esférico de suelo

    [Header("Animaciones")]
    [SerializeField] private Animator animator;              // Referencia al componente Animator

    private Rigidbody rb;                                     // Referencia al componente de físicas
    private Collider miCollider;                              // Referencia al Collider del personaje (CapsuleCollider)
    private PlayerMovement inputDeAcciones;                   // Clase auto-generada del Input System
    private Vector2 inputDeMovimiento;                        // Entrada analógica o teclado (WASD)
    private bool Corriendo;                                   // Estado de la acción de correr
    private bool EnSuelo = false;                             // Confirmación de contacto con la superficie

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        miCollider = GetComponent<Collider>();
        inputDeAcciones = new PlayerMovement();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

    private void OnMovePerformed(InputAction.CallbackContext context) => inputDeMovimiento = context.ReadValue<Vector2>();
    private void OnMoveCanceled(InputAction.CallbackContext context) => inputDeMovimiento = Vector2.zero;
    private void OnRunPerformed(InputAction.CallbackContext context) => Corriendo = true;
    private void OnRunCanceled(InputAction.CallbackContext context) => Corriendo = false;

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (EnSuelo)
        {
            rb.AddForce(Vector3.up * fuerzaDeSalto, ForceMode.Impulse);
            if (animator != null) animator.SetTrigger("Salto");
        }
    }



    private void OnDrawGizmosSelected()
    {

        if (CkeckCapaSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(CkeckCapaSuelo.position, distanciaSuelo);
        }
    }

    void Update()
    {
        // 1. Detección de suelo
        if (CkeckCapaSuelo != null)
        {
            EnSuelo = Physics.CheckSphere(CkeckCapaSuelo.position, distanciaSuelo, capaDelSuelo);
        }

        // 2. Control de parámetros del Animator
        if (animator != null)
        {
            animator.SetBool("EnSuelo", EnSuelo);

            float targetAnimSpeed = (inputDeMovimiento != Vector2.zero) ? (Corriendo ? 2f : 1f) : 0f;
            animator.SetFloat("velocidad", targetAnimSpeed, 0.1f, Time.deltaTime);
        }

        // 4. Alineación del cuerpo a la vista de la cámara
        if (Camera.main != null)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0f;

            if (cameraForward.sqrMagnitude > 0.001f)
            {
                cameraForward.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, velocidadRotacion * Time.deltaTime);
            }
        }
    }

    private void FixedUpdate()
    {

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        float currentSpeed = velocidad * (Corriendo ? multiplicadorVelocidad : 1f);
        Vector3 direction = (cameraForward * inputDeMovimiento.y + cameraRight * inputDeMovimiento.x).normalized;
        Vector3 targetVelocity = direction * currentSpeed;

        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }
}