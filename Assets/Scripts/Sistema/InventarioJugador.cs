using UnityEngine;
using System.Collections.Generic;

public class InventarioJugador : MonoBehaviour
{
    private List<Arma> armas = new List<Arma>();
    private int armaActual = -1;

    public event System.Action<Arma> OnArmaAgregada;
    public event System.Action<Arma> OnArmaRecargada;

    public void AgregarArma(Arma nuevaArma)
    {
        if (nuevaArma == null) return;

        // Buscar si ya tenemos esta arma
        Arma existente = armas.Find(a => a.Nombre == nuevaArma.Nombre);

        if (existente != null)
        {
            // Si ya existe, recargar
            existente.Recargar();
            OnArmaRecargada?.Invoke(existente);
            Debug.Log($"{existente.Nombre} recargada");
        }
        else
        {
            // Agregar nueva
            armas.Add(nuevaArma);
            if (armas.Count == 1) SeleccionarArma(0);
            OnArmaAgregada?.Invoke(nuevaArma);
            Debug.Log($"{nuevaArma.Nombre} añadida al inventario");
        }
    }

    public void RecargarArma(string nombreArma)
    {
        Arma arma = armas.Find(a => a.Nombre == nombreArma);
        if (arma != null)
        {
            arma.Recargar();
            OnArmaRecargada?.Invoke(arma);
        }
    }

    public bool TieneArma(string nombreArma)
    {
        return armas.Exists(a => a.Nombre == nombreArma);
    }

    public Arma ObtenerArmaPorNombre(string nombreArma)
    {
        return armas.Find(a => a.Nombre == nombreArma);
    }

    public void SeleccionarArma(int indice)
    {
        if (indice >= 0 && indice < armas.Count)
        {
            armaActual = indice;
            Debug.Log($"Arma seleccionada: {armas[armaActual].Nombre}");
        }
    }

    public bool DispararArmaActual()
    {
        if (armaActual < 0 || armaActual >= armas.Count)
        {
            Debug.Log("No hay arma seleccionada");
            return false;
        }
        return armas[armaActual].Disparar();
    }

    public Arma ObtenerArmaActual()
    {
        if (armaActual >= 0 && armaActual < armas.Count)
            return armas[armaActual];
        return null;
    }

    public List<Arma> ObtenerTodasLasArmas()
    {
        return new List<Arma>(armas);
    }
}