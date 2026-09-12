using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class ZoomThirdPerson : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CinemachineCamera virtualCam;

    [Header("Ajustes de Distancia")]
    [SerializeField] private float distanciaMinima = 1.5f; // Zoom cerca (puntería)
    [SerializeField] private float distanciaMaxima = 4f;   // Zoom lejos (exploración)
    [SerializeField] private float sensibilidad = 1f;
    [SerializeField] private float suavizado = 10f;

    private float distanciaObjetiva;
    private CinemachineThirdPersonFollow thirdPersonFollow;

    private void Start()
    {
        if (virtualCam == null)
        {
            virtualCam = GetComponent<CinemachineCamera>();
        }

        if (virtualCam != null)
        {
            thirdPersonFollow = virtualCam.GetComponent<CinemachineThirdPersonFollow>();
            if (thirdPersonFollow != null)
            {
                distanciaObjetiva = thirdPersonFollow.CameraDistance;
            }
        }
    }

    private void Update()
    {
        if (thirdPersonFollow == null || Mouse.current == null) return;

        float scroll = Mouse.current.scroll.y.ReadValue();

        if (Mathf.Abs(scroll) > 0.01f)
        {
            float direccion = Mathf.Sign(scroll);
            distanciaObjetiva -= direccion * sensibilidad;
            distanciaObjetiva = Mathf.Clamp(distanciaObjetiva, distanciaMinima, distanciaMaxima);
        }

        //Movemos suavemente la distancia de la cámara en 3ª persona
        thirdPersonFollow.CameraDistance = Mathf.Lerp(thirdPersonFollow.CameraDistance, distanciaObjetiva, Time.deltaTime * suavizado);
    }
}