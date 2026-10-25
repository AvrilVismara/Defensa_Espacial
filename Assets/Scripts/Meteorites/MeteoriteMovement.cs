using UnityEngine;

// Esta línea asegura que Unity te avise si olvidás ponerle un Rigidbody al meteorito
[RequireComponent(typeof(Rigidbody))]
public class MeteoriteMovement : MonoBehaviour
{
    public GameObject building;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float health = 10f;

    private Rigidbody rb;

    private void Start()
    {
        // Obtenemos la referencia al Rigidbody apenas nace el meteorito
        rb = GetComponent<Rigidbody>();
    }

    public void AsignarObjetivo(GameObject nuevoObjetivo)
    {
        building = nuevoObjetivo;
    }

    // Para físicas SIEMPRE usamos FixedUpdate en lugar de Update
    void FixedUpdate()
    {
        if (building != null)
        {
            // Calculamos la dirección
            Vector3 direction = (building.transform.position - transform.position).normalized;

            // Calculamos cuál será la próxima posición usando Time.fixedDeltaTime
            Vector3 nuevaPosicion = transform.position + direction * speed * Time.fixedDeltaTime;

            // Movemos el Rigidbody hacia esa posición calculando colisiones en el camino
            rb.MovePosition(nuevaPosicion);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RecibirDanio(float cantidad)
    {
        health -= cantidad;

        if (health <= 0)
        {
            Destruir();
        }
    }

    private void Destruir()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detectado contra: " + other.gameObject.name);

        if (other.CompareTag("building"))
        {
            Destroy(gameObject);
            Debug.Log("Meteoro Destruido");
        }
    }
}