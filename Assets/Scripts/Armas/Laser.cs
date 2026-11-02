using UnityEngine;

[System.Serializable]
public class Laser : ArmaDeFuego
{
    [Header("CONFIGURACIÓN DE LÁSER")]
    public float longitudRayo = 100f;
    public float duracionRayo = 0.2f;
    public int consumoEnergiaPorDisparo = 2;
    public float danioExtra = 0f;

    public float LongitudRayo => longitudRayo;
    public float DuracionRayo => duracionRayo;
    public int ConsumoEnergiaPorDisparo => consumoEnergiaPorDisparo;
    public float DanioExtra => danioExtra;

    public Laser(string nombre, int municionMaxima, float cadencia, float danio,
                 bool automatica, float dispersion, float longitudRayo, float duracionRayo,
                 int consumoEnergiaPorDisparo, float danioExtra)
        : base(nombre, municionMaxima, cadencia, danio, automatica, dispersion)
    {
        this.longitudRayo = longitudRayo;
        this.duracionRayo = duracionRayo;
        this.consumoEnergiaPorDisparo = consumoEnergiaPorDisparo;
        this.danioExtra = danioExtra;
    }

    public override bool Disparar()
    {
        if (municionActual < consumoEnergiaPorDisparo)
        {
            Debug.Log($"{nombre}: ¡Energía insuficiente! ({municionActual}/{consumoEnergiaPorDisparo})");
            return false;
        }

        municionActual -= consumoEnergiaPorDisparo;
        float danioTotal = danio + danioExtra;
        Debug.Log($"{nombre}: ¡Disparo láser! Daño: {danioTotal}");
        return true;
    }

    public override string ObtenerInfo()
    {
        return $"{nombre}: {municionActual}/{municionMaxima} (Energía) - Daño: {danio + danioExtra}";
    }
}