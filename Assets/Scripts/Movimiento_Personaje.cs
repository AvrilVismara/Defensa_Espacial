using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Movimiento_Personaje : MonoBehaviour
{
    // =========================================================
    // CONFIGURACIÓN DE MOVIMIENTO
    // =========================================================
    [Header("Movimiento")]
    [SerializeField] private float velocidad;                  // Velocidad base al caminar
    [SerializeField] private float multiplicadorVelocidad;      // Factor de multiplicación al correr

    // =========================================================
    // CONFIGURACIÓN DE SALTO ESTÁNDAR
    // =========================================================
    [Header("Salto")]
    [SerializeField] private float fuerzaDeSalto;              // Fuerza vertical aplicada para saltar
    [SerializeField] private LayerMask capaDelSuelo;           // Máscara para filtrar qué objetos cuentan como suelo
    [SerializeField] private Transform CkeckCapaSuelo;         // Objeto vacío en la base de los pies para el detector
    [SerializeField] private float distanciaSuelo;             // Radio del detector esférico de suelo

    // =========================================================
    // CONFIGURACIÓN DE SALTO DE OBSTÁCULOS (VAULTING)
    // =========================================================
    [Header("Saltar Obstaculos")]
    [SerializeField] private Transform OrigenSaltoCkeck;       // Objeto desde donde sale el Raycast (pecho/cintura)
    [SerializeField] private float saltoObsDistancia = 1.2f;   // Distancia frontal máxima que detecta el Raycast
    [SerializeField] private float duracionAnimacion = 1.2f;   // Tiempo en segundos que dura la maniobra
    [SerializeField] private LayerMask obstacleLayer;          // Máscara para detectar únicamente los obstáculos
    [SerializeField] private float offsetAlturaObstaculo = 0.5f;// Altura extra añadida para elevar la meta del salto

    // =========================================================
    // REFERENCIAS Y VARIABLES DE ESTADO
    // =========================================================
    [Header("Animaciones")]
    [SerializeField] private Animator animator;               // Referencia al componente Animator

    private Rigidbody rb;                                      // Referencia al componente de físicas
    private Collider miCollider;                               // Referencia al Collider del personaje (CapsuleCollider)
    private PlayerMovement inputDeAcciones;                   // Clase auto-generada del Input System
    private Vector2 inputDeMovimiento;                        // Entrada analógica o teclado (WASD)
    private bool Corriendo;                                    // Estado de la acción de correr
    private bool EnSuelo = false;                              // Confirmación de contacto con la superficie

    private bool SaltandoObstaculo;                            // Controla si la maniobra de vault está activa
    private float SaltandoTimer;                               // Temporizador para finalizar la animación
    private Vector3 puntoObstaculo;                            // Punto de impacto procesado del Raycast

    // =========================================================
    // INICIALIZACIÓN Y EVENTOS DE INPUT
    // =========================================================
    private void Awake()
    {
        // Se capturan las referencias a los componentes adjuntos al GameObject
        rb = GetComponent<Rigidbody>();
        miCollider = GetComponent<Collider>();
        inputDeAcciones = new PlayerMovement();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        // Se activa el mapa de acciones y se suscriben los eventos de entrada
        inputDeAcciones.Player.Enable();

        inputDeAcciones.Player.Move.performed += OnMovePerformed;
        inputDeAcciones.Player.Move.canceled += OnMoveCanceled;

        inputDeAcciones.Player.Run.performed += OnRunPerformed;
        inputDeAcciones.Player.Run.canceled += OnRunCanceled;

        inputDeAcciones.Player.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        // Se desuscriben las acciones para evitar errores en memoria
        inputDeAcciones.Player.Move.performed -= OnMovePerformed;
        inputDeAcciones.Player.Move.canceled -= OnMoveCanceled;

        inputDeAcciones.Player.Run.performed -= OnRunPerformed;
        inputDeAcciones.Player.Run.canceled -= OnRunCanceled;

        inputDeAcciones.Player.Jump.performed -= OnJumpPerformed;

        inputDeAcciones.Player.Disable();
    }

    // Callbacks del nuevo Input System para procesar las teclas o sticks
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
        // Si ya estamos atravesando un obstáculo, ignoramos nuevas entradas
        if (SaltandoObstaculo) return;

        // 1. Intentamos ejecutar el salto de obstáculo primero
        if (IntentaSaltarObstaculo())
        {
            return; // Si detectó un muro, cortamos aquí sin ejecutar el salto físico tradicional
        }

        // 2. Si no hay ningún obstáculo enfrente y estamos en el suelo, salta normalmente
        if (EnSuelo)
        {
            rb.AddForce(Vector3.up * fuerzaDeSalto, ForceMode.Impulse);
            animator.SetTrigger("Salto");
        }
    }

    private bool IntentaSaltarObstaculo()
    {
        // Lanza un Raycast hacia adelante buscando objetos en la capa de obstáculos
        if (OrigenSaltoCkeck != null && Physics.Raycast(OrigenSaltoCkeck.position, transform.forward, out RaycastHit hit, saltoObsDistancia, obstacleLayer))
        {
            SaltandoObstaculo = true;
            SaltandoTimer = duracionAnimacion;

            // Calcula el punto objetivo sumando el offset en el eje Y para sobrepasar el muro
            puntoObstaculo = hit.point + (Vector3.up * offsetAlturaObstaculo);

            // Anula la gravedad y las fuerzas para dar control total a la animación
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;

            // Desactiva el Collider para evitar que choque mecánicamente contra la pared
            if (miCollider != null)
            {
                miCollider.enabled = false;
            }

            // Dispara el Trigger en la ventana del Animator
            animator.SetTrigger("SaltarObstaculo");
            return true;
        }

        return false;
    }

    private void FinalizarSaltoObstaculo()
    {
        SaltandoObstaculo = false;

        // Restablece la gravedad y activa nuevamente el Collider al tocar tierra
        rb.useGravity = true;

        if (miCollider != null)
        {
            miCollider.enabled = true;
        }
    }

    // Dibuja una guía cian en la pestaña Scene para visualizar la distancia del Raycast
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
        // Verifica mediante una esfera si el personaje está haciendo contacto con el suelo
        if (CkeckCapaSuelo != null)
        {
            EnSuelo = Physics.CheckSphere(CkeckCapaSuelo.position, distanciaSuelo, capaDelSuelo);
        }

        // Actualiza el estado Animator sobre si el personaje toca tierra
        animator.SetBool("EnSuelo", EnSuelo);

        // Control del Blend Tree de velocidad (0: Idle, 1: Walk, 2: Run)
        float targetAnimSpeed;

        if (inputDeMovimiento != Vector2.zero)
        {
            targetAnimSpeed = Corriendo ? 2f : 1f;
        }
        else
        {
            targetAnimSpeed = 0f;
        }

        // Transición suavizada del parámetro velocidad en el Animator
        animator.SetFloat("velocidad", targetAnimSpeed, 0.1f, Time.deltaTime);

        // Ajuste mediante MatchTarget durante la reproducción de la animación de Vault
        if (SaltandoObstaculo)
        {
            // Agregamos !animator.IsInTransition(0) para evitar llamar a MatchTarget durante el fundido
            if (!animator.IsInTransition(0) && !animator.isMatchingTarget && animator.GetCurrentAnimatorStateInfo(0).IsName("Vault1"))
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
        }
    
    }

    // =========================================================
    // BUCLE DE FÍSICAS (FIXED UPDATE)
    // =========================================================
    private void FixedUpdate()
    {
        // Si está haciendo la pirueta del obstáculo, se desactiva el movimiento por código
        if (SaltandoObstaculo) return;

        // Cálculo de velocidad final considerando el multiplicador de carrera
        float currentSpeed = velocidad * (Corriendo ? multiplicadorVelocidad : 1f);

        // Proyección de la dirección del personaje ajustada a la orientación de la cámara
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = (cameraForward * inputDeMovimiento.y + cameraRight * inputDeMovimiento.x).normalized;

        // Asignación de la velocidad lineal en el Rigidbody manteniendo el eje Y original
        Vector3 targetVelocity = direction * currentSpeed;
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);

        // Rotación progresiva del cuerpo mirando en la dirección del movimiento
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.fixedDeltaTime);
        }
    }
}