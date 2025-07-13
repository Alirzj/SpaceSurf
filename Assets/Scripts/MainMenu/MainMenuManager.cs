using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameScene;

    public GameObject settingsPanel;
    public GameObject creditsPanel;

    public AudioClip audio;
    public void StartButton()
    {
        if (audio != null)
            AudioManager.Instance?.PlaySound2D(audio, 1f);
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
        if (audio != null)
            AudioManager.Instance?.PlaySound2D(audio, 1f);
        Application.Quit();
    }


}
