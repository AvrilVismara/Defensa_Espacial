using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    public TextMeshProUGUI messageText;

    private float tiempoRestante = 600f; // 10 minutos = 600 segundos
    private bool contadorActivo = true;

    void Update()
    {
        if (!contadorActivo)
            return;

        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;

            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);

            timerText.text = string.Format("{0:00}:{1:00}", minutos, segundos);

            if (tiempoRestante < 480f)
            {
                messageText.text = "¡Defiendan la plaza central!";
                Debug.Log("Desafío final");
            }
        }
        else
        {
            tiempoRestante = 0;
            timerText.text = "00:00";
            contadorActivo = false;

            Debug.Log("¡Tiempo terminado!");

            // Pantalla de Derrota
        }
    }
}
