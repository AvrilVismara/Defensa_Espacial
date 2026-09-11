using UnityEngine;

public class ComponenteArma : MonoBehaviour
{
    [Header("Configuración del Arma")]
    [SerializeField] private ScriptableObject configuracion;  // ← Puede ser cualquier Configuración

    private Arma arma;

    public Arma Arma => arma;
    public ScriptableObject Configuracion => configuracion;

    // MÉTODO PARA ASIGNAR CONFIGURACIÓN EN TIEMPO DE EJECUCIÓN

    public void AsignarConfiguracion(ScriptableObject nuevaConfiguracion) // usado por weapon point
    {
        configuracion = nuevaConfiguracion;
        CrearArma();
    }

    void Start()
    {
        if (configuracion == null)
        {
            Debug.LogError($"{gameObject.name}: No hay configuración asignada");
            return;
        }

        CrearArma();
    }

    private void CrearArma()
    {
        // Intentar crear el arma según el tipo de configuración
        if (configuracion is ConfigPistolaSO pistolaConfig)
        {
            arma = pistolaConfig.CrearArma();
        }
        else if (configuracion is ConfigAmetralladoraSO ametralladoraConfig)
        {
            arma = ametralladoraConfig.CrearArma();
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Configuración no válida!");
            return;
        }

        Debug.Log($"{arma.Nombre} creada desde configuración!");
    }

    // metodos públicos
    public bool Disparar()
    {
        if (arma == null)
        {
            return false;
        }
        return arma.Disparar();
    }

    public void Recargar()
    {
        if (arma != null)
        {
            arma.Recargar();
        }
        // Si arma es null, no hace nada
    }

    public string ObtenerInfo()
    {
        if (arma != null)
        {
            return arma.ObtenerInfo();
        }
        else
        {
            return "Sin arma";
        }
    }

    public bool TieneMunicion()
    {
        if (arma != null)
        {
            return arma.TieneMunicion();
        }
        else
        {
            return false;
        }
    }

    public int MunicionActual
    {
        get
        {
            if (arma != null)
            {
                return arma.MunicionActual;
            }
            else
            {
                return 0;
            }
        }
    }

    public int MunicionMaxima
    {
        get
        {
            if (arma != null)
            {
                return arma.MunicionMaxima;
            }
            else
            {
                return 0;
            }
        }
    }

    public string Nombre
    {
        get
        {
            if (arma != null)
            {
                return arma.Nombre;
            }
            else
            {
                return "Sin arma";
            }
        }
    }
}