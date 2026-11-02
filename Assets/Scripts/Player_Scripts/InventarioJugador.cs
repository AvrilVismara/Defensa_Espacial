using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

// Agrupamos toda la configuración visual en un solo lugar
[System.Serializable]
public struct ConfiguracionVisualArma
{
    [Tooltip("Debe coincidir exactamente con el nombre del arma en su ScriptableObject")]
    public string nombreArma;
    public GameObject modeloArma;
    public Transform targetIKIzquierda;
    public Transform targetIKDerecha;
    public float idAnimacion; // Valor para el Blend Tree del Animator
}

public class InventarioJugador : MonoBehaviour
{
    [Header("Inputs del Action Map")]
    [SerializeField] private InputActionReference inputArma1;
    [SerializeField] private InputActionReference inputArma2;
    [SerializeField] private InputActionReference inputArma3;
    private List<Arma> armas = new List<Arma>();
    private int armaActual = -1;

    public event System.Action<Arma> OnArmaAgregada;
    public event System.Action<Arma> OnArmaRecargada;
    public event System.Action<Arma> OnArmaCambiada;

    [Header("Configuración Visual de Armas (Cualquier Orden)")]
    [SerializeField] private List<ConfiguracionVisualArma> configuracionesVisuales;

    [Header("Referencias Animación e IK")]
    [SerializeField] private Animator animator;
    [SerializeField] private string nombreLayerUpperBody = "UpperBody";
    [SerializeField] private string parametroTipoArma = "TipoArma";
    [SerializeField] private IK scriptIK;

    [Header("Transiciones Suaves")]
    [SerializeField] private float velocidadTransicionAnimacion = 8f;

    private int hashTipoArma;
    private float pesoCapaObjetivo = 0f;
    private float pesoCapaActual = 0f;
    private int upperBodyLayerIndex;


    private void Start()
    {
        if (animator != null)
        {
            upperBodyLayerIndex = animator.GetLayerIndex(nombreLayerUpperBody);
            hashTipoArma = Animator.StringToHash(parametroTipoArma);
            animator.SetLayerWeight(upperBodyLayerIndex, 0f);
        }

        // Apagamos TODOS los modelos 3D al iniciar
        foreach (var config in configuracionesVisuales)
        {
            if (config.modeloArma != null) config.modeloArma.SetActive(false);
        }
    }

    private void Update()
    {
        if (animator != null && upperBodyLayerIndex != -1)
        {
            pesoCapaActual = Mathf.Lerp(pesoCapaActual, pesoCapaObjetivo, Time.deltaTime * velocidadTransicionAnimacion);
            animator.SetLayerWeight(upperBodyLayerIndex, pesoCapaActual);
        }
    }

    public void OnEnable()
    {
        if (inputArma1 != null)
        {
            inputArma1.action.Enable();
            inputArma1.action.performed += AlPresionarArma1;
        }
        if (inputArma2 != null)
        {
            inputArma2.action.Enable();
            inputArma2.action.performed += AlPresionarArma2;
        }
        if (inputArma3 != null)
        {
            inputArma3.action.Enable();
            inputArma3.action.performed += AlPresionarArma3;
        }
    }

    public void OnDisable()
    {
        if (inputArma1 != null) inputArma1.action.performed -= AlPresionarArma1;
        if (inputArma2 != null) inputArma2.action.performed -= AlPresionarArma2;
        if (inputArma3 != null) inputArma3.action.performed -= AlPresionarArma3;
    }

    private void AlPresionarArma1(InputAction.CallbackContext context)
    {
        Debug.Log("¡Botón 1 presionado! Buscando Ametralladora...");
        CambiarArmaPorNombre("Ametralladora");
    }

    private void AlPresionarArma2(InputAction.CallbackContext context)
    {
        Debug.Log("¡Botón 2 presionado! Buscando LanzaCohetes...");
        CambiarArmaPorNombre("LanzaCohetes");
    }

    private void AlPresionarArma3(InputAction.CallbackContext context)
    {
        Debug.Log("¡Botón 3 presionado! Buscando Laser...");
        CambiarArmaPorNombre("Laser");
    }

    public void CambiarArmaPorNombre(string nombreArmaDeseada)
    {
        // Buscamos en qué posición de nuestro inventario (lista 'armas') está esta arma específica
        int indiceDeseado = armas.FindIndex(a => a.Nombre == nombreArmaDeseada);

        if (indiceDeseado != -1) // Si es distinto a -1, significa que SÍ la recogimos
        {
            Arma armaDeseada = armas[indiceDeseado];

            if (armaDeseada.MunicionActual > 0)
            {
                if (armaActual != indiceDeseado)
                {
                    SeleccionarArma(indiceDeseado);
                }
                else
                {
                    Debug.Log($"Ya tienes {armaDeseada.Nombre} en las manos.");
                }
            }
            else
            {
                Debug.Log($"No puedes equipar {armaDeseada.Nombre}, está sin munición.");
            }
        }
        else
        {
            Debug.Log($"Cancelado: Todavía no has recogido el arma {nombreArmaDeseada}.");
        }
    }
    public void AgregarArma(Arma nuevaArma)
    {
        if (nuevaArma == null) return;

        Arma existente = armas.Find(a => a.Nombre == nuevaArma.Nombre);

        if (existente != null)
        {
            existente.Recargar();
            OnArmaRecargada?.Invoke(existente);
            Debug.Log($"{existente.Nombre} recargada");
        }
        else
        {
            armas.Add(nuevaArma);
            if (armas.Count == 1) SeleccionarArma(0);
            OnArmaAgregada?.Invoke(nuevaArma);
            Debug.Log($"{nuevaArma.Nombre} añadida al inventario");
        }
    }

    // Método que llamamos desde el SpawnPoint
    public void EquiparArma(string nombreArma)
    {
        int indice = armas.FindIndex(a => a.Nombre == nombreArma);

        if (indice != -1)
        {
            // Solo llamamos a la selección si el arma no es la que ya está activa
            if (armaActual != indice)
            {
                SeleccionarArma(indice);
            }
            else
            {
                Debug.Log($"Arma {nombreArma} recogida y mantenida en las manos.");
            }
        }
    }

    public void RecargarArma(string nombreArma)
    {
        Arma arma = armas.Find(a => a.Nombre == nombreArma);
        if (arma != null)
        {
            arma.Recargar();
            OnArmaRecargada?.Invoke(arma);
        }
    }

    public bool TieneArma(string nombreArma)
    {
        return armas.Exists(a => a.Nombre == nombreArma);
    }

    public Arma ObtenerArmaPorNombre(string nombreArma)
    {
        return armas.Find(a => a.Nombre == nombreArma);
    }

    public void SeleccionarArma(int indice)
    {
        if (armaActual == indice)
        {
            DesequiparArma();
            return;
        }

        armaActual = indice;
        pesoCapaObjetivo = 1f;

        if (indice >= 0 && indice < armas.Count)
        {
            Arma armaSeleccionada = armas[indice];
            Debug.Log($"Arma seleccionada: {armaSeleccionada.Nombre}");

            // Buscamos la configuración visual basada en el NOMBRE, sin importar el índice
            bool configEncontrada = false;

            foreach (var config in configuracionesVisuales)
            {
                Debug.LogWarning($"Intentando emparejar: ArmaRecogida='{armaSeleccionada.Nombre}' con ListaInspector='{config.nombreArma}'");

                bool esArmaActual = (config.nombreArma == armaSeleccionada.Nombre);

                if (config.modeloArma != null)
                    config.modeloArma.SetActive(esArmaActual);

                if (esArmaActual)
                {
                    configEncontrada = true;

                    if (animator != null && upperBodyLayerIndex != -1)
                    {
                        animator.SetLayerWeight(upperBodyLayerIndex, 1f);
                        animator.SetFloat(hashTipoArma, config.idAnimacion);
                    }

                    if (scriptIK != null)
                    {
                        scriptIK.objetivoManoIzquierda = config.targetIKIzquierda;
                        scriptIK.objetivoManoDerecha = config.targetIKDerecha;
                        scriptIK.pesoObjetivo = 1f;
                    }
                }
            }

            if (!configEncontrada)
                Debug.LogWarning($"No se encontró configuración visual para: {armaSeleccionada.Nombre}");

            OnArmaCambiada?.Invoke(armaSeleccionada);
        }
    }

    public void DesequiparArma()
    {
        armaActual = -1;
        pesoCapaObjetivo = 0f;

        if (animator != null && upperBodyLayerIndex != -1)
            animator.SetLayerWeight(upperBodyLayerIndex, 0f);

        foreach (var config in configuracionesVisuales)
        {
            if (config.modeloArma != null) config.modeloArma.SetActive(false);
        }

        if (scriptIK != null) scriptIK.pesoObjetivo = 0f;

        OnArmaCambiada?.Invoke(null);
    }

    public bool DispararArmaActual()
    {
        if (armaActual < 0 || armaActual >= armas.Count) return false;

        Arma arma = armas[armaActual];

        if (!TieneMunicionSuficiente(arma))
            return CambiarAProximaArmaConMunicion();

        if (arma.Disparar())
        {
            if (!TieneMunicionSuficiente(arma))
                CambiarAProximaArmaConMunicion();
            return true;
        }

        return CambiarAProximaArmaConMunicion();
    }

    public bool CambiarAProximaArmaConMunicion()
    {
        if (armas.Count <= 1) return false;

        int totalArmas = armas.Count;
        for (int i = 1; i < totalArmas; i++)
        {
            int siguienteIndice = (armaActual + i) % totalArmas;
            Arma candidata = armas[siguienteIndice];

            if (TieneMunicionSuficiente(candidata))
            {
                SeleccionarArma(siguienteIndice);
                return armas[armaActual].Disparar();
            }
        }
        return false;
    }

    public bool TieneMunicionSuficiente(Arma arma)
    {
        if (arma == null || arma.MunicionActual <= 0) return false;
        if (arma is Ametralladora ametralladora)
            return ametralladora.MunicionActual >= ametralladora.BalasPorRafaga;
        return true;
    }

    public Arma ObtenerArmaActual()
    {
        if (armaActual >= 0 && armaActual < armas.Count) return armas[armaActual];
        return null;
    }

    public List<Arma> ObtenerTodasLasArmas()
    {
        return new List<Arma>(armas);
    }

}