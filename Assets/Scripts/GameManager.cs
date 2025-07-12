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

        // Shuffle sprite array and pick 3
        List<Sprite> shuffled = new List<Sprite>(upgradeCardSprites);
        ShuffleList(shuffled);

        for (int i = 0; i < upgradeButtons.Length && i < shuffled.Count; i++)
        {
            Button btn = upgradeButtons[i];
            btn.gameObject.SetActive(true);

            // Set the image of the button to the card sprite
            Image img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = shuffled[i];
            }

            // TODO: Assign what the card does on click later
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
}
