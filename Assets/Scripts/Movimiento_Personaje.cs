using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Movimiento_Personaje : MonoBehaviour
{
    // =========================================================
    // CONFIGURACIÓN DE MOVIMIENTO
    // =========================================================
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;            // Velocidad base al caminar
    [SerializeField] private float multiplicadorVelocidad = 1.5f; // Factor de multiplicación al correr
    [SerializeField] private float velocidadRotacion = 25f;    // Velocidad con la que el cuerpo se alinea a la cámara

    // =========================================================
    // CONFIGURACIÓN DE SALTO ESTÁNDAR
    // =========================================================
    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto = 5f;        // Fuerza vertical aplicada para saltar
    [SerializeField] private LayerMask capaDelSuelo;          // Máscara para filtrar qué objetos cuentan como suelo
    [SerializeField] private Transform CkeckCapaSuelo;        // Objeto vacío en la base de los pies para el detector
    [SerializeField] private float distanciaSuelo = 0.2f;     // Radio del detector esférico de suelo

    // =========================================================
    // CONFIGURACIÓN DE SALTO DE OBSTÁCULOS (VAULTING)
    // =========================================================
    [Header("Saltar Obstaculos")]
    [SerializeField] private Transform OrigenSaltoCkeck;      // Objeto desde donde sale el Raycast (pecho/cintura)
    [SerializeField] private float saltoObsDistancia = 1.2f;  // Distancia frontal máxima que detecta el Raycast
    [SerializeField] private float duracionAnimacion = 1.2f;  // Tiempo en segundos que dura la maniobra
    [SerializeField] private LayerMask obstacleLayer;         // Máscara para detectar únicamente los obstáculos
    [SerializeField] private float offsetAlturaObstaculo = 0.5f; // Altura extra añadida para elevar la meta del salto

    // =========================================================
    // REFERENCIAS Y VARIABLES DE ESTADO
    // =========================================================
    [Header("Animaciones")]
    [SerializeField] private Animator animator;              // Referencia al componente Animator

    private Rigidbody rb;                                     // Referencia al componente de físicas
    private Collider miCollider;                              // Referencia al Collider del personaje (CapsuleCollider)
    private PlayerMovement inputDeAcciones;                   // Clase auto-generada del Input System
    private Vector2 inputDeMovimiento;                        // Entrada analógica o teclado (WASD)
    private bool Corriendo;                                   // Estado de la acción de correr
    private bool EnSuelo = false;                             // Confirmación de contacto con la superficie

    private bool SaltandoObstaculo;                           // Controla si la maniobra de vault está activa
    private float SaltandoTimer;                              // Temporizador para finalizar la animación
    private Vector3 puntoObstaculo;                           // Punto de impacto procesado del Raycast

    // =========================================================
    // INICIALIZACIÓN Y EVENTOS DE INPUT
    // =========================================================
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        miCollider = GetComponent<Collider>();
        inputDeAcciones = new PlayerMovement();
        animator = GetComponent<Animator>();
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

    // =========================================================
    // ACCIÓN DE SALTO Y DETECCIÓN DE OBSTÁCULOS
    // =========================================================
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (SaltandoObstaculo) return;

        if (IntentaSaltarObstaculo())
        {
            return;
        }

        if (EnSuelo)
        {
            rb.AddForce(Vector3.up * fuerzaDeSalto, ForceMode.Impulse);
            if (animator != null) animator.SetTrigger("Salto");
        }
    }

    private bool IntentaSaltarObstaculo()
    {
        if (OrigenSaltoCkeck != null && Physics.Raycast(OrigenSaltoCkeck.position, transform.forward, out RaycastHit hit, saltoObsDistancia, obstacleLayer))
        {
            SaltandoObstaculo = true;
            SaltandoTimer = duracionAnimacion;

            puntoObstaculo = hit.point + (Vector3.up * offsetAlturaObstaculo);

            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;

            if (miCollider != null)
            {
                miCollider.enabled = false;
            }

            if (animator != null) animator.SetTrigger("SaltarObstaculo");
            return true;
        }

        return false;
    }

    private void FinalizarSaltoObstaculo()
    {
        SaltandoObstaculo = false;
        rb.useGravity = true;

        if (miCollider != null)
        {
            miCollider.enabled = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (OrigenSaltoCkeck != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(OrigenSaltoCkeck.position, transform.forward * saltoObsDistancia);
        }
    }

    // =========================================================
    // BUCLE DE ACTUALIZACIÓN (UPDATE)
    // =========================================================
    void Update()
    {
        // 1. Detección de suelo
        if (CkeckCapaSuelo != null)
        {
            EnSuelo = Physics.CheckSphere(CkeckCapaSuelo.position, distanciaSuelo, capaDelSuelo);
        }

        if (animator != null)
        {
            animator.SetBool("EnSuelo", EnSuelo);

            // Parámetro de velocidad para el Blend Tree (0: Idle, 1: Walk, 2: Run)
            float targetAnimSpeed = (inputDeMovimiento != Vector2.zero) ? (Corriendo ? 2f : 1f) : 0f;
            animator.SetFloat("velocidad", targetAnimSpeed, 0.1f, Time.deltaTime);
        }

        // 2. Control de la maniobra de salto de obstáculo (Vaulting)
        if (SaltandoObstaculo)
        {
            if (animator != null && !animator.IsInTransition(0) && !animator.isMatchingTarget && animator.GetCurrentAnimatorStateInfo(0).IsName("Vault1"))
            {
                animator.MatchTarget(
                    puntoObstaculo,
                    Quaternion.identity,
                    AvatarTarget.Root,
                    new MatchTargetWeightMask(Vector3.one, 0f),
                    0.0f,
                    0.6f
                );
            }

            SaltandoTimer -= Time.deltaTime;
            if (SaltandoTimer <= 0f)
            {
                FinalizarSaltoObstaculo();
            }
            return; // Si estamos en vaulting, no procesamos rotación de cámara
        }

        // 3. ROTACIÓN CONTINUA ESTILO SHOOTER (Alineación constante del personaje a la cámara)
        if (Camera.main != null)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0f; // Evitamos rotaciones en vertical

            if (cameraForward.sqrMagnitude > 0.001f)
            {
                cameraForward.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, velocidadRotacion * Time.deltaTime);
            }
        }
    }

    // =========================================================
    // BUCLE DE FÍSICAS (FIXED UPDATE)
    // =========================================================
    private void FixedUpdate()
    {
        // Si estamos ejecutando la animación de Vault, dejamos que MatchTarget controle la posición
        if (SaltandoObstaculo || Camera.main == null) return;

        // Calculamos las direcciones horizontales relativas a la cámara
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculamos la velocidad actual según el estado de la tecla Run
        float currentSpeed = velocidad * (Corriendo ? multiplicadorVelocidad : 1f);

        // Vector final de desplazamiento basado en las entradas (WASD)
        Vector3 direction = (cameraForward * inputDeMovimiento.y + cameraRight * inputDeMovimiento.x).normalized;
        Vector3 targetVelocity = direction * currentSpeed;

        // Aplicamos la velocidad al Rigidbody preservando el eje Y para la gravedad
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }
}