using UnityEngine;

public abstract class Arma
{
    // Variables protegidas para que las hijas puedan acceder ---
    protected string nombre;
    protected int municionMaxima;
    protected int municionActual;
    protected float cadencia;
    protected float danio;

    // Encapsulamiento para acceso externo ---
    public string Nombre => nombre;
    public int MunicionMaxima => municionMaxima;
    public int MunicionActual => municionActual;
    public float Cadencia => cadencia;
    public float Danio => danio;

    public Arma(string nombre, int municionMaxima, float cadencia, float danio)
    {
        this.nombre = nombre;
        this.municionMaxima = municionMaxima;
        this.municionActual = municionMaxima;
        this.cadencia = cadencia;
        this.danio = danio;
    }

    // Método virtual para que las hijas se puedan personalizar ---
    public virtual bool Disparar()
    {
        if (municionActual <= 0)
        {
            Debug.Log($"{nombre}: ¡Sin munición!");
            return false;
        }

        municionActual--;
        Debug.Log($"{nombre}: ¡Disparo! {municionActual}/{municionMaxima}");
        return true;
    }

    public virtual void Recargar()
    {
        int municionAntes = municionActual;
        municionActual = municionMaxima;
        Debug.Log($"{nombre}: Recargada ({municionAntes} → {municionActual})");
    }

    // Munición disponible ---
    public bool TieneMunicion()
    {
        return municionActual > 0;
    }
    public float PorcentajeMunicion()
    {
        return (float)municionActual / municionMaxima;
    }

    public virtual string ObtenerInfo()
    {
        return $"{nombre}: {municionActual}/{municionMaxima} (Daño: {danio})";
    }
}
