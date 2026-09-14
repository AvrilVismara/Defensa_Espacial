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
    private void OnCollisionEnter(Collision collision)//para detectar colision y destruir enemigo - Recibe datos
    {
        if (collision.gameObject.CompareTag("meteorite"))//indica que objeto toca
        {
            health--;
            
        }
       // if (health <= 0)
       // {
          //  Destroy(gameObject);

       // }


    }

}

