using UnityEngine;

[System.Serializable]
public class ArmaDeFuego : Arma
{
    public bool automatica;
    public float dispersion;

    public bool EsAutomatica => automatica;
    public float Dispersion => dispersion;

    public ArmaDeFuego(string nombre, int municionMaxima, float cadencia, float danio, bool automatica = false, float dispersion = 0.5f)
        : base(nombre, municionMaxima, cadencia, danio)
    {
        this.automatica = automatica;
        this.dispersion = dispersion;
    }

    public virtual void PatronDisparo()
    {
        Debug.Log($"Disparo base de {nombre}");
    }
}

// PISTOLA ---
[System.Serializable]
public class Pistola : ArmaDeFuego
{
    // Constructor ---
    public Pistola(string nombre, int municionMaxima, float cadencia, float danio, bool automatica, float dispersion)
        : base(nombre, municionMaxima, cadencia, danio, automatica, dispersion) { }

    public override void PatronDisparo()
    {
        Debug.Log($"{nombre}: Disparo preciso y controlado");
    }

    public override string ObtenerInfo()
    {
        return $"{nombre}: {municionActual}/{municionMaxima} (Daño: {danio})";
    }
}


// AMETRALLADORA ---
[System.Serializable]
public class Ametralladora : ArmaDeFuego
{
    public int balasPorRafaga;
    public float dispersionAdicional;
    public float danioExtra;
    public float tiempoEnfriamiento;

    public int BalasPorRafaga => balasPorRafaga;
    public float DispersionAdicional => dispersionAdicional;
    public float DanioExtra => danioExtra;
    public float TiempoEnfriamiento => tiempoEnfriamiento;

    // Constructor ---
    public Ametralladora(string nombre, int municionMaxima, float cadencia, float danio, bool automatica, float dispersion, int balasRafaga, float dispersionAdicional, float danioExtra, float tiempoEnfriamiento)
        : base(nombre, municionMaxima, cadencia, danio, automatica, dispersion)
    {
        this.balasPorRafaga = balasRafaga;
        this.dispersionAdicional = dispersionAdicional;
        this.danioExtra = danioExtra;
        this.tiempoEnfriamiento = tiempoEnfriamiento;
    }

    public override bool Disparar()
    {
        if (municionActual <= 0)
        {
            Debug.Log($"{nombre}: ¡Sin munición!");
            return false;
        }

        if (municionActual < balasPorRafaga)
        {
            Debug.Log($"{nombre}: No hay suficientes balas para ráfaga ({municionActual}/{balasPorRafaga})");
            return false;
        }

        for (int i = 0; i < balasPorRafaga; i++)
        {
            municionActual--;
            Debug.Log($"{nombre}: Bala {i + 1}/{balasPorRafaga} disparada");
        }

        float danioTotal = danio + danioExtra;
        Debug.Log($"{nombre}: Ráfaga de {balasPorRafaga} balas! Daño total: {danioTotal * balasPorRafaga}");
        return true;
    }

    public override void PatronDisparo()
    {
        Debug.Log($"{nombre}: Disparo en ráfaga de {balasPorRafaga} balas (Dispersión: {dispersionAdicional}%)");
    }

    public void CambiarRafaga(int nuevaCantidad)
    {
        if (nuevaCantidad > 0 && nuevaCantidad <= municionMaxima)
        {
            balasPorRafaga = nuevaCantidad;
            Debug.Log($"Ráfaga cambiada a {balasPorRafaga} balas");
        }
    }

    public override string ObtenerInfo()
    {
        return $"{nombre}: {municionActual}/{municionMaxima} (Ráfaga: {balasPorRafaga})";
    }
}