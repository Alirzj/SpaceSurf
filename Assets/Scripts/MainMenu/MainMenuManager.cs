using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameScene;

    public GameObject settingsPanel;
    public GameObject creditsPanel;

    public void StartButton()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void SettingsButton()
    {
        settingsPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void CreditsButton()
    {
        creditsPanel.SetActive(true);
    }

    public void Exitbutton()
    {
        Application.Quit();
    }


}
