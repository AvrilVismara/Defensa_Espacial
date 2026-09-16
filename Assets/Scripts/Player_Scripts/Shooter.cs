using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [Header("REFERENCIAS")]
    [SerializeField] private Transform puntoDisparo; // donde sale la bala
    [SerializeField] private GameObject prefabBala;
    [SerializeField] private InventarioJugador inventario; // ref al inventario
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask capasImpacto = ~0; // Capas que detecta el rayo de apuntado

    private float tiempoUltimoDisparo = 0f;
    private bool disparando = false; // Bandera de disparo continuo

    private PlayerMovement inputDeAcciones;

    private void Awake()
    {
        inputDeAcciones = new PlayerMovement();
    }

    private void OnEnable()
    {
        inputDeAcciones.Player.Enable();
        animator = GetComponent<Animator>();
        inputDeAcciones.Player.Shoot.performed += OnShootPerformed;
        inputDeAcciones.Player.Shoot.canceled += OnShootCanceled;
    }

    private void OnDisable()
    {
        inputDeAcciones.Player.Shoot.performed -= OnShootPerformed;
        inputDeAcciones.Player.Shoot.canceled -= OnShootCanceled;

        inputDeAcciones.Player.Disable();
    }

    private void OnShootPerformed(InputAction.CallbackContext context)
    {
        disparando = true;
    }

    private void OnShootCanceled(InputAction.CallbackContext context)
    {
        disparando = false;
        if (animator != null) animator.SetBool("IsShooting", false);
    }

    void Start()
    {

        if (inventario == null)
        {
            inventario = GetComponent<InventarioJugador>();
        }

        // Si no hay punto de disparo, usar la posición del jugador ---
        if (puntoDisparo == null)
        {
            puntoDisparo = transform;
        }
    }

    void Update()
    {
        // Disparo continuo mientras se mantiene presionado ---
        if (disparando)
        {
            IntentarDisparar();
        }
    }

    // Lógica de disparo ---
    private void IntentarDisparar()
    {
        if (inventario == null) return;

        Arma armaActual = inventario.ObtenerArmaActual();
        if (armaActual == null) return;

        // Verificar la cadencia del arma ---
        if (Time.time - tiempoUltimoDisparo < armaActual.Cadencia)
        {
            return;
        }

        // Intentar disparar (gasta munición) ---
        bool disparoExitoso = inventario.DispararArmaActual();

        if (disparoExitoso)
        {
            CrearBala(armaActual);
            tiempoUltimoDisparo = Time.time;
            if (animator != null) animator.SetBool("IsShooting", true);
        }
    }

    // Crear la bala orientada hacia el punto central de la vista de la cámara
    private void CrearBala(Arma arma)
    {
        if (prefabBala == null || Camera.main == null) return;

        // 1. Raycast desde el centro de la pantalla (punto focal de la cámara en 3D)
        Ray rayoCamara = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 puntoObjetivo;

        if (Physics.Raycast(rayoCamara, out RaycastHit hit, 200f, capasImpacto))
        {
            puntoObjetivo = hit.point; // Si el rayo choca contra algo, apuntamos ahí
        }
        else
        {
            puntoObjetivo = rayoCamara.GetPoint(200f); // Si apunta al aire, usamos un punto lejano
        }

        // 2. Calcular la dirección desde la punta del arma hasta el objetivo en 3D (incluye el ángulo vertical)
        Vector3 direccionDisparo = (puntoObjetivo - puntoDisparo.position).normalized;
        Quaternion rotacionBala = Quaternion.LookRotation(direccionDisparo);

        // 3. Instanciar la bala con la rotación inclinada real hacia la retícula
        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, rotacionBala);

        // Pasar el daño del arma a la bala ---
        Bullet componenteBala = bala.GetComponent<Bullet>();
        if (componenteBala != null)
        {
            componenteBala.Configurar(arma.Danio);
        }
    }
}