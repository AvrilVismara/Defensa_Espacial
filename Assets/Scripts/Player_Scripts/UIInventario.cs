using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class UIInventario : MonoBehaviour
{
    [Header("REFERENCIAS UI")]
    [SerializeField] private GameObject panelInventario;
    [SerializeField] private Transform contenedorArmas;
    [SerializeField] private GameObject prefabSlotArma;

    [Header("REFERENCIAS DEL JUGADOR")]
    [SerializeField] private InventarioJugador inventario;

    private bool inventarioAbierto = false;
    private List<GameObject> slotsActuales = new List<GameObject>();

    private PlayerMovement inputDeAcciones;

    private void Awake()
    {
        inputDeAcciones = new PlayerMovement();
    }

    private void OnEnable()
    {
        inputDeAcciones.Player.Enable();

        inputDeAcciones.Player.ToggleInventory.performed += OnToggleInventory;
        inputDeAcciones.Player.SelectWeapon.performed += OnSelectWeapon;
    }

    private void OnDisable()
    {
        inputDeAcciones.Player.ToggleInventory.performed -= OnToggleInventory;
        inputDeAcciones.Player.SelectWeapon.performed -= OnSelectWeapon;

        inputDeAcciones.Player.Disable();
    }

    // Abrir/cerrar inventario con "I" ---
    private void OnToggleInventory(InputAction.CallbackContext context)
    {
        ToggleInventario();
    }

    // Seleccionar arma con números 1-9 ---
    private void OnSelectWeapon(InputAction.CallbackContext context)
    {
        float valor = context.ReadValue<float>();
        int indice = Mathf.RoundToInt(valor) - 1;
        Debug.Log($"[INPUT DETECTADO] Tecla presionada con valor: {valor} -> Índice calculado: {indice}");
        if (indice < 0) return;

        if (inventario != null)
        {
            inventario.SeleccionarArma(indice);
            ActualizarSlots();
        }
    }

    // Abrir/cerrar el inventario ---
    public void ToggleInventario()
    {
        inventarioAbierto = !inventarioAbierto;

        if (panelInventario != null)
        {
            panelInventario.SetActive(inventarioAbierto);
        }

        if (inventarioAbierto)
        {
            ActualizarSlots();
        }
    }

    void Start()
    {
        // Obtener el inventario si no está asignado ---
        if (inventario == null)
        {
            inventario = FindFirstObjectByType<InventarioJugador>();
        }

        // Suscribirse a eventos del inventario ---
        if (inventario != null)
        {
            inventario.OnArmaAgregada += OnInventarioCambiado;
            inventario.OnArmaRecargada += OnInventarioCambiado;
            inventario.OnArmaCambiada += OnInventarioCambiado;
        }

        // Ocultar el panel al inicio ---
        if (panelInventario != null)
        {
            panelInventario.SetActive(false);
        }
    }

    // Actualizar slots cuando cambia el inventario ---
    private void OnInventarioCambiado(Arma arma)
    {
        if (inventarioAbierto)
        {
            ActualizarSlots();
        }
    }

    // Actualizar la lista de slots ---
    private void ActualizarSlots()
    {
        // Limpiar slots anteriores ---
        foreach (GameObject slot in slotsActuales)
        {
            Destroy(slot);
        }
        slotsActuales.Clear();

        if (inventario == null || contenedorArmas == null || prefabSlotArma == null) return;

        List<Arma> armas = inventario.ObtenerTodasLasArmas();
        Arma armaActual = inventario.ObtenerArmaActual();
        int armaActualIndex = armaActual != null ? armas.FindIndex(a => a.Nombre == armaActual.Nombre) : -1;

        // Crear un slot por cada arma ---
        for (int i = 0; i < armas.Count; i++)
        {
            Arma arma = armas[i];
            GameObject slot = Instantiate(prefabSlotArma, contenedorArmas);
            slotsActuales.Add(slot);

            // Configurar el texto ---
            TextMeshProUGUI textoSlot = slot.GetComponentInChildren<TextMeshProUGUI>();
            if (textoSlot != null)
            {
                string seleccion = (i == armaActualIndex) ? " ►" : "";
                textoSlot.text = $"{i + 1}. {arma.ObtenerInfo()}{seleccion}";
            }

            // Resaltar el arma seleccionada ---
            Image fondoSlot = slot.GetComponent<Image>();
            if (fondoSlot != null)
            {
                fondoSlot.color = (i == armaActualIndex) ? Color.yellow : Color.white;
            }

            // Botón para seleccionar con clic ---
            Button botonSlot = slot.GetComponent<Button>();
            if (botonSlot != null)
            {
                int indice = i;
                botonSlot.onClick.AddListener(() => {
                    inventario.SeleccionarArma(indice);
                    ActualizarSlots();
                });
            }
        }
    }

    void OnDestroy()
    {
        // Desuscribirse de eventos ---
        if (inventario != null)
        {
            inventario.OnArmaAgregada -= OnInventarioCambiado;
            inventario.OnArmaRecargada -= OnInventarioCambiado;
            inventario.OnArmaCambiada -= OnInventarioCambiado;
        }
    }
}