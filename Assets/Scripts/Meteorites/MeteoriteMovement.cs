using UnityEngine;

public class MeteoriteMovement : MonoBehaviour
{
    [SerializeField] private GameObject building;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float health = 30f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Configurar el Rigidbody para física correcta
        if (rb != null)
        {
            rb.useGravity = false; // Opcional, si no querés gravedad
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Para evitar tunneling
        }
    }

    public void AsignarObjetivo(GameObject nuevoObjetivo)
    {
        building = nuevoObjetivo;
    }

    void FixedUpdate()
    {
        if (building != null && rb != null)
        {
            Vector3 direction = (building.transform.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
        else if (building == null)
        {
            Destroy(gameObject);
        }
    }

    public void RecibirDanio(float cantidad)
    {
        health -= cantidad;
        if (health <= 0) Destruir();
    }

    private void Destruir()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        // Verificar si el objeto tiene el tag del edificio
        if (other.gameObject.CompareTag("building"))
        {
            Debug.Log($"Meteorito impactó con {other.gameObject.name}");
            Destroy(gameObject);
        }
    }
}