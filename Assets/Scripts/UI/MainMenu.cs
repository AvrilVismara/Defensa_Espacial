using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject controlsMenuUI;

    [SerializeField] private GameObject creditsMenuUI;

    public void ShowMainMenu()
    {
        mainMenuUI.SetActive(true);
        controlsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(false);
    }

    public void ShowControls()
    {
        mainMenuUI.SetActive(false);
        controlsMenuUI.SetActive(true);
        creditsMenuUI.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenuUI.SetActive(false);
        controlsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
