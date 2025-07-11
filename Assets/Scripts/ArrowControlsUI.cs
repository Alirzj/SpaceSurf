using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArrowControlsUI : MonoBehaviour
{
    [Header("UI References")]
    public Button leftButton;
    public Button rightButton;
    public Button toggleButton;
    public Canvas controlsCanvas;

    [Header("Button Settings")]
    public float buttonSize = 80f;
    public float buttonSpacing = 20f;
    public float bottomOffset = 100f;
    public float toggleButtonSize = 100f;

    private PlayerController playerController;
    private bool isVisible = true;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();

        // If no buttons are assigned, create them
        if (leftButton == null || rightButton == null || toggleButton == null)
        {
            CreateArrowButtons();
        }

        SetupButtonEvents();
    }

    void CreateArrowButtons()
    {
        // Create canvas if it doesn't exist
        if (controlsCanvas == null)
        {
            GameObject canvasObj = new GameObject("ArrowControlsCanvas");
            controlsCanvas = canvasObj.AddComponent<Canvas>();
            controlsCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            controlsCanvas.sortingOrder = 100;

            // Add CanvasScaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Add GraphicRaycaster
            canvasObj.AddComponent<GraphicRaycaster>();

            // Ensure EventSystem exists
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }

        // Create left button
        leftButton = CreateButton("LeftArrow", "◀", new Vector2(-buttonSpacing / 2f - buttonSize / 2f, bottomOffset));

        // Create right button
        rightButton = CreateButton("RightArrow", "▶", new Vector2(buttonSpacing / 2f + buttonSize / 2f, bottomOffset));

        // Create toggle button (top right corner)
        toggleButton = CreateToggleButton("ToggleControls", "GYRO", new Vector2(-toggleButtonSize / 2f, -toggleButtonSize / 2f));
    }

    Button CreateButton(string name, string text, Vector2 position)
    {
        // Create button GameObject
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(controlsCanvas.transform, false);

        // Set up RectTransform
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(buttonSize, buttonSize);
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.anchoredPosition = position;

        // Add Image component
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.8f);

        // Add Button component
        Button button = buttonObj.AddComponent<Button>();

        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 24;
        textComponent.color = Color.black;
        textComponent.alignment = TextAnchor.MiddleCenter;

        return button;
    }

    Button CreateToggleButton(string name, string text, Vector2 position)
    {
        // Create button GameObject
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(controlsCanvas.transform, false);

        // Set up RectTransform (top right corner)
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(toggleButtonSize, toggleButtonSize * 0.6f);
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = position;

        // Add Image component
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.8f, 0.2f, 0.8f);

        // Add Button component
        Button button = buttonObj.AddComponent<Button>();

        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 18;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.fontStyle = FontStyle.Bold;

        return button;
    }

    void SetupButtonEvents()
    {
        if (leftButton != null)
        {
            // Add EventTrigger for continuous input
            EventTrigger leftTrigger = leftButton.gameObject.AddComponent<EventTrigger>();

            // Pointer Down
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { OnLeftPressed(); });
            leftTrigger.triggers.Add(pointerDown);

            // Pointer Up
            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { OnButtonReleased(); });
            leftTrigger.triggers.Add(pointerUp);

            // Pointer Exit (in case finger slides off)
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnButtonReleased(); });
            leftTrigger.triggers.Add(pointerExit);
        }

        if (rightButton != null)
        {
            // Add EventTrigger for continuous input
            EventTrigger rightTrigger = rightButton.gameObject.AddComponent<EventTrigger>();

            // Pointer Down
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { OnRightPressed(); });
            rightTrigger.triggers.Add(pointerDown);

            // Pointer Up
            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { OnButtonReleased(); });
            rightTrigger.triggers.Add(pointerUp);

            // Pointer Exit
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnButtonReleased(); });
            rightTrigger.triggers.Add(pointerExit);
        }

        // Setup toggle button
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(() => {
                if (playerController != null)
                {
                    playerController.ToggleControlType();
                    UpdateToggleButtonText();
                }
            });
        }
    }

    void UpdateToggleButtonText()
    {
        if (toggleButton != null && playerController != null)
        {
            Text buttonText = toggleButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = playerController.currentControlType == ControlType.Gyro ? "ARROW" : "GYRO";

                // Change button color based on current control type
                Image buttonImage = toggleButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = playerController.currentControlType == ControlType.Gyro ?
                        new Color(0.8f, 0.2f, 0.2f, 0.8f) : // Red when showing "ARROW" (currently gyro)
                        new Color(0.2f, 0.8f, 0.2f, 0.8f);  // Green when showing "GYRO" (currently arrow)
                }
            }
        }
    }

    void OnLeftPressed()
    {
        if (playerController != null)
            playerController.OnLeftArrowPressed();
    }

    void OnRightPressed()
    {
        if (playerController != null)
            playerController.OnRightArrowPressed();
    }

    void OnButtonReleased()
    {
        if (playerController != null)
            playerController.OnArrowReleased();
    }

    public void SetVisible(bool visible)
    {
        isVisible = visible;

        if (controlsCanvas != null)
        {
            controlsCanvas.gameObject.SetActive(visible);
        }

        if (leftButton != null)
        {
            leftButton.gameObject.SetActive(visible);
        }

        if (rightButton != null)
        {
            rightButton.gameObject.SetActive(visible);
        }

        // Toggle button should always be visible
        if (toggleButton != null)
        {
            toggleButton.gameObject.SetActive(true);
        }

        // Update toggle button text when visibility changes
        UpdateToggleButtonText();
    }

    public bool IsVisible()
    {
        return isVisible;
    }
}