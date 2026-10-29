using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float alcanceMaximo = 100f;
    [SerializeField] private LayerMask capasImpacto = ~0; // Qué capas puede golpear
    [SerializeField] private Transform puntoDisparo; // Desde dónde sale el rayo

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false; // Empieza apagado
    }

    // Este método lo llamará tu Shooter cuando el jugador dispare
    public void DispararLaser(float danio)
    {
        lineRenderer.enabled = true;

        // 1. El rayo empieza en el punto de disparo
        lineRenderer.SetPosition(0, puntoDisparo.position);

        // 2. Lanzamos el Raycast
        Ray rayo = new Ray(puntoDisparo.position, puntoDisparo.forward);
        RaycastHit hit;

        if (Physics.Raycast(rayo, out hit, alcanceMaximo, capasImpacto))
        {
            // Si golpea algo, el rayo termina ahí
            lineRenderer.SetPosition(1, hit.point);

            // Aplicar daño si es un meteorito
            if (hit.collider.CompareTag("Meteorites"))
            {
                MeteoriteMovement meteorito = hit.collider.GetComponent<MeteoriteMovement>();
                if (meteorito != null)
                {
                    meteorito.RecibirDanio(danio);
                    Debug.Log($"Láser impactó en {hit.collider.name} (Daño: {danio})");
                }
            }
        }
        else
        {
            // Si no golpea nada, el rayo llega hasta el alcance máximo
            lineRenderer.SetPosition(1, puntoDisparo.position + puntoDisparo.forward * alcanceMaximo);
        }
    }

    // Para apagar el rayo (ej: cuando el jugador suelta el botón)
    public void ApagarLaser()
    {
        lineRenderer.enabled = false;
    }
}