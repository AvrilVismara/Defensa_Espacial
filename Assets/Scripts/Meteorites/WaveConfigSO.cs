using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NuevaOleada", menuName = "Oleadas/ConfiguracionOleada")]
public class WaveConfigSO : ScriptableObject
{
    [Header("INFORMACIÓN DE LA OLEADA")]
    public string nombreOleada = "Oleada 1";
    public int numeroOleada = 1;

    [Header("SPAWNER DE ESTA OLEADA")]
    [Tooltip("Índice del Spawner en la lista del WaveManager (0, 1, 2...)")]
    public int indiceSpawner = 0;

    [Header("TIEMPO")]
    public float tiempoPreparacion = 5f;
    public float intervaloAparicion = 2f;

    [Header("METEORITOS")]
    public List<GameObject> prefabsMeteoritos = new List<GameObject>();
    public int cantidadMeteoritos = 10;

    [Header("ARMAS DESBLOQUEADAS")]
    public List<string> armasDesbloqueadas = new List<string>();
}