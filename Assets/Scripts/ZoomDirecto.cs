using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ZoomDirecto : MonoBehaviour
{
    [SerializeField] private float sensibilidad = 0.5f;
    [SerializeField] private float minRadio = 3f;
    [SerializeField] private float maxRadio = 15f;

    private CinemachineOrbitalFollow orbital;

    private void Start()
    {
        orbital = GetComponent<CinemachineOrbitalFollow>();
    }

    private void Update()
    {
        if (orbital == null || Mouse.current == null) return;

        float scroll = Mouse.current.scroll.y.ReadValue();

        if (Mathf.Abs(scroll) > 0.01f)
        {
            float delta = (scroll / 120f) * sensibilidad;
            orbital.Radius = Mathf.Clamp(orbital.Radius - delta, minRadio, maxRadio);
        }
    }
}