using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UpgradeCard : MonoBehaviour
{
    [HideInInspector] public Sprite assignedSprite;
    public UnityEvent onUpgradeSelected; // assign upgrade logic in Inspector OR via code

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        
        button.onClick.AddListener(OnClicked);
    }

    public void SetUpgrade(Sprite sprite, UnityAction action)
    {
        assignedSprite = sprite;

        // Clear previous listeners if any
        onUpgradeSelected.RemoveAllListeners();
        onUpgradeSelected.AddListener(action);
    }

    private void OnClicked()
    {
        Debug.Log("Upgrade selected: " + assignedSprite.name);
        onUpgradeSelected?.Invoke();

        // Optionally hide UI or resume game
        //GameManager.Instance.ResumeGameAfterUpgrade();
    }
}
