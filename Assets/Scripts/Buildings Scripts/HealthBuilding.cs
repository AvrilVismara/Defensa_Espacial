using System;
using UnityEngine;

public class HealthBuilding : MonoBehaviour
{
    [SerializeField] private GameObject meteorite;
    [SerializeField] private int health;
    private int maxHealth;
    public event Action<float, float> OnDamage;

    private void Start()
    {
        maxHealth = health; // Guardamos la salud inicial
    }

    private void OnCollisionEnter(Collision other)//para detectar colision y destruir enemigo - Recibe datos
    {
        if (other.gameObject.CompareTag("Meteorites"))//indica que objeto toca

        {
            health--;
            // Invocamos el evento enviando la salud actual y la máxima
            OnDamage?.Invoke(health, maxHealth);

            Debug.Log("El edificio recibió daño. Salud actual: " + health);
        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }


    }

}

