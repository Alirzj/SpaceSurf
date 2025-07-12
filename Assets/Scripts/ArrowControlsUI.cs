using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArrowControlsUI : MonoBehaviour
{
    [Header("UI References")]
    public Button leftButton;
    public Button rightButton;
    public Canvas controlsCanvas;

    private PlayerController playerController;
    private bool isVisible = true;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        SetupButtonEvents();
    }

    void SetupButtonEvents()
    {
        if (leftButton != null)
        {
            EventTrigger leftTrigger = leftButton.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDown.callback.AddListener((data) => { OnLeftPressed(); });
            leftTrigger.triggers.Add(pointerDown);

            EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            pointerUp.callback.AddListener((data) => { OnButtonReleased(); });
            leftTrigger.triggers.Add(pointerUp);

            EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExit.callback.AddListener((data) => { OnButtonReleased(); });
            leftTrigger.triggers.Add(pointerExit);
        }

        if (rightButton != null)
        {
            EventTrigger rightTrigger = rightButton.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDown.callback.AddListener((data) => { OnRightPressed(); });
            rightTrigger.triggers.Add(pointerDown);

            EventTrigger.Entry pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            pointerUp.callback.AddListener((data) => { OnButtonReleased(); });
            rightTrigger.triggers.Add(pointerUp);

            EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            pointerExit.callback.AddListener((data) => { OnButtonReleased(); });
            rightTrigger.triggers.Add(pointerExit);
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
            controlsCanvas.gameObject.SetActive(visible);

        if (leftButton != null)
            leftButton.gameObject.SetActive(visible);

        if (rightButton != null)
            rightButton.gameObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return isVisible;
    }
}
