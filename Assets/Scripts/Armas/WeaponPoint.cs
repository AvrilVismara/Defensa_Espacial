using UnityEngine;
using System.Collections;

public class WeaponPoint : MonoBehaviour
{
    [Header("CONFIGURACIÓN DEL PUNTO")]
    [SerializeField] private float tiempoReaparicion = 8f;
    [SerializeField] private float tiempoInicial = 2f;

    [Header("ARMA FIJA DEL PUNTO")]
    [Tooltip("La única configuración de arma que puede aparecer en este punto")]
    [SerializeField] private ScriptableObject configuracionArma;

    [Header("REFERENCIA VISUAL")]
    [SerializeField] private GameObject prefabArmaVisual;

    private GameObject armaVisualActual;
    private ComponenteArma componenteArma;
    private Collider colliderPunto;
    private bool armaDisponible = false;
    private Coroutine corrutinaReaparicion;

    void Start()
    {
        if (configuracionArma == null)
        {
            Debug.LogError($"{gameObject.name}: No hay configuración de arma asignada.");
            return;
        }

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

        if (prefabArmaVisual == null)
        {
            Debug.LogError($"{gameObject.name}: No se asignó un prefab visual.");
            return;
        }

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
        yield return new WaitForSeconds(tiempoInicial);

        while (true)
        {
            // 1. Verificar si el arma está desbloqueada globalmente
            string nombreArma = ObtenerNombreArma(configuracionArma);

            if (!WaveManager.Instancia.EstaDesbloqueada(nombreArma))
            {
                // Si no está desbloqueada, esperar y reintentar
                yield return new WaitForSeconds(2f);
                continue;
            }

            // 2. Crear el arma visual
            CrearArmaVisual();

            // 3. Mostrarla
            if (armaVisualActual != null)
            {
                armaVisualActual.SetActive(true);
                armaDisponible = true;
                Debug.Log($"{gameObject.name}: {configuracionArma.name} disponible");
            }
            else
            {
                Debug.LogError($"{gameObject.name}: No se pudo crear el arma visual.");
                yield break;
            }

            // 4. Esperar hasta que el jugador la recoja (o pase el tiempo máximo)
            float tiempoMaximoVisible = 60f;
            float tiempoTranscurrido = 0f;

            while (armaDisponible && tiempoTranscurrido < tiempoMaximoVisible)
            {
                yield return new WaitForSeconds(0.5f);
                tiempoTranscurrido += 0.5f;
            }

            // 5. Ocultarla (si sigue disponible)
            if (armaDisponible)
            {
                OcultarArmaVisual();
            }

            // 6. Esperar para reaparecer
            yield return new WaitForSeconds(tiempoReaparicion);
        }
    }

    private string ObtenerNombreArma(ScriptableObject config)
    {
        if (config is ConfigAmetralladoraSO am) return am.nombreArma;
        if (config is ConfigLanzaCohetesSO lc) return lc.nombreArma;
        if (config is ConfigLaserSO laser) return laser.nombreArma;
        return null;
    }

    public void CrearArmaVisual()
    {
        if (armaVisualActual != null)
        {
            Destroy(armaVisualActual);
            armaVisualActual = null;
            componenteArma = null;
        }

        armaVisualActual = Instantiate(prefabArmaVisual, transform.position, Quaternion.identity, transform);

        componenteArma = armaVisualActual.GetComponent<ComponenteArma>();
        if (componenteArma == null)
        {
            Debug.LogError($"{gameObject.name}: El prefab visual no tiene 'ComponenteArma'.");
            Destroy(armaVisualActual);
            armaVisualActual = null;
            return;
        }

        componenteArma.AsignarConfiguracion(configuracionArma);
        armaVisualActual.SetActive(false);
    }

    public void OcultarArmaVisual()
    {
        if (armaVisualActual != null)
        {
            armaVisualActual.SetActive(false);
            armaDisponible = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!armaDisponible) return;
        if (!other.CompareTag("Player")) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("asdwqdwqdsad");
        }

        InventarioJugador inventario = other.GetComponent<InventarioJugador>();
        if (inventario == null) return;

        Arma arma = componenteArma?.Arma;
        if (arma == null) return;

        if (inventario.TieneArma(arma.Nombre))
        {
            Arma armaJugador = inventario.ObtenerArmaPorNombre(arma.Nombre);

            if (armaJugador.MunicionActual >= armaJugador.MunicionMaxima)
            {
                Debug.Log($"{arma.Nombre}: Ya tienes el arma con munición completa. No puedes recoger más.");
                return;
            }
            else
            {
                inventario.RecargarArma(arma.Nombre);
                inventario.EquiparArma(arma.Nombre);
                Debug.Log($"{arma.Nombre}: Munición recargada y arma equipada");
                OcultarArmaVisual();
            }
        }
        else
        {
            inventario.AgregarArma(arma);
            inventario.EquiparArma(arma.Nombre);
            Debug.Log($"{arma.Nombre}: Arma nueva añadida al inventario!");
            OcultarArmaVisual();
        }
    }
}