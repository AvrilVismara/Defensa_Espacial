using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Shooter : MonoBehaviour
{
    [Header("REFERENCIAS")]
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private GameObject prefabBala;
    [SerializeField] private InventarioJugador inventario;
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask capasImpacto = ~0;
    [SerializeField] private LaserBeam laserBeam;

    private float tiempoUltimoDisparo = 0f;
    private bool disparando = false;

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

        if (laserBeam != null) laserBeam.ApagarLaser();
    }

    void Start()
    {
        if (inventario == null) inventario = GetComponent<InventarioJugador>();
        if (puntoDisparo == null) puntoDisparo = transform;
    }

    void Update()
    {
        if (disparando) IntentarDisparar();
    }

    private void IntentarDisparar()
    {
        if (inventario == null) return;

        Arma armaActual = inventario.ObtenerArmaActual();
        if (armaActual == null) return;

        if (Time.time - tiempoUltimoDisparo < armaActual.Cadencia) return;

        bool disparoExitoso = inventario.DispararArmaActual();

        if (disparoExitoso)
        {
            // ===== DIAGNÓSTICO =====
            Debug.Log($"🔍 Tipo real del arma: {armaActual.GetType().Name}");
            Debug.Log($"🔍 ¿Es Laser? {armaActual is Laser}");
            Debug.Log($"🔍 ¿laserBeam es null? {laserBeam == null}");
            // =======================

            // ← NUEVO: Si el arma es un Láser, usar el LineRenderer
            if (armaActual is Laser)
            {
                if (laserBeam != null)
                {
                    laserBeam.DispararLaser(armaActual.Danio);
                }
            }
            else
            {
                // Para las demás armas, crear bala
                CrearBala(armaActual);
            }

            tiempoUltimoDisparo = Time.time;
            if (animator != null) animator.SetBool("IsShooting", true);
        }
        else
        {
            // ← NUEVO: Si el disparo falla (sin munición), apagar el láser
            if (armaActual is Laser && laserBeam != null)
            {
                laserBeam.ApagarLaser();
            }
        }
    }

    private void CrearBala(Arma arma)
    {
        if (prefabBala == null || Camera.main == null) return;

        Ray rayoCamara = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 puntoObjetivo;

        if (Physics.Raycast(rayoCamara, out RaycastHit hit, 200f, capasImpacto))
            puntoObjetivo = hit.point;
        else
            puntoObjetivo = rayoCamara.GetPoint(200f);

        Vector3 direccionDisparo = (puntoObjetivo - puntoDisparo.position).normalized;
        Quaternion rotacionBala = Quaternion.LookRotation(direccionDisparo);

        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, rotacionBala);

        Bullet componenteBala = bala.GetComponent<Bullet>();
        if (componenteBala != null) componenteBala.Configurar(arma.Danio);
    }
}