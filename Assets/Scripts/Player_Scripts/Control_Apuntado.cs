using UnityEngine;

public class ApuntadoJugador : MonoBehaviour
{
    [Header("Referencias")]
    // La cámara principal del juego (la que usamos para apuntar con el mouse).
    public Transform camaraPrincipal;

    // Referencia al Animator para poder acceder a los huesos del personaje.
    public Animator animator;

    [Header("Configuración del Torso")]
    public HumanBodyBones huesoTorso = HumanBodyBones.Spine;
    private Transform torso;

    // Compensación para arreglar la postura si el pecho queda torcido al apuntar.
    public Vector3 compensacionRotacionTorso = new Vector3(0, 0, 0);

    [Header("Configuración de la Cabeza")]
    // --- NUEVO: Seleccionamos el hueso de la cabeza ---
    public HumanBodyBones huesoCabeza = HumanBodyBones.Head;
    private Transform cabeza;

    // --- NUEVO: La cabeza suele tener ejes distintos al torso, por lo que necesita su propio ajuste ---
    public Vector3 compensacionRotacionCabeza = new Vector3(0, 0, 0);

    [Header("Ajustes")]
    public float suavidadGiro = 15f;

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();

        // Buscamos las coordenadas exactas de la columna y la cabeza dentro del modelo 3D
        if (animator != null)
        {
            torso = animator.GetBoneTransform(huesoTorso);
            cabeza = animator.GetBoneTransform(huesoCabeza); // Guardamos el hueso de la cabeza
        }
    }

    private void Update()
    {
        // 1. ROTACIÓN HORIZONTAL (Girar de izquierda a derecha el cuerpo entero).
        if (camaraPrincipal != null)
        {
            float anguloYCamara = camaraPrincipal.eulerAngles.y;
            Quaternion rotacionObjetivo = Quaternion.Euler(0, anguloYCamara, 0);

            // Suavizamos el giro para que el personaje no se voltee instantáneamente de forma robótica
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * suavidadGiro);
        }
    }

    private void LateUpdate()
    {
        // 2 y 3. ROTACIÓN VERTICAL Y MIRADA (Sobreescribimos la animación justo antes de dibujar el frame).

        if (camaraPrincipal != null)
        {
            // 2. TORSO: Igualamos su rotación a la de la cámara y le sumamos su compensación
            if (torso != null)
            {
                torso.rotation = camaraPrincipal.rotation * Quaternion.Euler(compensacionRotacionTorso);
            }

            // 3. CABEZA: Igualamos su rotación a la de la cámara para que los ojos apunten al crosshair
            if (cabeza != null)
            {
                cabeza.rotation = camaraPrincipal.rotation * Quaternion.Euler(compensacionRotacionCabeza);
            }
        }
    }
}