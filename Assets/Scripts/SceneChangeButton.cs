using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    // Name of the scene to load
    public string sceneName;

    // This function can be linked to a UI Button's OnClick()
    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is not set.");
        }
    }
}
