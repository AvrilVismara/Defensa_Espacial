using UnityEngine;
using UnityEngine.InputSystem;

public class ControlCamaraShooter : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform pivoteCamara; // Objeto CameraPivot (hijo del jugador)

    [Header("Sensibilidad y Límites")]
    [SerializeField] private float sensibilidadX = 0.15f;
    [SerializeField] private float sensibilidadY = 0.15f;
    [SerializeField] private float limiteVerticalMin = -50f; // Límite para mirar abajo
    [SerializeField] private float limiteVerticalMax = 90f;  // Límite para mirar arriba

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

    private void LateUpdate()
    {
        if (Mouse.current == null || pivoteCamara == null) return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 1. Rotación horizontal (Yaw): Rota todo el cuerpo del jugador sobre el eje Y
        float rotacionY = mouseDelta.x * sensibilidadX;
        transform.Rotate(Vector3.up * rotacionY);

        // 2. Rotación vertical (Pitch): Rota únicamente el CameraPivot en su eje X
        rotacionX -= mouseDelta.y * sensibilidadY;
        rotacionX = Mathf.Clamp(rotacionX, limiteVerticalMin, limiteVerticalMax);

        pivoteCamara.localEulerAngles = new Vector3(rotacionX, 0f, 0f);
    }
}