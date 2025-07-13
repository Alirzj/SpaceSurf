using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    // Name of the scene to load
    public string sceneName;

    public AudioClip audio;
    // This function can be linked to a UI Button's OnClick()
    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            if (audio != null)
                AudioManager.Instance?.PlaySound2D(audio, 1f);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is not set.");
        }
    }
}
