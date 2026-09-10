using UnityEngine;

public class MeteoriteMovement : MonoBehaviour
{
    [SerializeField] float timer;
    [SerializeField] private GameObject building;
    [SerializeField] private GameObject meteorite;


    [SerializeField] int speed;

    [SerializeField] float health;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       if (building != null)
       {
            Vector3 direction = (building.transform.position - transform.position).normalized;
            transform.Translate(direction * Time.deltaTime * speed);
       }
       
    }
    private void OnCollisionEnter(Collision collision)//para detectar colision y destruir enemigo - Recibe datos
    {
        if (collision.gameObject.CompareTag("building"))//indica que objeto toca
        {

            Destroy(gameObject);
            //meteorite = null;
        }



    }

}


