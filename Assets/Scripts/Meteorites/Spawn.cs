using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour
{
    [Header("Tags de Edificios Objetivos")]
    [SerializeField] private List<string> tagsEdificios = new List<string>();

    private List<GameObject> prefabsMeteoritos = new List<GameObject>();
    private List<int> cantidadesMaximas = new List<int>();
    private List<int> cantidadesGeneradas = new List<int>();

    private float tiempoEntreSpawns = 2f;
    private float timer = 0f;

    private bool oleadaEnCurso = false;
    private WaveConfigSO oleadaActual;

    // Evento para notificar al WaveManager que la oleada terminó
    public System.Action OnOleadaCompletada;

    public void IniciarOleada(WaveConfigSO oleada)
    {
        oleadaActual = oleada;
        prefabsMeteoritos = oleada.prefabsMeteoritos;
        tiempoEntreSpawns = oleada.intervaloAparicion;

        // Distribuir la cantidad total entre los prefabs
        cantidadesMaximas.Clear();
        cantidadesGeneradas.Clear();
        int cantidadPorPrefab = oleada.cantidadMeteoritos / prefabsMeteoritos.Count;
        int resto = oleada.cantidadMeteoritos % prefabsMeteoritos.Count;

        for (int i = 0; i < prefabsMeteoritos.Count; i++)
        {
            cantidadesMaximas.Add(cantidadPorPrefab + (i < resto ? 1 : 0));
            cantidadesGeneradas.Add(0);
        }

        timer = 0f;
        oleadaEnCurso = true;
        Debug.Log($"Spawner inició {oleada.nombreOleada} ({oleada.cantidadMeteoritos} meteoritos)");
    }

    public void DetenerOleada()
    {
        oleadaEnCurso = false;
    }

    void Update()
    {
        if (!oleadaEnCurso) return;

        timer += Time.deltaTime;

        if (timer >= tiempoEntreSpawns)
        {
            SpawnMeteorite();
            timer = 0f;
        }
    }

    void SpawnMeteorite()
    {
        if (prefabsMeteoritos.Count == 0) return;

        List<int> indicesDisponibles = new List<int>();

        for (int i = 0; i < prefabsMeteoritos.Count; i++)
        {
            if (cantidadesGeneradas[i] < cantidadesMaximas[i])
                indicesDisponibles.Add(i);
        }

        if (indicesDisponibles.Count == 0)
        {
            oleadaEnCurso = false;
            Debug.Log($"Spawner completó {oleadaActual.nombreOleada}");
            OnOleadaCompletada?.Invoke();
            return;
        }

        int indiceElegido = indicesDisponibles[Random.Range(0, indicesDisponibles.Count)];
        string tagObjetivo = tagsEdificios[Random.Range(0, tagsEdificios.Count)];
        GameObject[] edificios = GameObject.FindGameObjectsWithTag(tagObjetivo);

        if (edificios.Length == 0) return;

        GameObject edificioElegido = edificios[Random.Range(0, edificios.Length)];
        GameObject meteorito = Instantiate(prefabsMeteoritos[indiceElegido], transform.position, transform.rotation);

        MeteoriteMovement script = meteorito.GetComponent<MeteoriteMovement>();
        if (script != null)
            script.AsignarObjetivo(edificioElegido);

        cantidadesGeneradas[indiceElegido]++;
    }
}