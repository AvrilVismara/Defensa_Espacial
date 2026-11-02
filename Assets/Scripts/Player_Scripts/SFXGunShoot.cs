using UnityEngine;

public class SFXGunShoot : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip sonidoDisparo;

    public InventarioJugador inventario;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Clic izquierdo
        {
            if (inventario != null && inventario.DispararArmaActual())
            {
               audioSource.PlayOneShot(sonidoDisparo);
            }
        }
    }
   
}
