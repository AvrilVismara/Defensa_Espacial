using UnityEngine;

public class ApuntadoJugador : MonoBehaviour
{
    [Header("Referencias")]
    // La cámara principal del juego (la que el jugador mueve con el mouse para apuntar).
    public Transform camaraPrincipal;

    // Referencia al Animator para poder acceder a la estructura de huesos del personaje.
    public Animator animator;

    [Header("Configuración del Torso")]
    // Spine (columna) o Chest (pecho) son ideales para inclinar al personaje al apuntar.
    public HumanBodyBones huesoTorso = HumanBodyBones.Spine;

    // Aquí guardaremos la posición y rotación exacta de ese hueso en el espacio 3D para modificarlo.
    private Transform torso;

    [Header("Ajustes")]
    // Controla qué tan rápido gira el personaje sobre su propio eje.
    // Un valor más alto hace que el cuerpo siga a la cámara casi al instante.
    public float suavidadGiro = 15f;
    // Este vector nos permite sumar o restar grados manualmente para enderezar al personaje si queda torcido al apuntar.
    public Vector3 compensacionRotacion = new Vector3(0, 0, 0);

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();

        /* Le pedimos al Animator que nos devuelva el Transform (coordenadas) del hueso específico 
        que elegimos arriba (por defecto, la columna/Spine) y lo guardamos.*/
        if (animator != null)
        {
            torso = animator.GetBoneTransform(huesoTorso);
        }
    }

    private void Update()
    {
        // Lo usamos EXCLUSIVAMENTE para la ROTACIÓN HORIZONTAL (Girar de izquierda a derecha el cuerpo entero).

        if (camaraPrincipal != null)
        {
            // Obtenemos solamente los grados de rotación en el eje Y (giro horizontal) de la cámara.
            // Ignoramos X (arriba/abajo) y Z (inclinación) para que el personaje no se acueste en el piso ni se caiga.
            float anguloYCamara = camaraPrincipal.eulerAngles.y;

            // Creamos una nueva rotación (Quaternion) usando solo ese ángulo Y.
            Quaternion rotacionObjetivo = Quaternion.Euler(0, anguloYCamara, 0);

            // Quaternion.Slerp interpola (suaviza) la rotación actual del personaje hacia la nueva rotación.
            // Multiplicar por Time.deltaTime asegura que la velocidad sea igual en cualquier PC (independiente de los FPS).
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * suavidadGiro);
        }
    }

    private void LateUpdate()
    {
        // LateUpdate se ejecuta un instante DESPUÉS de que el Animator haya calculado y aplicado su animación.
        // Es el ÚNICO lugar donde podemos modificar un hueso a mano. Si hiciéramos esto en Update(),
        // el Animator lo sobreescribiría milisegundos después y el hueso no se movería.

        // Lo usamos para la ROTACIÓN VERTICAL (Doblar la columna para apuntar arriba/abajo).

        if (torso != null && camaraPrincipal != null)
        {
            // 1. Tomamos la rotación exacta de la cámara (hacia dónde está mirando).
            // 2. Le sumamos (multiplicando en el lenguaje de los cuaterniones) nuestra compensación (offset) 
            //    para corregir ejes desalineados del modelo 3D.
            // 3. Se la aplicamos directamente al hueso de la columna para que el pecho apunte a donde mira la cámara.
            torso.rotation = camaraPrincipal.rotation * Quaternion.Euler(compensacionRotacion);
        }
    }
}