using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HealthBuilding : MonoBehaviour
{
    [SerializeField] private float health;
    private MeteoriteMovement meteorite;
    private float maxHealth;
    public event Action<float, float> OnDamage;

    private void Start()
    {
        maxHealth = health; // Guardamos la salud inicial
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Meteorites"))
        {
            // Obtenemos el script del meteorito específico que acaba de chocar
            MeteoriteMovement meteorite = other.gameObject.GetComponent<MeteoriteMovement>();

            if (meteorite != null)
            {
                // Asegurate de que la variable Health sea pública en el script del meteorito
                float danorecibido = meteorite.Health;

                health -= danorecibido;

                // Corregimos la lógica: se destruye si la vida cae a 0 o menos
                if (health <= 0)
                {
                    Destroy(gameObject);
                }

                OnDamage?.Invoke(danorecibido, maxHealth);

                Debug.Log("El edificio recibió daño. Salud actual: " + health);
            }
        }
    }
}