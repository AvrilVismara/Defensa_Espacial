using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class Spawn : MonoBehaviour
{
    [SerializeField] private List<GameObject> meteorites = new List<GameObject>();
    [SerializeField] float Timer = 0;

    void Update()
    {
        Timer += Time.deltaTime;
        if(Timer > 20) 
        {
            SpawnMeteorite();
            Timer = 0;
        }
    }

    void SpawnMeteorite()
    {
        int random = Random.Range(0,3);
        Instantiate(meteorites[random], transform.position,transform.rotation);
    }
}
