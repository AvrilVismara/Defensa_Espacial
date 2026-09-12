using UnityEngine;

public class Shooter : MonoBehaviour
{
    [Header("REFERENCIAS")]
    [SerializeField] private Transform puntoDisparo; // donde sale la bala
    [SerializeField] private GameObject prefabBala;
    [SerializeField] private InventarioJugador inventario; // ref al inventario

    private float tiempoUltimoDisparo = 0f;

    void Start()
    {
        // Obtener el inventario si no está asignado
        if (inventario == null)
        {
            inventario = GetComponent<InventarioJugador>();
        }

        // Si no se asignó punto de disparo, usar la posición del jugador
        if (puntoDisparo == null)
        {
            puntoDisparo = transform;
        }
    }

    void Update()
    {
        // Disparar con clic izquierdo (mantener presionado para automáticas)
        if (Input.GetMouseButton(0))
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

        // Verificar la cadencia del arma
        if (Time.time - tiempoUltimoDisparo < armaActual.Cadencia)
        {
            return;
        }

        // Intentar disparar - gasta munición
        bool disparoExitoso = inventario.DispararArmaActual();

        if (disparoExitoso)
        {
            CrearBala(armaActual);
            tiempoUltimoDisparo = Time.time;
        }
    }

    private void CrearBala(Arma arma)
    {
        if (prefabBala == null) return;

        // Instanciar la bala en el punto de disparo
        GameObject bala = Instantiate(prefabBala, puntoDisparo.position, puntoDisparo.rotation);

        // Configurar la bala con el daño del arma
        Bullet componenteBala = bala.GetComponent<Bullet>();
        if (componenteBala != null)
        {
            componenteBala.Configurar(arma.Danio);
        }
    }
}