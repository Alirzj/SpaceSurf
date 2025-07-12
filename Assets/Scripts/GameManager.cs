using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Upgrade UI")]
    public GameObject upgradePanel;
    public Button[] upgradeButtons; // 3 card buttons
    public Sprite[] upgradeCardSprites; // Sprites to assign randomly
    public Transform[] cardSlots = new Transform[3]; // Assign the 3 spawn points in inspector

    [Header("Slowdown Settings")]
    public float slowDownDuration = 3f;
    public float targetTimeScale = 0f;

    [Header("Upgrade Values")]
    public float speedIncreaseAmount = 0.2f;
    public float durationIncreaseAmount = 2f;


    [System.Serializable]
    public class UpgradeDefinition
    {
        public GameObject upgradePrefab; // Prefab that includes visuals + button
        public UpgradeType upgradeType;
    }


    public enum UpgradeType
    {
        IncreaseSpeed,
        FullHeal,
        IncreaseShieldDuration,
        IncreaseMagnetDuration,
        IncreaseLaserDuration,
        IncreaseSpeedBoostDuration
    }

    [Header("Upgrade Mappings")]
    public List<UpgradeDefinition> upgradeDefinitions = new List<UpgradeDefinition>();
    private Dictionary<Sprite, UpgradeType> spriteToUpgradeMap = new Dictionary<Sprite, UpgradeType>();

    private PlayerPowerUpController playerController;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Ensure time scale is normal on start
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // Hide upgrade panel if it's shown in editor
        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        // Optional: Clear card slots in case anything is already in the scene
        ClearPreviousUpgradeCards();

        playerController = FindObjectOfType<PlayerPowerUpController>();

    }


    public void TriggerUpgradeChoice()
    {
        StartCoroutine(SlowDownAndShowUpgrade());
    }

    IEnumerator SlowDownAndShowUpgrade()
    {
        float startScale = Time.timeScale;
        float elapsed = 0f;

        while (elapsed < slowDownDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, targetTimeScale, elapsed / slowDownDuration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;

        ClearPreviousUpgradeCards();
        ShowUpgradeCards();
    }

    void ShowUpgradeCards()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(true);

        // Shuffle available upgrade definitions
        List<UpgradeDefinition> shuffled = new List<UpgradeDefinition>(upgradeDefinitions);
        ShuffleList(shuffled);

        for (int i = 0; i < cardSlots.Length && i < shuffled.Count; i++)
        {
            var upgradeDef = shuffled[i];
            var slot = cardSlots[i];

            // Instantiate the prefab
            GameObject instance = Instantiate(upgradeDef.upgradePrefab, slot.position, slot.rotation, slot);

            // Find the button in the prefab
            Button btn = instance.GetComponentInChildren<Button>();

            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                UpgradeType type = upgradeDef.upgradeType; // Capture in local variable for closure
                btn.onClick.AddListener(() => ApplyUpgrade(type));
                btn.onClick.AddListener(() => StartCoroutine(GraduallyResumeGame()));
                btn.onClick.AddListener(() => upgradePanel.SetActive(false));
            }
        }
    }


    void ClearPreviousUpgradeCards()
    {
        foreach (var slot in cardSlots)
        {
            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }
        }
    }


    void ApplyUpgrade(UpgradeType type)
    {
        if (playerController == null) return;

        switch (type)
        {
            case UpgradeType.IncreaseSpeed:
                playerController.UpgradeBaseSpeed(speedIncreaseAmount);
                Debug.Log($"Upgrade: Base speed increased by {speedIncreaseAmount}!");
                break;

            case UpgradeType.FullHeal:
                playerController.FullHeal();
                Debug.Log("Upgrade: Full Health restored!");
                break;

            case UpgradeType.IncreaseShieldDuration:
                playerController.UpgradePowerUpDuration(PowerUp.PowerUpType.Shield, durationIncreaseAmount);
                Debug.Log($"Upgrade: Shield duration increased by {durationIncreaseAmount} seconds!");
                break;

            case UpgradeType.IncreaseMagnetDuration:
                playerController.UpgradePowerUpDuration(PowerUp.PowerUpType.Magnet, durationIncreaseAmount);
                Debug.Log($"Upgrade: Magnet duration increased by {durationIncreaseAmount} seconds!");
                break;

            case UpgradeType.IncreaseLaserDuration:
                playerController.UpgradePowerUpDuration(PowerUp.PowerUpType.EagleStrategem, durationIncreaseAmount);
                Debug.Log($"Upgrade: Laser duration increased by {durationIncreaseAmount} seconds!");
                break;

            case UpgradeType.IncreaseSpeedBoostDuration:
                playerController.UpgradePowerUpDuration(PowerUp.PowerUpType.SpeedBoost, durationIncreaseAmount);
                Debug.Log($"Upgrade: Speed boost duration increased by {durationIncreaseAmount} seconds!");
                break;

            default:
                Debug.LogWarning("Unknown upgrade selected.");
                break;
        }
    }

    IEnumerator GraduallyResumeGame()
    {
        float elapsed = 0f;
        float resumeDuration = slowDownDuration;
        float startScale = 0f;
        float endScale = 1f;

        while (elapsed < resumeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(startScale, endScale, elapsed / resumeDuration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[rand];
            list[rand] = temp;
        }
    }
}