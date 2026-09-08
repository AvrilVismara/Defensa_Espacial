using System.Collections.Generic;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{

    [SerializeField] private List<GameObject> meteorites = new List<GameObject>();
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

            timer = 0;

        }
    }
    void SpawnMeteorite()
    {
        if (meteorites.Count > 1)
        {
            int ramdon = Random.Range(0, meteorites.Count);// permite agregar meteoritos desde el inspector

            Instantiate(meteorites[ramdon], transform.position, transform.rotation);
        }
    }

}
