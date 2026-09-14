using UnityEngine;

public class MeteoriteMovement : MonoBehaviour
{
    [SerializeField] private GameObject building;
    [SerializeField] private GameObject meteorite;


    [SerializeField] int speed;

    [SerializeField] float health;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(building == null)
        {
            building = GameObject.FindGameObjectWithTag("building");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (building != null)
        {
            Vector3 direction = (building.transform.position - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * speed);
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


    private void OnCollisionEnter(Collision other)//para detectar colision y destruir enemigo - Recibe datos
    {
        if (other.gameObject.CompareTag("building"))//indica que objeto toca
        {

            Destroy(gameObject);
            //meteorite = null;
            Debug.Log("el meteorito hizo bum");
        }
    }
}


