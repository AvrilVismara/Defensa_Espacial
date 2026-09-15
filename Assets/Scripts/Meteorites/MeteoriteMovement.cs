using UnityEngine;

public class MeteoriteMovement : MonoBehaviour
{
    [SerializeField] private GameObject building;
    [SerializeField] private GameObject meteorite;

    [SerializeField] private int speed;
    [SerializeField] private float health;

    // Método público para que el Spawner le asigne su objetivo específico
    public void AsignarObjetivo(GameObject nuevoObjetivo)
    {
        building = nuevoObjetivo;
    }

    // Update is called once per frame
    void Update()
    {
        if (building != null)
        {
            Vector3 direction = (building.transform.position - transform.position).normalized;

            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Recibir daño de las balas ---
    public void RecibirDanio(float cantidad)
    {
        health -= cantidad;

        if (health <= 0)
        {
            Destruir();
        }
    }

    // Destruir por balas ---
    private void Destruir()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        // Detecta si choca contra cualquier edificio que empiece con "building" 
        // o puedes usar other.gameObject.CompareTag(building.tag) para asegurarte que colisiona con su blanco exacto.
        if (other.gameObject.tag.StartsWith("building"))
        {
            Destroy(gameObject);
        }
    }
}