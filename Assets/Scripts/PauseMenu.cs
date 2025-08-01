using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public AudioClip clickAudio;

    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Button pauseButton;
    public Button resumeButton;
    public Button restartButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Settings Panel")]
    public GameObject settingsPanel;
    public GameObject GreyBG;
    public GameObject GreyBG2;

    public Button muteButton;
    public Button gyroButton;
    public Button arrowButton;
    public Button backButton;

    [Header("Settings")]
    public string mainMenuSceneName = "Main Menu";
    public bool pauseAudio = true;
    public bool showCursor = true;

    [Header("Animation Settings")]
    public bool useAnimations = true;
    public float fadeInDuration = 0.3f;
    public AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isPaused = false;
    private bool wasAudioPaused = false;
    private CursorLockMode previousCursorLockMode;
    private bool previousCursorVisible;

    private CanvasGroup pauseMenuCanvasGroup;
    private float animationTimer = 0f;

    void Start()
    {
        pauseMenuCanvasGroup = pauseMenuPanel.GetComponent<CanvasGroup>();
        if (pauseMenuCanvasGroup == null)
            pauseMenuCanvasGroup = pauseMenuPanel.AddComponent<CanvasGroup>();

        SetupButtonListeners();
        SetPauseMenuVisibility(false);
        settingsPanel.SetActive(false);
        GreyBG.SetActive(false);
        GreyBG2.SetActive(false);


    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        if (useAnimations && animationTimer > 0f)
        {
            animationTimer -= Time.unscaledDeltaTime;
            float normalizedTime = 1f - (animationTimer / fadeInDuration);
            float curveValue = fadeInCurve.Evaluate(normalizedTime);

            if (isPaused)
            {
                pauseMenuCanvasGroup.alpha = curveValue;
                pauseMenuPanel.transform.localScale = Vector3.one * curveValue;
            }
            else
            {
                pauseMenuCanvasGroup.alpha = 1f - curveValue;
                pauseMenuPanel.transform.localScale = Vector3.one * (1f - curveValue);
            }
        }
    }

    void SetupButtonListeners()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(() =>
            {
                PauseGame();
                PlayClickSound();
            });
        }

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(() =>
            {
                ResumeGame();
                PlayClickSound();
            });
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(() =>
            {
                RestartGame();
                PlayClickSound();
            });
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(() =>
            {
                OpenSettingsPanel();
                PlayClickSound();
            });
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(() =>
            {
                QuitToMainMenu();
                PlayClickSound();
            });
        }

        if (muteButton != null)
        {
            muteButton.onClick.RemoveAllListeners();
            muteButton.onClick.AddListener(() =>
            {
                ToggleMute();
                PlayClickSound();
            });
        }

        if (gyroButton != null)
        {
            gyroButton.onClick.RemoveAllListeners();
            gyroButton.onClick.AddListener(() =>
            {
                SetGyroControls();
                PlayClickSound();
            });
        }

        if (arrowButton != null)
        {
            arrowButton.onClick.RemoveAllListeners();
            arrowButton.onClick.AddListener(() =>
            {
                SetArrowControls();
                PlayClickSound();
            });
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() =>
            {
                CloseSettingsPanel();
                PlayClickSound();
            });
        }
    }

    void PlayClickSound()
    {
        if (clickAudio != null)
            AudioManager.Instance?.PlaySound2D(clickAudio, 1f, true); // true to ignore pause
    }



    public void PauseGame()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        previousCursorLockMode = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        if (showCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (pauseAudio)
        {
            AudioListener.pause = true;
            wasAudioPaused = true;
        }

        SetPauseMenuVisibility(true);
        GreyBG2.SetActive(true);
        pauseButton.interactable = false;
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = previousCursorLockMode;
        Cursor.visible = previousCursorVisible;

        if (wasAudioPaused)
        {
            AudioListener.pause = false;
            wasAudioPaused = false;
        }

        SetPauseMenuVisibility(false);
        GreyBG2.SetActive(false);
        pauseButton.interactable = true;
        settingsPanel.SetActive(false);
        GreyBG.SetActive(false);

    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OpenSettingsPanel()
    {
        settingsPanel.SetActive(true);
        GreyBG.SetActive(true);
        pauseMenuPanel.SetActive(false);
        GreyBG2.SetActive(false);
    }

    public void CloseSettingsPanel()
    {
        settingsPanel.SetActive(false);
        GreyBG.SetActive(false);
        pauseMenuPanel.SetActive(true);
        GreyBG2.SetActive(true);

    }

    public void ToggleMute()
    {
        AudioListener.pause = !AudioListener.pause;
        if (muteButton != null)
        {
            var txt = muteButton.GetComponentInChildren<Text>();
            if (txt != null)
                txt.text = AudioListener.pause ? "Unmute" : "Mute";
        }
    }

    public void SetGyroControls()
    {
        var controller = FindFirstObjectByType<PlayerController>();
        controller?.SetControlType(ControlType.Gyro);
    }

    public void SetArrowControls()
    {
        var controller = FindFirstObjectByType<PlayerController>();
        controller?.SetControlType(ControlType.Arrows);
    }

    void SetPauseMenuVisibility(bool visible)
    {
        if (pauseMenuPanel == null) return;

        pauseMenuPanel.SetActive(visible);
        GreyBG.SetActive(visible);


        if (useAnimations)
        {
            animationTimer = fadeInDuration;
            if (visible)
            {
                pauseMenuCanvasGroup.alpha = 0f;
                pauseMenuPanel.transform.localScale = Vector3.zero;
            }
        }
        else
        {
            pauseMenuCanvasGroup.alpha = visible ? 1f : 0f;
            pauseMenuPanel.transform.localScale = Vector3.one;
        }
    }

    public bool IsPaused => isPaused;

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    void OnDisable()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }

    void OnDestroy()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}
