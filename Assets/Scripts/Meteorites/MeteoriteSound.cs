using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MeteoriteSound : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip meteoriteTrail;
    public AudioClip meteoriteExplotion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource.PlayOneShot(meteoriteTrail);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("building1")) || (collision.gameObject.CompareTag("building2")) || (collision.gameObject.CompareTag("building3")))
        {
            AudioSource.PlayOneShot(meteoriteExplotion);
            Debug.Log("Explotó Meteorito");
        }
    }
   
}
