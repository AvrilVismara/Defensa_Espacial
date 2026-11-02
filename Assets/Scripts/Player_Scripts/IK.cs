using UnityEngine;

public class IK : MonoBehaviour
{
    [Header("Configuración IK")]
    [SerializeField] private Animator animator;
    public Transform objetivoManoIzquierda;
    public Transform objetivoManoDerecha;

    [Range(0, 1)] public float pesoIK = 0f;      // Peso actual
    public float pesoObjetivo = 0f;              // A dónde queremos llegar (0 o 1)
    public float velocidadTransicion = 8f;       // Velocidad del movimiento

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Interpolar suavemente el peso actual hacia el objetivo
        pesoIK = Mathf.Lerp(pesoIK, pesoObjetivo, Time.deltaTime * velocidadTransicion);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator != null && objetivoManoIzquierda != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, pesoIK);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, pesoIK);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, objetivoManoIzquierda.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, objetivoManoIzquierda.rotation);
        }
        else if (animator != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
        }

        if (animator != null && objetivoManoDerecha != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, pesoIK);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, pesoIK);
            animator.SetIKPosition(AvatarIKGoal.RightHand, objetivoManoDerecha.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, objetivoManoDerecha.rotation);
        }
        else if (animator != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
        }
    }
}