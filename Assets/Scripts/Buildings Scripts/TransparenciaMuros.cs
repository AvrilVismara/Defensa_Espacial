using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparenciaMuros : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform objetivoJugador; // Asigná el Transform del Jugador
    [SerializeField] private LayerMask capaMuros;        // Capas de los muros (ej: Default, Obstacle)

    [Header("Ajustes de Transparencia")]
    [Range(0f, 1f)]
    [SerializeField] private float alfaTransparente = 0.3f; // Nivel de opacidad (0 = invisible, 1 = opaco)
    [SerializeField] private float velocidadSuavizado = 10f;

    private List<Renderer> murosActualesTransparentes = new List<Renderer>();
    private List<Renderer> murosAnterioresTransparentes = new List<Renderer>();

    private void LateUpdate()
    {
        if (objetivoJugador == null) return;

        // Limpiamos la lista de muros detectados en este frame
        murosActualesTransparentes.Clear();

        Vector3 direccion = objetivoJugador.position - transform.position;
        float distancia = direccion.magnitude;

        // Lanzamos un Raycast desde la cámara hacia el jugador
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direccion.normalized, distancia, capaMuros);

        foreach (RaycastHit hit in hits)
        {
            // Verificamos que no sea el propio jugador
            if (hit.collider.transform != objetivoJugador && !hit.collider.transform.IsChildOf(objetivoJugador))
            {
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null)
                {
                    murosActualesTransparentes.Add(rend);
                    AplicarTransparencia(rend, alfaTransparente);
                }
            }
        }

        // Restaurar la opacidad de los muros que ya no se interponen
        foreach (Renderer rend in murosAnterioresTransparentes)
        {
            if (!murosActualesTransparentes.Contains(rend) && rend != null)
            {
                RestaurarOpacidad(rend);
            }
        }

        // Actualizamos la lista previa
        murosAnterioresTransparentes = new List<Renderer>(murosActualesTransparentes);
    }

    private void AplicarTransparencia(Renderer rend, float objetivoAlfa)
    {
        foreach (Material mat in rend.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color colorActual = mat.color;
                colorActual.a = Mathf.Lerp(colorActual.a, objetivoAlfa, Time.deltaTime * velocidadSuavizado);
                mat.color = colorActual;
            }
            // Soporte para shaders URP / HDRP (propiedad _BaseColor)
            else if (mat.HasProperty("_BaseColor"))
            {
                Color colorActual = mat.GetColor("_BaseColor");
                colorActual.a = Mathf.Lerp(colorActual.a, objetivoAlfa, Time.deltaTime * velocidadSuavizado);
                mat.SetColor("_BaseColor", colorActual);
            }
        }
    }

    private void RestaurarOpacidad(Renderer rend)
    {
        foreach (Material mat in rend.materials)
        {
            if (mat.HasProperty("_Color"))
            {
                Color colorActual = mat.color;
                colorActual.a = Mathf.Lerp(colorActual.a, 1f, Time.deltaTime * velocidadSuavizado);
                mat.color = colorActual;
            }
            else if (mat.HasProperty("_BaseColor"))
            {
                Color colorActual = mat.GetColor("_BaseColor");
                colorActual.a = Mathf.Lerp(colorActual.a, 1f, Time.deltaTime * velocidadSuavizado);
                mat.SetColor("_BaseColor", colorActual);
            }
        }
    }
}