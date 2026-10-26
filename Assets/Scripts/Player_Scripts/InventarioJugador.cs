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
        if (indice >= 0 && indice < armas.Count)
        {
            armaActual = indice;
            Debug.Log($"Arma seleccionada: {armas[armaActual].Nombre}");
            if (animator != null)
            {
                animator.SetLayerWeight(upperBodyLayerIndex, 1f);
            }

            // 2. Apagar todas las armas de la mano y prender solo la que corresponde
            for (int i = 0; i < modelosArmasEnMano.Count; i++)
            {
                if (modelosArmasEnMano[i] != null)
                {
                    // Si el índice coincide, se activa. Si no, se desactiva.
                    modelosArmasEnMano[i].SetActive(i == indice);
                }
            }
            OnArmaCambiada?.Invoke(armas[armaActual]);
        }
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