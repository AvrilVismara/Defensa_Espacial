using UnityEngine;
using UnityEngine.EventSystems;

public class PauseGame : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private GameObject menuPause;



    [Header("Scripts a desactivar al pausar")]
    [SerializeField] private MonoBehaviour scriptCamara;

    private bool gamePaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (menuPause != null)
            menuPause.SetActive(false);

        Time.timeScale = 1f;
        gamePaused = false;

        // Volver a bloquear cursor para gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (scriptCamara != null)
            scriptCamara.enabled = true;
    }

    public void Pause()
    {
        if (menuPause != null)
            menuPause.SetActive(true);

        Time.timeScale = 0f;
        gamePaused = true;

        // Asegurar que el script de cámara no siga consumiendo la entrada del mouse
        if (scriptCamara != null)
            scriptCamara.enabled = false;

        // Liberar el cursor explícitamente para la UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Forzar al EventSystem a reconocer la posición actual del puntero
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("Juego pausado");
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}