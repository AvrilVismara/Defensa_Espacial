using UnityEngine;
[RequireComponent(typeof(MeshCollider))]

public class SFXReloadedWeapon : MonoBehaviour
{
    public AudioSource AudioSource;
  
   
    private bool dentro;

    private void Reset() { GetComponent<MeshCollider>().isTrigger = true; }

    private void OnTriggerEnter(Collider otro)
    {
        if (!otro.CompareTag("Player") || dentro) return;
        dentro = true;
        if (AudioSource != null && !AudioSource.isPlaying) AudioSource.Play();
        
        Debug.Log("Jugador: Recargo arma", this);
    }

    private void OnTriggerExit(Collider otro)
    {
        if (!otro.CompareTag("Player") || !dentro) return;
        dentro = false;
        if (AudioSource != null) AudioSource.Stop();

        Debug.Log("Jugador: Salio del punto de recarga", this);
    }

    private void OnDisable()
    {
        if (AudioSource != null) AudioSource.Stop();
       
        dentro = false;
    }
}
