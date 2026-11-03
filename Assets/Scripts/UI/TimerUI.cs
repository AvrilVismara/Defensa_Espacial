using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI messageText;

    [Header("Audio")]
    public AudioSource audioSourceVoice;
    public AudioSource audioSourceMusic;

    public AudioClip finalChallengeAudio;
    public AudioClip musicFinalChallenge;

    [Header("Timer")]
    public float tiempoRestante = 600f; // 10 minutos

    private bool contadorActivo = true;
    private bool audioFinalReproducido = false;

    void Start()
    {
        if (audioSourceVoice == null)
        {
            Debug.LogWarning("AudioSourceVoice no asignado.");
        }

        if (audioSourceMusic == null)
        {
            Debug.LogWarning("AudioSourceMusic no asignado.");
        }

        if (finalChallengeAudio == null)
        {
            Debug.LogError("No se asignó Final Challenge Audio.");
        }

        if (musicFinalChallenge == null)
        {
            Debug.LogWarning("No se asignó Music Final Challenge.");
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

            // Se activa cuando quedan 8 minutos (480 segundos)
            if (tiempoRestante <= 480f && !audioFinalReproducido)
            {
                audioFinalReproducido = true;

                messageText.text = "¡Defiendan la plaza central!";

                Debug.Log("DESAFÍO FINAL ACTIVADO");

                // Reproducir locución
                if (audioSourceVoice != null && finalChallengeAudio != null)
                {
                    audioSourceVoice.PlayOneShot(finalChallengeAudio);
                    Debug.Log("Locución reproducida");
                }
                else
                {
                    Debug.LogError("Falta AudioSourceVoice o FinalChallengeAudio");
                }

                // Reproducir música
                if (audioSourceMusic != null && musicFinalChallenge != null)
                {
                    audioSourceMusic.clip = musicFinalChallenge;
                    audioSourceMusic.Play();

                    Debug.Log("Música final reproducida");
                }
                else
                {
                    Debug.LogError("Falta AudioSourceMusic o MusicFinalChallenge");
                }
            }
        }
        else
        {
            tiempoRestante = 0f;
            timerText.text = "00:00";
            contadorActivo = false;

            messageText.text = "¡Tiempo terminado!";
            Debug.Log("¡Tiempo terminado!");
        }
    }
}