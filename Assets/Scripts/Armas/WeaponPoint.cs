using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponPoint : MonoBehaviour
{
    [Header("CONFIGURACIÓN DEL PUNTO")]
    [SerializeField] private float tiempoReaparicion = 8f; //Tiempo en segundos que tarda en aparecer el arma después de ser recogida
    [SerializeField] private float tiempoInicial = 2f; //Tiempo inicial de espera antes de la primera aparición

    [Header("CONFIGURACIÓN DE ARMAS (SPAWN ALEATORIO)")]
    [SerializeField] private List<ScriptableObject> configuracionesPosibles = new List<ScriptableObject>(); // lista de config de armas que pueden aparecer en este punto

    [Header("REFERENCIA VISUAL")]
    [SerializeField] private GameObject prefabArmaVisual;

    private GameObject armaVisualActual;      // El arma que está en el mapa
    private ComponenteArma componenteArma;    // Componente del arma visual
    private Collider colliderPunto;           // Collider del punto (Trigger)
    private bool armaDisponible = false;      // Si hay arma para recoger
    private Coroutine corrutinaReaparicion;   // Controla el ciclo de aparición
    private ScriptableObject configuracionActual; // Configuración del arma actual


    void Start()
    {
        // Verifica que haya al menos una configuración --
        if (configuracionesPosibles == null || configuracionesPosibles.Count == 0)
        {
            Debug.LogError($"{gameObject.name}: No hay configuraciones de armas asignadas.");
            return;
        }

        // Configurar el collider del punto como Trigger
        colliderPunto = GetComponent<Collider>();
        if (colliderPunto == null)
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 1.5f;
            colliderPunto = col;
        }
        else
        {
            colliderPunto.isTrigger = true;
        }

        // Verificar que haya un prefab visual
        if (prefabArmaVisual == null)
        {
            Debug.LogError($"{gameObject.name}: No se asignó un prefab visual.");
            return;
        }

        // Iniciar el ciclo de aparición
        IniciarCiclo();
    }

    public void IniciarCiclo()
    {
        if (corrutinaReaparicion != null)
        {
            StopCoroutine(corrutinaReaparicion);
        }
        corrutinaReaparicion = StartCoroutine(CicloAparicion());
    }

    private IEnumerator CicloAparicion()
    {
        // Esperar el tiempo inicial
        yield return new WaitForSeconds(tiempoInicial);

        while (true)
        {
            // 1. SELECCIONAR ARMA ALEATORIA
            configuracionActual = SeleccionarArmaAleatoria();

            // 2. CREAR EL ARMA VISUAL
            CrearArmaVisual();

            // 3. MOSTRARLA
            if (armaVisualActual != null)
            {
                armaVisualActual.SetActive(true);
                armaDisponible = true;
                Debug.Log($"{gameObject.name}: {configuracionActual.name} disponible");
            }
            else
            {
                Debug.LogError($"{gameObject.name}: No se pudo crear el arma visual.");
                yield break;
            }

            // 4. ESPERAR HASTA QUE EL JUGADOR LA RECOJA (o pase el tiempo máximo)
            float tiempoMaximoVisible = 60f;
            float tiempoTranscurrido = 0f;

            while (armaDisponible && tiempoTranscurrido < tiempoMaximoVisible)
            {
                yield return new WaitForSeconds(0.5f);
                tiempoTranscurrido += 0.5f;
            }

            // 5. OCULTARLA (si sigue disponible)
            if (armaDisponible)
            {
                OcultarArmaVisual();
                Debug.Log($"{gameObject.name}: Arma desaparecida por tiempo de espera.");
            }

            // 6. ESPERAR PARA REAPARECER
            yield return new WaitForSeconds(tiempoReaparicion);
        }
    }

    private ScriptableObject SeleccionarArmaAleatoria()
    {
        // Elegir un arma aleatoria de la lista
        int indice = Random.Range(0, configuracionesPosibles.Count);
        return configuracionesPosibles[indice];
    }

    private void CrearArmaVisual()
    {
        // Si ya hay un arma visual, destruirla
        if (armaVisualActual != null)
        {
            Destroy(armaVisualActual);
            armaVisualActual = null;
            componenteArma = null;
        }

        // Instanciar el prefab en la posición del punto
        armaVisualActual = Instantiate(prefabArmaVisual, transform.position, Quaternion.identity, transform);

        // Obtener el ComponenteArma
        componenteArma = armaVisualActual.GetComponent<ComponenteArma>();
        if (componenteArma == null)
        {
            Debug.LogError($"{gameObject.name}: El prefab visual no tiene 'ComponenteArma'.");
            Destroy(armaVisualActual);
            armaVisualActual = null;
            return;
        }

        // Asignar la configuración seleccionada al ComponenteArma
        componenteArma.AsignarConfiguracion(configuracionActual);

        // Ocultarla inicialmente
        armaVisualActual.SetActive(false);
    }

    private void OcultarArmaVisual()
    {
        if (armaVisualActual != null)
        {
            armaVisualActual.SetActive(false);
            armaDisponible = false;
        }
    }

// Recogida del arma
    private void OnTriggerEnter(Collider other)
    {
        // Solo si el arma está disponible y el que entra es el jugador
        if (!armaDisponible) return;
        if (!other.CompareTag("Player")) return;

        // Obtener el inventario del jugador
        InventarioJugador inventario = other.GetComponent<InventarioJugador>();
        if (inventario == null) return;

        // Obtener el arma del componente
        Arma arma = componenteArma?.Arma;
        if (arma == null) return;

        // El jugador ya tiene este arma?
        if (inventario.TieneArma(arma.Nombre))
        {
            // Obtener el arma que tiene el jugador
            Arma armaJugador = inventario.ObtenerArmaPorNombre(arma.Nombre);

            // La munición está completa?
            if (armaJugador.MunicionActual >= armaJugador.MunicionMaxima)
            {
                // con nunición completa no puede recoger
                Debug.Log($"{arma.Nombre}: Ya tienes el arma con munición completa. No puedes recoger más.");
                return; // no levanta nada, la municion continua en el suelo
            }
            else
            {
                // Tiene el arma pero con poca municion
                inventario.RecargarArma(arma.Nombre);
                Debug.Log($"{arma.Nombre}: Munición recargada.");

                // El arma desaparece porque ya recargó
                OcultarArmaVisual();
            }
        }
        else
        {
            //No tiene el arma
            inventario.AgregarArma(arma);
            Debug.Log($"{arma.Nombre}: ¡Arma nueva añadida al inventario!");

            // El arma desaparece porque fue recogida
            OcultarArmaVisual();
        }
    }
}