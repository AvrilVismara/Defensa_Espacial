using UnityEngine;

public class MeteoriteMovement : MonoBehaviour
{
    [SerializeField] private GameObject building;
    [SerializeField] private float speed;
    [SerializeField] private float health;
    public float Health => health;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        if (building == null)
            building = GameObject.FindGameObjectWithTag("building");
    }

    public void AsignarObjetivo(GameObject nuevoObjetivo)
    {
        building = nuevoObjetivo;
    }

    //Configurar desde la oleada
    public void ConfigurarDesdeOleada(float vida, float velocidad)
    {
        this.health = vida;
        this.speed = velocidad;
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
        if (other.gameObject.layer == LayerMask.NameToLayer("buildings"))
        {
            Debug.Log($"Meteorito impactó con {other.gameObject.name}");
            Destroy(gameObject);
        }
    }
}