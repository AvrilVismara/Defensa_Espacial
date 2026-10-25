using UnityEngine;

[CreateAssetMenu(fileName = "NuevaPistola", menuName = "Armas/Pistola")]
public class ConfigPistolaSO : ScriptableObject
{
    [Header("CONFIGURACIÓN GENERAL")]
    public string nombreArma = "Pistola";
    public int municionMaxima = 15;
    public float cadencia = 0.5f;
    public float danio = 10f;

    [Header("CONFIGURACIÓN ARMA DE FUEGO")]
    public bool automatica = false;
    public float dispersion = 0.3f;

    public Arma CrearArma()
    {
        return new Pistola(
            nombreArma,
            municionMaxima,
            cadencia,
            danio,
            automatica,
            dispersion
        );
    }
}