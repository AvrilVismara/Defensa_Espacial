using UnityEngine;

[CreateAssetMenu(fileName = "NuevoLaser", menuName = "Armas/Laser")]
public class ConfigLaserSO : ScriptableObject
{
    [Header("CONFIGURACIÓN GENERAL")]
    public string nombreArma = "Láser";
    public int municionMaxima = 50;
    public float cadencia = 0.05f;
    public float danio = 15f;

    [Header("CONFIGURACIÓN ARMA DE FUEGO")]
    public bool automatica = true;
    public float dispersion = 0.1f;

    [Header("CONFIGURACIÓN LÁSER")]
    public float longitudRayo = 100f;
    public float duracionRayo = 0.2f;
    public int consumoEnergiaPorDisparo = 2;
    public float danioExtra = 0f;

    public Arma CrearArma()
    {
        return new Laser(
            nombreArma, municionMaxima, cadencia, danio,
            automatica, dispersion, longitudRayo, duracionRayo,
            consumoEnergiaPorDisparo, danioExtra
        );
    }
}