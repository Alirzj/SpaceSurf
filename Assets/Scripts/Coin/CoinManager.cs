using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("Slider Settings")]
    public Slider coinSlider;
    public int coinsToFillBar = 10;

    [Header("Smooth Fill Settings")]
    public float fillSpeed = 2f;

    private int currentCoins = 0;
    private float targetFill = 0f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (coinSlider != null)
        {
            coinSlider.minValue = 0f;
            coinSlider.maxValue = 1f;
            coinSlider.value = 0f;
        }
    }

    void Update()
    {
        // Smoothly increase the slider
        if (coinSlider != null && coinSlider.value < targetFill)
        {
            coinSlider.value = Mathf.MoveTowards(coinSlider.value, targetFill, fillSpeed * Time.unscaledDeltaTime);
        }

        // Check if bar is full
        if (coinSlider.value >= 1f && currentCoins >= coinsToFillBar)
        {
            ReachedFullBar();
            currentCoins = 0;
            targetFill = 0f;
            coinSlider.value = 0f;
        }
    }

    public void AddCoin(int value = 1)
    {
        currentCoins += value;
        targetFill = Mathf.Clamp01((float)currentCoins / coinsToFillBar);
    }

    private void ReachedFullBar()
    {
        Debug.Log("Coin bar filled!");
        GameManager.Instance?.TriggerUpgradeChoice(); // ← This is what was missing
    }
}
