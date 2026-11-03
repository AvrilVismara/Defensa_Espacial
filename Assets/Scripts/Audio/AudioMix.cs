using UnityEngine;

using UnityEngine.Audio;
using System.Collections;


public class AudioMix : MonoBehaviour
{
    [Header("Snapshots")]
    public AudioMixerSnapshot locucionInicio;
    public AudioMixerSnapshot normal;
    public AudioMixerSnapshot desafioFinal;
    [Min(0f)] public float transicion = 2f;

    public void LocucionInicio()
    {
        if (locucionInicio != null)
            locucionInicio.TransitionTo(transicion);
    }

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
        // Inicia snapshot locucionInicio
       LocucionInicio();

        // Corrutina para controlar tiempo
        StartCoroutine(SecuenciaDeSnapshots());
    }
    private IEnumerator SecuenciaDeSnapshots()
    {
        // Tiempo de espera
        yield return new WaitForSecondsRealtime(30f);


        // Cambia a snapshot normal
        Debug.Log("Ejecutando Normal()");
        Normal();

        // Tiempo espera
        Debug.Log(" Esperando 90s para cambiar a Desafio Final...");
        yield return new WaitForSecondsRealtime(91f);

        // Inicia snapshot desafio final
        Debug.Log("Ejecutando DesafioFinal()");
        DesafioFinal();

        yield return new WaitForSecondsRealtime(18f);

        Normal();
    }
}
