using UnityEngine;

public class SFXGunShoot : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sonidoDisparo;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic izquierdo
        {
            audioSource.PlayOneShot(sonidoDisparo);
        }
    }
   
}
