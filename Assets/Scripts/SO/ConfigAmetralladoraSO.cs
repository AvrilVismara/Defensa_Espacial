using UnityEngine;

[CreateAssetMenu(fileName = "NuevaAmetralladora", menuName = "Armas/Ametralladora")]
public class ConfigAmetralladoraSO : ScriptableObject
{
    [Header("CONFIGURACIÓN GENERAL")]
    public string nombreArma = "Ametralladora";
    public int municionMaxima = 100;
    public float cadencia = 0.1f;
    public float danio = 7f;

    [Header("CONFIGURACIÓN ARMA DE FUEGO")]
    public bool automatica = true;
    public float dispersion = 1.5f;

    [Header("CONFIGURACIÓN AMETRALLADORA")]
    public int balasPorRafaga = 5;
    public float dispersionAdicional = 20f;
    public float danioExtra = 0f;
    public float tiempoEnfriamiento = 0.5f;

    public Arma CrearArma()
    {
        return new Ametralladora(
            nombreArma,
            municionMaxima,
            cadencia,
            danio,
            automatica,
            dispersion,
            balasPorRafaga,
            dispersionAdicional,
            danioExtra,
            tiempoEnfriamiento
        );
    }
}