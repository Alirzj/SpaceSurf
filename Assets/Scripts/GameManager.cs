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
        AmuneToSimpleMeteor,
        AmuneToFire,
        GainTwoTimesCoin,
        IncreaseShieldDuration,
        IncreaseMagnetDuration,
        IncreaseLaserDuration,
        // Add more upgrades here
    }

    [Header("Upgrade Mappings")]
    public List<UpgradeDefinition> upgradeDefinitions = new List<UpgradeDefinition>();

    private Dictionary<Sprite, UpgradeType> spriteToUpgradeMap = new Dictionary<Sprite, UpgradeType>();


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
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

        // Create the mapping dictionary
        spriteToUpgradeMap.Clear();
        foreach (var def in upgradeDefinitions)
        {
            if (!spriteToUpgradeMap.ContainsKey(def.upgradeSprite))
                spriteToUpgradeMap.Add(def.upgradeSprite, def.upgradeType);
        }

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
            {
                img.sprite = assignedSprite;
            }

            // Remove old listeners
            btn.onClick.RemoveAllListeners();

            // Assign logic
            if (spriteToUpgradeMap.ContainsKey(assignedSprite))
            {
                UpgradeType upgradeType = spriteToUpgradeMap[assignedSprite];
                btn.onClick.AddListener(() => ApplyUpgrade(upgradeType));
            }

            // Optional: Close panel after selection
            btn.onClick.AddListener(() =>
            {
                upgradePanel.SetActive(false);
                ResumeGameAfterUpgrade();
            });
        }
    }

    void ApplyUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.IncreaseSpeed:
                Debug.Log("Upgrade: Speed increased!");
                // TODO: Apply speed logic
                break;

            case UpgradeType.FullHeal:
                Debug.Log("Upgrade: Full Health activated!");
                // TODO: Add shield
                break;

            case UpgradeType.AmuneToSimpleMeteor:
                Debug.Log("Upgrade: Amune To Simple Meteor!");
                // TODO: Heal player
                break;
            case UpgradeType.AmuneToFire:
                Debug.Log("Upgrade: Amune To Fire!");
                // TODO: Heal player
                break;
            case UpgradeType.GainTwoTimesCoin:
                Debug.Log("Upgrade: Gain Two Times Coin!");
                // TODO: Heal player
                break;
            case UpgradeType.IncreaseShieldDuration:
                Debug.Log("Upgrade: Increase Shield Duration!");
                // TODO: Heal player
                break;
            case UpgradeType.IncreaseMagnetDuration:
                Debug.Log("Upgrade: Increase Magnet Duration!");
                // TODO: Heal player
                break;
            case UpgradeType.IncreaseLaserDuration:
                Debug.Log("Upgrade: Increase Magnet Duration!");
                // TODO: Heal player
                break;

            default:
                Debug.LogWarning("Unknown upgrade selected.");
                break;
        }
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
    void ResumeGameAfterUpgrade()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        Debug.Log("Game resumed after upgrade.");
    }

}

