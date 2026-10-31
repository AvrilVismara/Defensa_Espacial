using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instancia { get; private set; }

    [Header("CONFIGURACIÓN DE OLEADAS")]
    [SerializeField] private List<WaveConfigSO> oleadas = new List<WaveConfigSO>();

    [Header("REFERENCIAS")]
    [Tooltip("Lista de todos los Spawners de la escena (índice 0, 1, 2...)")]
    [SerializeField] private List<Spawn> spawners = new List<Spawn>();

    [Header("ESTADO ACTUAL")]
    [SerializeField] private int oleadaActualIndex = 0;

    private HashSet<string> armasDesbloqueadas = new HashSet<string>();

    // Bandera para saber si la oleada actual terminó
    private bool oleadaTerminada = false;

    public int OleadaActual => oleadaActualIndex + 1;

    private void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(CicloOleadas());
    }

    public bool EstaDesbloqueada(string nombreArma)
    {
        return armasDesbloqueadas.Contains(nombreArma);
    }

    private IEnumerator CicloOleadas()
    {
        for (int i = 0; i < oleadas.Count; i++)
        {
            WaveConfigSO oleada = oleadas[i];
            oleadaActualIndex = i;

            Debug.Log($"Preparando {oleada.nombreOleada}...");

            // Desbloquear armas de esta oleada
            foreach (string arma in oleada.armasDesbloqueadas)
            {
                armasDesbloqueadas.Add(arma);
                Debug.Log($"Arma desbloqueada: {arma}");
            }

            yield return new WaitForSeconds(oleada.tiempoPreparacion);

            Debug.Log($"{oleada.nombreOleada} iniciada!");

            // Obtener el Spawner correspondiente a esta oleada
            Spawn spawnerActual = ObtenerSpawner(oleada.indiceSpawner);

            if (spawnerActual == null)
            {
                Debug.LogError($"No se encontró el Spawner con índice {oleada.indiceSpawner}");
                continue;
            }

            // Reiniciar la bandera
            oleadaTerminada = false;

            // Suscribirse con un método con nombre
            spawnerActual.OnOleadaCompletada += MarcarOleadaTerminada;

            // Iniciar la oleada en ese Spawner
            spawnerActual.IniciarOleada(oleada);

            // Esperar a que termine
            yield return new WaitUntil(() => oleadaTerminada);

            // Desuscribirse con el MISMO método
            spawnerActual.OnOleadaCompletada -= MarcarOleadaTerminada;

            Debug.Log($"{oleada.nombreOleada} completada.");
            yield return new WaitForSeconds(2f);
        }

        Debug.Log("¡Todas las oleadas completadas!");
    }

    // Método con nombre para poder desuscribirse correctamente
    private void MarcarOleadaTerminada()
    {
        oleadaTerminada = true;
    }

    private Spawn ObtenerSpawner(int indice)
    {
        if (indice >= 0 && indice < spawners.Count)
            return spawners[indice];
        return null;
    }
}