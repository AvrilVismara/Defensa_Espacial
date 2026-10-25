using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [Header("REFERENCIAS")]
    [SerializeField] private Transform puntoDisparo; // donde sale la bala
    [SerializeField] private GameObject prefabBala;
    [SerializeField] private InventarioJugador inventario; // ref al inventario

    private float tiempoUltimoDisparo = 0f;
    private bool disparando = false;// Bandera de disparo continuo

    private PlayerMovement inputDeAcciones;

    private void Awake()
    {
        inputDeAcciones = new PlayerMovement();
    }

    private void OnEnable()
    {
        inputDeAcciones.Player.Enable();

        // Suscripción a eventos del Input System ---
        inputDeAcciones.Player.Shoot.performed += OnShootPerformed;
        inputDeAcciones.Player.Shoot.canceled += OnShootCanceled;
    }

    private void OnDisable()
    {
        // Desuscripción para evitar errores ---
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
    }

    void Start()
    {
        // Obtener el inventario si no está asignado ---
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
        }
    }

    // Crear la bala en el punto de disparo ---
    private void CrearBala(Arma arma)
    {
        if (prefabBala == null) return;

        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, puntoDisparo.rotation);

        // Pasar el daño del arma a la bala ---
        Bullet componenteBala = bala.GetComponent<Bullet>();
        if (componenteBala != null)
        {
            componenteBala.Configurar(arma.Danio);
        }
    }
}