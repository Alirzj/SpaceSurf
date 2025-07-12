using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPowerUpController : MonoBehaviour
{
    [Header("Health UI")]
    public Image[] heartImages; // Assign 3 heart UI Images in the inspector
    public Sprite fullHeartSprite;
    public Sprite brokenHeartSprite;
    public int maxHealth = 3;
    public int currentHealth;

    public float baseSpeed = 5f;
    public float speedBoostMultiplier = 2f;
    public float baseDamage = 10f;
    public float damageBoostMultiplier = 2f;

    private float currentSpeed;
    private float currentDamage;

    public ParticleSystem speedEffect;
    public float boostedSimSpeed = 18f; 
    public float defaultSimSpeed = 8f;
    public TextMeshProUGUI distanceCounterText;
    private float distanceCounter = 0f;

    //public GameObject sweeperPrefab; // Assign in Inspector
    //public Transform sweeperSpawnPoint; // Optional spawn point

    public Transform playerTransform; // assign in inspector if needed

    private bool isMagnetActive = false;
    private float magnetSpeed = 10f; // how fast coins move to player

    private bool isShieldActive = false;

    public GameObject sweeperClearPrefab;
    public GameObject sweeperShieldPrefab;
    public GameObject sweeperMagnetPrefab;
    public GameObject shieldPrefab;

    public Transform sweeperClearSpawnPoint;
    public Transform sweeperShieldSpawnPoint;
    public Transform sweeperMagnetSpawnPoint;
    public Transform shieldSpawnPoint;


    private Dictionary<PowerUp.PowerUpType, Coroutine> activePowerUps = new Dictionary<PowerUp.PowerUpType, Coroutine>();

    private void Start()
    {
        ResetStats();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        // Increase counter based on currentSpeed
        distanceCounter += currentSpeed * Time.deltaTime;

        // Update UI
        if (distanceCounterText != null)
            distanceCounterText.text = distanceCounter.ToString("F2");

        if (isMagnetActive)
            AttractCoins();
    }

    public void ActivatePowerUp(PowerUp.PowerUpType type, float duration)
    {
        // If the power-up is already active, restart its timer
        if (activePowerUps.ContainsKey(type))
        {
            StopCoroutine(activePowerUps[type]);
            activePowerUps.Remove(type);
        }

        Coroutine newPowerUp = StartCoroutine(ApplyPowerUp(type, duration));
        activePowerUps[type] = newPowerUp;
    }

    private System.Collections.IEnumerator ApplyPowerUp(PowerUp.PowerUpType type, float duration)
    {
        ApplyStatBoost(type, true);

        yield return new WaitForSeconds(duration);

        ApplyStatBoost(type, false);
        activePowerUps.Remove(type);
    }

    private void ApplyStatBoost(PowerUp.PowerUpType type, bool activate)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.SpeedBoost:
                if (activate)
                {
                    MoveToZ.globalSpeed = 80f; // Your custom boost speed
                }
                else
                {
                    MoveToZ.globalSpeed = 40f; // Reset to default value
                }
                if (speedEffect != null)
                {
                    var main = speedEffect.main;
                    main.simulationSpeed = activate ? boostedSimSpeed : defaultSimSpeed;
                }

                break;


            case PowerUp.PowerUpType.EagleStrategem: // ClearObstacles
                if (activate)
                {
                    ClearAllObstacles();
                    SpawnSweeper(sweeperClearPrefab, sweeperClearSpawnPoint); // existing
                }
                break;

            case PowerUp.PowerUpType.Magnet:
                if (activate)
                {
                    EnableMagnet();
                    SpawnSweeper(sweeperMagnetPrefab, sweeperMagnetSpawnPoint); // new spawn
                }
                else
                {
                    DisableMagnet();
                }
                break;

            case PowerUp.PowerUpType.Shield:
                isShieldActive = activate;
                if (activate)
                    SpawnSweeper(sweeperShieldPrefab, sweeperShieldSpawnPoint); // new spawn
                    SpawnSweeper(shieldPrefab, shieldSpawnPoint); // new spawn
                break;
        }
    }


    private void ClearAllObstacles()
    {
        // Destroy all tagged obstacles
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obj in obstacles)
        {
            Destroy(obj);
        }

        // Spawn sweeper effect
        //if (sweeperPrefab != null)
        //{
        //    Vector3 spawnPos = sweeperSpawnPoint != null ? sweeperSpawnPoint.position : transform.position;
        //    Instantiate(sweeperPrefab, spawnPos, Quaternion.identity);
        //}
    }

    private void EnableMagnet()
    {
        isMagnetActive = true;
    }

    private void DisableMagnet()
    {
        isMagnetActive = false;
    }

    private void AttractCoins()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

        foreach (GameObject coin in coins)
        {
            if (coin == null) continue;

            Vector3 direction = (transform.position - coin.transform.position).normalized;
            coin.transform.position += direction * magnetSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Obstacle1"))
        {
            if (!isShieldActive)
            {
                currentHealth--;
                UpdateHealthUI();
                Debug.Log("Player hit! Health: " + currentHealth);

                if (currentHealth <= 0)
                    Die();
            }
            else
            {
                Debug.Log("Shield protected the player!");
            }

            // Destroy the obstacle
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Obstacle2"))
        {
            if (!isShieldActive)
            {
                currentHealth -= 2;
                UpdateHealthUI();
                Debug.Log("Player hit! Health: " + currentHealth);

                if (currentHealth <= 0)
                    Die();
            }
            else
            {
                Debug.Log("Shield protected the player!");
            }

            // Destroy the obstacle
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Obstacle3"))
        {
            if (!isShieldActive)
            {
                currentHealth -= 3;
                UpdateHealthUI();

                Debug.Log("Player hit! Health: " + currentHealth);

                if (currentHealth <= 0)
                    Die();
            }
            else
            {
                Debug.Log("Shield protected the player!");
            }

            // Destroy the obstacle
            Destroy(collision.gameObject);
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // Add death behavior (destroy, game over, etc.)
        // Destroy(gameObject);
    }

    private void SpawnSweeper(GameObject sweeperPrefab, Transform spawnPoint = null)
    {
        if (sweeperPrefab == null) return;

        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        Instantiate(sweeperPrefab, spawnPos, Quaternion.identity);
    }

    private void UpdateHealthUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
                heartImages[i].sprite = fullHeartSprite;
            else
                heartImages[i].sprite = brokenHeartSprite;
        }
    }


    private void ResetStats()
    {
        currentSpeed = baseSpeed;
        currentDamage = baseDamage;
    }

    public float GetCurrentSpeed() => currentSpeed;
    public float GetCurrentDamage() => currentDamage;
}
