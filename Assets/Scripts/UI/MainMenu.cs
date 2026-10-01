using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject optionsMenuUI;

    [SerializeField] private GameObject creditsMenuUI;

    public void ShowMainMenu()
    {
        mainMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(false);
    }

    public void ShowOptions()
    {
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
        creditsMenuUI.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        creditsMenuUI.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

}
