using UnityEngine;

using UnityEngine.Audio;
using System.Collections;


public class AudioMix : MonoBehaviour
{
    [Header("Snapshots")]
    public AudioMixerSnapshot normal;
    public AudioMixerSnapshot desafioFinal;
    [Min(0f)] public float transicion = 1f;

    public void Normal() 
    
    {
        if (normal != null)

            normal.TransitionTo(transicion);
    }

    public void DesafioFinal() 
    
    {
        if (desafioFinal != null)

            desafioFinal.TransitionTo(transicion); 
    }

  
    private void Start()
    {
        // Comienza el snapshot normal
        Normal();

        // Comienza corrutina para esperar (120 segundos)
        StartCoroutine(EsperarYCambiarSnapshot());
    }

    private IEnumerator EsperarYCambiarSnapshot()
    {
        // Espera los 120 segundos
        yield return new WaitForSeconds(120f);

        // Se activa el snapshot de desafío final 
        DesafioFinal();
    }
}
