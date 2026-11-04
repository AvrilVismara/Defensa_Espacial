using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MeteoriteSound : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip meteoriteTrail;
    public AudioClip meteoriteExplotion;

    [Header("Variación de Pitch y Volumen")]
    [Range(0.8f, 1.2f)] public float minPitch = 0.9f;
    [Range(0.8f, 1.2f)] public float maxPitch = 1.1f;

    [Range(0.7f, 1f)] public float minVolume = 0.9f;
    [Range(1f, 1.3f)] public float maxVolume = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource.pitch = Random.Range(minPitch, maxPitch);
        AudioSource.volume = Random.Range(minVolume, maxVolume);
        AudioSource.PlayOneShot(meteoriteTrail);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("building1")) || (collision.gameObject.CompareTag("building2")) || (collision.gameObject.CompareTag("building3")))
        {
            AudioSource.pitch = Random.Range(minPitch, maxPitch);
            AudioSource.volume = Random.Range(minVolume, maxVolume);
            AudioSource.PlayOneShot(meteoriteExplotion);
            Debug.Log("Explotó Meteorito");
        }
    }
   
}
