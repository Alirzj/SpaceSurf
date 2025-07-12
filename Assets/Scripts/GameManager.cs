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

    [Header("Slowdown Settings")]
    public float slowDownDuration = 3f;
    public float targetTimeScale = 0f;

    [Header("Upgrade Values")]
    public float speedIncreaseAmount = 0.2f;
    public float durationIncreaseAmount = 2f;

    [System.Serializable]
    public class UpgradeDefinition
    {
        public Sprite upgradeSprite;
        public UpgradeType upgradeType;
    }

    public enum UpgradeType
    {
        IncreaseSpeed,
        FullHeal,
        ImmunityToSimpleMeteor,
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
        playerController = FindFirstObjectByType<PlayerPowerUpController>();

        // Initialize upgrade mappings
        spriteToUpgradeMap.Clear();
        foreach (var def in upgradeDefinitions)
        {
            if (!spriteToUpgradeMap.ContainsKey(def.upgradeSprite))
                spriteToUpgradeMap.Add(def.upgradeSprite, def.upgradeType);
        }
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

        ShowUpgradeCards();
    }

    void ShowUpgradeCards()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(true);

        // Shuffle sprite array and pick 3
        List<Sprite> shuffled = new List<Sprite>(upgradeCardSprites);
        ShuffleList(shuffled);

        for (int i = 0; i < upgradeButtons.Length && i < shuffled.Count; i++)
        {
            Button btn = upgradeButtons[i];
            btn.gameObject.SetActive(true);

            Sprite assignedSprite = shuffled[i];

            // Set the image
            Image img = btn.GetComponent<Image>();
            if (img != null)
                img.sprite = assignedSprite;

            // Remove old listeners
            btn.onClick.RemoveAllListeners();

            // Assign upgrade logic
            if (spriteToUpgradeMap.ContainsKey(assignedSprite))
            {
                UpgradeType upgradeType = spriteToUpgradeMap[assignedSprite];
                btn.onClick.AddListener(() => ApplyUpgrade(upgradeType));
            }

            // Close panel and resume game gradually
            btn.onClick.AddListener(() =>
            {
                upgradePanel.SetActive(false);
                StartCoroutine(GraduallyResumeGame());
            });
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

            case UpgradeType.ImmunityToSimpleMeteor:
                // This would require additional implementation in the collision system
                Debug.Log("Upgrade: Immunity to simple meteors activated!");
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