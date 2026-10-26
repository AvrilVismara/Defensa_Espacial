using UnityEngine;
using System.Collections.Generic;

public class InventarioJugador : MonoBehaviour
{
    private List<Arma> armas = new List<Arma>();
    private int armaActual = -1;

    public event System.Action<Arma> OnArmaAgregada;
    public event System.Action<Arma> OnArmaRecargada;
    public event System.Action<Arma> OnArmaCambiada;

    [Header("Referencias Visuales y Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject modeloArmaEnMano; // El GameObject del arma emparentado al hueso
    [SerializeField] private string nombreLayerUpperBody = "UpperBody";
    [SerializeField] private List<GameObject> modelosArmasEnMano;

    [Header("Control IK (Manos)")]
    [SerializeField] private IK scriptIK;
    // Lista para poner el LeftHandTarget de cada arma (en el mismo orden que los modelos)
    [SerializeField] private List<Transform> targetsManoIzquierdaIK;

    [Header("Transiciones Suaves")]
    [SerializeField] private float velocidadTransicionAnimacion = 8f;
    private float pesoCapaObjetivo = 0f;
    private float pesoCapaActual = 0f;

    private int upperBodyLayerIndex;
    private void Start()
    {
        // 1. Configuramos el Animator para que empiece sin la pose del arma
        if (animator != null)
        {
            upperBodyLayerIndex = animator.GetLayerIndex(nombreLayerUpperBody);
            animator.SetLayerWeight(upperBodyLayerIndex, 0f);
        }

        // 2. Apagamos TODOS los modelos 3D de las armas en las manos al iniciar el juego
        if (modelosArmasEnMano != null)
        {
            foreach (GameObject modeloArma in modelosArmasEnMano)
            {
                if (modeloArma != null)
                {
                    modeloArma.SetActive(false);
                }
            }
        }
    }

    private void Update()
    {
        // Interpolamos el peso de la capa UpperBody de forma suave
        if (animator != null && upperBodyLayerIndex != -1)
        {
            pesoCapaActual = Mathf.Lerp(pesoCapaActual, pesoCapaObjetivo, Time.deltaTime * velocidadTransicionAnimacion);
            animator.SetLayerWeight(upperBodyLayerIndex, pesoCapaActual);
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

        // 1. PARTE VISUAL
        if (animator != null && upperBodyLayerIndex != -1)
        {
            animator.SetLayerWeight(upperBodyLayerIndex, 1f);
        }
        pesoCapaObjetivo = 1f;
        for (int i = 0; i < modelosArmasEnMano.Count; i++)
        {
            if (modelosArmasEnMano[i] != null)
            {
                modelosArmasEnMano[i].SetActive(i == indice);
            }
        }

        // --- NUEVO: ACTIVAR IK ---
        if (scriptIK != null && targetsManoIzquierdaIK.Count > indice)
        {
            scriptIK.objetivoManoIzquierda = targetsManoIzquierdaIK[indice];
            scriptIK.pesoObjetivo = 1f;
        }
        // -------------------------

        // 2. PARTE LÓGICA
        armaActual = indice;

        if (indice >= 0 && indice < armas.Count)
        {
            Debug.Log($"Arma seleccionada: {armas[armaActual].Nombre}");
            OnArmaCambiada?.Invoke(armas[armaActual]);
        }

    }

    // Nuevo método para apagar todo y volver al estado desarmado
    public void DesequiparArma()
    {
        Debug.Log("Guardando arma...");
        armaActual = -1;

        if (animator != null && upperBodyLayerIndex != -1)
        {
            animator.SetLayerWeight(upperBodyLayerIndex, 0f);
        }
        pesoCapaObjetivo = 0f;
        for (int i = 0; i < modelosArmasEnMano.Count; i++)
        {
            if (modelosArmasEnMano[i] != null)
            {
                modelosArmasEnMano[i].SetActive(false);
            }
        }

        // --- NUEVO: DESACTIVAR IK ---
        if (scriptIK != null)
        {
            scriptIK.pesoObjetivo = 0f;
            // No hacemos objetivoManoIzquierda = null acá, porque si no la mano se cae de golpe 
            // antes de que el Lerp termine. El peso bajando a 0 se encarga de soltarla suavemente.
        }
        // ----------------------------

        OnArmaCambiada?.Invoke(null);
    }

    public bool DispararArmaActual()
    {
        if (armaActual < 0 || armaActual >= armas.Count)
        {
            Debug.Log("No hay arma seleccionada");
            return false;
        }

        Arma arma = armas[armaActual];

        if (!TieneMunicionSuficiente(arma))
        {
            Debug.Log($"[{arma.Nombre}] sin munición suficiente. Buscando otra arma...");
            return CambiarAProximaArmaConMunicion();
        }

        bool disparoExitoso = arma.Disparar();

        if (disparoExitoso)
        {

            if (!TieneMunicionSuficiente(arma))
            {
                Debug.Log($"[{arma.Nombre}] se quedó sin munición tras el disparo. Cambiando de arma...");
                CambiarAProximaArmaConMunicion();
            }
            return true;
        }
        else
        {

            return CambiarAProximaArmaConMunicion();
        }
    }

    private bool CambiarAProximaArmaConMunicion()
    {
        if (armas.Count <= 1)
        {
            Debug.LogWarning("Sin otras armas disponibles en el inventario.");
            return false;
        }

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

        Debug.LogWarning("¡Todas las armas del inventario están sin munición!");
        return false;
    }

    private bool TieneMunicionSuficiente(Arma arma)
    {
        if (arma == null || arma.MunicionActual <= 0) return false;


        if (arma is Ametralladora ametralladora)
        {
            return ametralladora.MunicionActual >= ametralladora.BalasPorRafaga;
        }

        return true;
    }

    public Arma ObtenerArmaActual()
    {
        if (armaActual >= 0 && armaActual < armas.Count)
            return armas[armaActual];
        return null;
    }

    public List<Arma> ObtenerTodasLasArmas()
    {
        return new List<Arma>(armas);
    }
}