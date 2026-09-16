using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour
{
    [Header("1. Prefabs de Meteoritos")]
    [SerializeField] private List<GameObject> prefabsMeteoritos = new List<GameObject>();

    [Header("2. Tags de Edificios Objetivos")]
    [SerializeField] private List<string> tagsEdificios = new List<string>();

    [Header("3. Cantidad Máxima por Tipo")]
    [SerializeField] private List<int> cantidadesMaximas = new List<int>();

    [Header("Temporizador")]
    [SerializeField] private float tiempoEntreSpawns = 2f;
    private float timer = 0f;

    [Header("Encadenamiento de Oleadas")]
    [SerializeField] private Spawn siguienteSpawner; // Arrastra aquí el Spawn_2
    [SerializeField] private float retrasoSiguienteOleada = 5f; // Tiempo de espera en segundos

    private List<int> cantidadesGeneradas = new List<int>();
    private bool finalizado = false; // Evita que se ejecute la transición múltiples veces

    void Start()
    {
        SincronizarContadores();
    }

    void Update()
    {
        // Si ya completó su trabajo, no sigue ejecutando el Update
        if (finalizado) return;

        timer += Time.deltaTime;

        if (timer >= tiempoEntreSpawns)
        {
            SpawnMeteorite();
            timer = 0f;
        }
    }

    private void SincronizarContadores()
    {
        cantidadesGeneradas.Clear();
        for (int i = 0; i < prefabsMeteoritos.Count; i++)
        {
            cantidadesGeneradas.Add(0);
        }
    }

    void SpawnMeteorite()
    {
        if (prefabsMeteoritos.Count == 0 || tagsEdificios.Count != prefabsMeteoritos.Count || cantidadesMaximas.Count != prefabsMeteoritos.Count) return;

        if (cantidadesGeneradas.Count != prefabsMeteoritos.Count) SincronizarContadores();

        List<int> indicesDisponibles = new List<int>();

        for (int i = 0; i < prefabsMeteoritos.Count; i++)
        {
            if (cantidadesGeneradas[i] < cantidadesMaximas[i])
            {
                indicesDisponibles.Add(i);
            }
        }

        // Si ya no quedan meteoritos por soltar
        if (indicesDisponibles.Count == 0)
        {
            finalizado = true; // Marcamos como finalizado
            Debug.Log($"Spawner '{gameObject.name}' completó su oleada.");

            if (siguienteSpawner != null)
            {
                // Inicia la cuenta regresiva antes de activar el siguiente
                StartCoroutine(ActivarSiguienteOleada());
            }
            else
            {
                this.enabled = false; // Si era el último spawner, se apaga directamente
            }

            return;
        }

        int indiceElegido = indicesDisponibles[Random.Range(0, indicesDisponibles.Count)];

        string tagObjetivo = tagsEdificios[indiceElegido];
        GameObject[] edificiosObjetivo = GameObject.FindGameObjectsWithTag(tagObjetivo);

        if (edificiosObjetivo.Length == 0) return;

        GameObject edificioElegido = edificiosObjetivo[Random.Range(0, edificiosObjetivo.Length)];

        GameObject prefabElegido = prefabsMeteoritos[indiceElegido];
        GameObject meteorito = Instantiate(prefabElegido, transform.position, transform.rotation);

        MeteoriteMovement scriptMeteorito = meteorito.GetComponent<MeteoriteMovement>();
        if (scriptMeteorito != null)
        {
            scriptMeteorito.AsignarObjetivo(edificioElegido);
        }

        cantidadesGeneradas[indiceElegido]++;
    }

    // Corrutina que maneja el tiempo de descanso entre oleadas
    private IEnumerator ActivarSiguienteOleada()
    {
        Debug.Log($"Esperando {retrasoSiguienteOleada} segundos para iniciar la siguiente oleada...");

        yield return new WaitForSeconds(retrasoSiguienteOleada);

        if (siguienteSpawner != null)
        {
            siguienteSpawner.enabled = true; // Activa el componente del Spawn_2
            Debug.Log($"¡Oleada de '{siguienteSpawner.gameObject.name}' ACTIVADA!");
        }

        this.enabled = false; // Apaga finalmente el Spawn_1
    }
}