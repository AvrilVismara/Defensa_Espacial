using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI messageText;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip finalChallengeAudio;

    [Header("Timer")]
    public float tiempoRestante = 600f; // 10 minutos

    private bool contadorActivo = true;
    private bool audioFinalReproducido = false;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = FindFirstObjectByType<AudioSource>();
        }

        if (audioSource == null)
        {
            Debug.LogError("No se encontró ningún AudioSource en la escena.");
        }

        if (finalChallengeAudio == null)
        {
            Debug.LogError("No se asignó Final Challenge Audio en el Inspector.");
        }
    }

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

            // Evento al llegar a 540 segundos
            if (tiempoRestante <= 540f && !audioFinalReproducido)
            {
                audioFinalReproducido = true;

                messageText.text = "¡Defiendan la plaza central!";

                Debug.Log("DESAFÍO FINAL ACTIVADO");

                if (audioSource != null && finalChallengeAudio != null)
                {
                    audioSource.clip = finalChallengeAudio;
                    audioSource.Play();

                    Debug.Log("Reproduciendo audio final");
                }
                else
                {
                    Debug.LogError("Falta AudioSource o AudioClip");
                }
            }
        }
        else
        {
            tiempoRestante = 0;
            timerText.text = "00:00";
            contadorActivo = false;

            Debug.Log("¡Tiempo terminado!");
        }
    }
}