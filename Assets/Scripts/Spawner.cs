using System.Collections.Generic;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{

    [SerializeField] private GameObject meteorite;  // El objeto a instanciar

    [SerializeField] float timer = 0;
    [SerializeField] float timeToSpawn;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        timer += Time.deltaTime;

        if (timer > timeToSpawn)// para tener más de un spawner
        {
            SpawnMeteorite();

            timer = 0f;

        }
    }
    void SpawnMeteorite()
    {
        Instantiate(meteorite, transform.position, transform.rotation);

        //  int random = Random.Range(0, meteorites.Count);// permite agregar meteoritos desde el inspector

        //Instantiate(meteorites[random], transform.position, transform.rotation);

    }

}

