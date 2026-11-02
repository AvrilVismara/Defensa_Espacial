using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float alcanceMaximo = 100f;
    [SerializeField] private LayerMask capasImpacto = ~0;
    [SerializeField] private Transform puntoDisparo; // ← Asignar el shootingPoint

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    public void DispararLaser(float danio)
    {
        if (lineRenderer == null || puntoDisparo == null || Camera.main == null) return;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, puntoDisparo.position);

        // Calcular la dirección desde el centro de la pantalla (la mira)
        Ray rayoCamara = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 puntoObjetivo;

        if (Physics.Raycast(rayoCamara, out RaycastHit hitCamara, alcanceMaximo, capasImpacto))
            puntoObjetivo = hitCamara.point;
        else
            puntoObjetivo = rayoCamara.GetPoint(alcanceMaximo);

        Vector3 direccion = (puntoObjetivo - puntoDisparo.position).normalized;

        if (Physics.Raycast(puntoDisparo.position, direccion, out RaycastHit hit, alcanceMaximo, capasImpacto))
        {
            lineRenderer.SetPosition(1, hit.point);

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
            lineRenderer.SetPosition(1, puntoDisparo.position + direccion * alcanceMaximo);
        }
    }

    public void ApagarLaser()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;
    }
}