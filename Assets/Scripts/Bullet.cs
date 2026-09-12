using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("CONFIGURACIÓN DE LA BALA")]
    [SerializeField] private float velocidad = 20f; // Velocidad de la bala
    [SerializeField] private float tiempoVida = 3f; // Tiempo máximo de bala en pantalla
    [SerializeField] private float danio = 10f; // Daño al impactar

    private Rigidbody rb;
    private bool haImpactado = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();    // Obtener el Rigidbody y aplicar velocidad inicial ---
        rb.linearVelocity = transform.forward * velocidad;
        Destroy(gameObject, tiempoVida); // Destruir la bala después del tiempo de vida
    }

    // Colisiones ---
    private void OnTriggerEnter(Collider other)
    {
        if (haImpactado) return;
        if (other.CompareTag("Player")) return;
        if (other.CompareTag("Bullet")) return;

        haImpactado = true;

        // Aplicar daño (pendiente: conectar con vida del meteorito)
        Debug.Log($"Bullet impactó con {other.name} (Daño: {danio})");

        Destroy(gameObject);
    }

    // Método público para configurar el daño desde el script Shooter
    public void Configurar(float danioRecibido)
    {
        this.danio = danioRecibido;
    }
}