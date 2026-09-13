using UnityEngine;
using UnityEngine.InputSystem;

public class ControlCamaraShooter : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform pivoteCamara; // Objeto CameraPivot

    [Header("Sensibilidad y Límites")]
    [SerializeField] private float sensibilidadX = 0.15f;
    [SerializeField] private float sensibilidadY = 0.15f;
    [SerializeField] private float limiteVerticalMin = -30f; // Límite para mirar abajo
    [SerializeField] private float limiteVerticalMax = 60f;  // Límite para mirar arriba

    private float rotacionX = 0f;

    private void Start()
    {
        // Bloquea y oculta el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pivoteCamara != null)
        {
            rotacionX = pivoteCamara.localEulerAngles.x;
        }
    }

    private void Update()
    {
        if (Mouse.current == null || pivoteCamara == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 1. Rotación horizontal Rota todo el cuerpo del jugador
        float rotacionY = mouseDelta.x * sensibilidadX;
        transform.Rotate(Vector3.up * rotacionY);

        // 2. Rotación vertical Rota únicamente el CameraPivot para inclinar la vista
        rotacionX -= mouseDelta.y * sensibilidadY;
        rotacionX = Mathf.Clamp(rotacionX, limiteVerticalMin, limiteVerticalMax);

        pivoteCamara.localEulerAngles = new Vector3(rotacionX, 0f, 0f);
    }
}