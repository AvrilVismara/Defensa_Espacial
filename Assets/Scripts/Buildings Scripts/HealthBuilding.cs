using UnityEngine;

public class HealthBuilding : MonoBehaviour
{
    [SerializeField] private GameObject meteorite;
    [SerializeField] private int health;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
    }
    private void OnCollisionEnter(Collision other)//para detectar colision y destruir enemigo - Recibe datos
    {
        if (other.gameObject.CompareTag("meteorite"))//indica que objeto toca
        {
            health--;
            Debug.Log("El edificio recibio daño");
            
        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }


    }

}

