using UnityEngine;
using UnityEngine.UI;
public class HealthController : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private HealthBuilding healthBuilding;
   
    //[SerializeField] private float speed = 1f;     
   

    private void Start()
    {
        if (healthBuilding != null)
            healthBuilding.OnDamage += UpdateBar;

    }
    private void OnDestroy()
    {
        if (healthBuilding != null)
        {
            healthBuilding.OnDamage -= UpdateBar;
        }
    }

    public void UpdateBar(float currentHealth, float maxHealth)
    {
        if (healthBar != null && maxHealth > 0)
        {
            // Calcula un valor float entre 0.0 y 1.0
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

}
