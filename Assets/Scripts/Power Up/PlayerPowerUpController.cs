using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPowerUpController : MonoBehaviour
{
    [Header("Health UI")]
    public Image[] heartImages;
    public Sprite fullHeartSprite;
    public Sprite brokenHeartSprite;
    public int maxHealth = 3;
    public int currentHealth;


    public AudioClip[] HitAudios;
    public AudioClip[] RockAudios;

    [Header("Base Stats")]
    public float baseSpeed = 40f;
    public float baseSpeedMultiplier = 1f;

    // Store the original speed to restore to
    private float originalGlobalSpeed;

    [Header("Distance Counter")]
    public TextMeshProUGUI distanceCounterText;
    private float distanceCounter = 0f;

    [Header("Powerup Durations (Upgradeable)")]
    public float biggieDuration = 5f;
    public float oguDuration = 5f;
    public float shieldDuration = 5f;
    public float speedDuration = 5f;

    [Header("Powerup Settings")]
    public float speedBoostMultiplier = 1.5f;
    public float magnetRange = 15f;
    public float magnetSpeed = 10f;
    public float coinFadeSpeed = 2f;

    [Header("Powerup Prefabs")]
    public GameObject biggiePrefab;
    public GameObject oguPrefab;
    public GameObject shieldPrefab;
    public GameObject speedPrefab;

    [Header("Shield Effect")]
    public GameObject shieldEffectPrefab; // The shield effect that surrounds the player
    public Vector3 shieldEffectOffset = Vector3.zero; // Offset from player position

    [Header("Spawn Positions")]
    public Transform biggieSpawnPoint;
    public Transform oguSpawnPoint;
    public Transform shieldSpawnPoint;
    public Transform speedSpawnPoint;

    [Header("Biggie Laser")]
    public float laserWidth = 10f;
    public float laserLength = 20f;

    [Header("Particles")]
    public ParticleSystem speedParticles;
    public float boostedParticleSpeed = 18f;
    public float defaultParticleSpeed = 8f;

    public ParticleSystem MeteorParticles;
    public float MeteorboostedParticleSpeed = 18f;
    public float MeteordefaultParticleSpeed = 8f;

    // Active powerup instances
    private GameObject activeBiggie;
    private GameObject activeOgu;
    private GameObject activeShield;
    private GameObject activeSpeed;
    private GameObject activeLaser;
    private GameObject activeShieldEffect; // The shield effect around the player

    // Animation components
    private Animator biggieAnimator;
    private Animator oguAnimator;
    private Animator shieldAnimator;
    private Animator speedAnimator;

    // Active powerup tracking
    private Dictionary<PowerUp.PowerUpType, Coroutine> activePowerUps = new Dictionary<PowerUp.PowerUpType, Coroutine>();

    // Powerup states
    private bool isBiggieActive = false;
    private bool isOguActive = false;
    private bool isShieldActive = false;
    private bool isSpeedActive = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Store the original global speed at start
        originalGlobalSpeed = MoveToZ.globalSpeed;

        // Also sync our base speed with the global speed
        if (baseSpeed != MoveToZ.globalSpeed)
        {
            Debug.Log($"Warning: baseSpeed ({baseSpeed}) doesn't match MoveToZ.globalSpeed ({MoveToZ.globalSpeed}). Using MoveToZ.globalSpeed.");
            baseSpeed = MoveToZ.globalSpeed;
        }
    }

    private void Update()
    {
        // Update distance counter
        distanceCounter += MoveToZ.globalSpeed * Time.deltaTime;
        if (distanceCounterText != null)
            distanceCounterText.text = distanceCounter.ToString("F2") + " KM"; // <- added unit

        // Handle active powerup effects
        if (isOguActive)
            HandleMagnetEffect();

        if (isBiggieActive)
            HandleLaserDestruction();

        // Debug key for checking speed
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log($"Current Speed: {MoveToZ.globalSpeed}, Base Speed: {baseSpeed}, Multiplier: {baseSpeedMultiplier}, Original: {originalGlobalSpeed}");
        }
    }


    public void ActivatePowerUp(PowerUp.PowerUpType type, float duration = 0f)
    {
        // Use custom duration or default
        float actualDuration = duration > 0 ? duration : GetPowerUpDuration(type);

        // If powerup is already active, restart it
        if (activePowerUps.ContainsKey(type))
        {
            StopCoroutine(activePowerUps[type]);
            activePowerUps.Remove(type);
        }

        Coroutine newPowerUp = StartCoroutine(PowerUpSequence(type, actualDuration));
        activePowerUps[type] = newPowerUp;
    }

    private float GetPowerUpDuration(PowerUp.PowerUpType type)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.EagleStrategem: return biggieDuration; // Biggie
            case PowerUp.PowerUpType.Magnet: return oguDuration; // Ogu
            case PowerUp.PowerUpType.Shield: return shieldDuration;
            case PowerUp.PowerUpType.SpeedBoost: return speedDuration;
            default: return 5f;
        }
    }

    private IEnumerator PowerUpSequence(PowerUp.PowerUpType type, float duration)
    {
        // Coming animation
        yield return StartCoroutine(PlayComingAnimation(type));

        // Activate effect
        ActivatePowerUpEffect(type);

        // Play idle
        PlayIdleAnimation(type);

        // Wait until Idle animation state actually starts
        yield return StartCoroutine(WaitForIdleAnimation(type));

        // THEN wait for duration
        yield return new WaitForSeconds(duration);

        // Leaving animation
        yield return StartCoroutine(PlayLeavingAnimation(type));

        // Deactivate
        DeactivatePowerUpEffect(type);

        activePowerUps.Remove(type);
    }

    private IEnumerator WaitForIdleAnimation(PowerUp.PowerUpType type)
    {
        GameObject powerupInstance = GetActivePowerUpInstance(type);
        Animator animator = powerupInstance?.GetComponent<Animator>();

        if (animator == null)
            yield break;

        // Wait until the "Idle" animation is playing
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            yield return null; // wait a frame
        }
    }


    private IEnumerator PlayComingAnimation(PowerUp.PowerUpType type)
    {
        GameObject powerupInstance = SpawnPowerUp(type);
        Animator animator = powerupInstance?.GetComponent<Animator>();

        if (powerupInstance != null && animator != null)
        {
            animator.Play("Coming");
            yield return new WaitForSeconds(GetAnimationLength(animator, "Coming"));
        }
    }

    private void PlayIdleAnimation(PowerUp.PowerUpType type)
    {
        GameObject powerupInstance = GetActivePowerUpInstance(type);
        Animator animator = powerupInstance?.GetComponent<Animator>();

        if (animator != null)
        {
            animator.Play("Idle");
        }
    }
    private IEnumerator PlayLeavingAnimation(PowerUp.PowerUpType type)
    {
        GameObject powerupInstance = GetActivePowerUpInstance(type);
        Animator animator = powerupInstance?.GetComponent<Animator>();

        if (animator != null)
        {
            animator.Play("Leaving");

            // Wait for the animation to actually start
            yield return null;

            // Wait until the animation is no longer playing
            while (animator.GetCurrentAnimatorStateInfo(0).IsName("Leaving") &&
                   animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }
        }

        // Destroy the powerup instance
        if (powerupInstance != null)
        {
            Destroy(powerupInstance);
            ClearActivePowerUpInstance(type);
        }
    }

    private void ActivatePowerUpEffect(PowerUp.PowerUpType type)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.EagleStrategem: // Biggie
                isBiggieActive = true;
                SpawnLaser();
                break;

            case PowerUp.PowerUpType.Magnet: // Ogu
                isOguActive = true;
                break;

            case PowerUp.PowerUpType.Shield:
                isShieldActive = true;
                SpawnShieldEffect();
                break;

            case PowerUp.PowerUpType.SpeedBoost:
                isSpeedActive = true;
                // Store current speed before boosting (in case it was already modified)
                float currentSpeed = MoveToZ.globalSpeed;
                MoveToZ.globalSpeed = currentSpeed * speedBoostMultiplier;
                Debug.Log($"Speed boost activated: {currentSpeed} -> {MoveToZ.globalSpeed}");

                // Boost particle speed
                if (speedParticles != null)
                {
                    var main = speedParticles.main;
                    main.simulationSpeed = boostedParticleSpeed;
                }

                if (MeteorParticles != null)
                {
                    var main = MeteorParticles.main;
                    main.simulationSpeed = MeteorboostedParticleSpeed;
                }

                break;
        }
    }

    private void DeactivatePowerUpEffect(PowerUp.PowerUpType type)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.EagleStrategem: // Biggie
                isBiggieActive = false;
                DestroyLaser();
                break;

            case PowerUp.PowerUpType.Magnet: // Ogu
                isOguActive = false;
                break;

            case PowerUp.PowerUpType.Shield:
                isShieldActive = false;
                DestroyShieldEffect();
                break;

            case PowerUp.PowerUpType.SpeedBoost:
                isSpeedActive = false;
                // Restore speed to what it should be (accounting for upgrades)
                float normalSpeed = baseSpeed * baseSpeedMultiplier;
                MoveToZ.globalSpeed = normalSpeed;
                Debug.Log($"Speed boost deactivated: restored to {normalSpeed}");

                // Reset particle speed
                if (speedParticles != null)
                {
                    var main = speedParticles.main;
                    main.simulationSpeed = defaultParticleSpeed;
                }

                if (MeteorParticles != null)
                {
                    var main = MeteorParticles.main;
                    main.simulationSpeed = MeteordefaultParticleSpeed;
                }

                break;
        }
    }

    private void HandleMagnetEffect()
    {
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");

        foreach (GameObject coin in coins)
        {
            if (coin == null) continue;

            float distance = Vector3.Distance(transform.position, coin.transform.position);

            if (distance <= magnetRange)
            {
                StartCoroutine(AttractAndCollectCoin(coin));
            }
        }
    }

    private IEnumerator AttractAndCollectCoin(GameObject coin)
    {
        if (coin == null) yield break;

        // Get coin renderer for fade effect
        Renderer coinRenderer = coin.GetComponent<Renderer>();
        Color originalColor = coinRenderer.material.color;

        while (coin != null && Vector3.Distance(transform.position, coin.transform.position) > 0.5f)
        {
            // Move coin towards player
            Vector3 direction = (transform.position - coin.transform.position).normalized;
            coin.transform.position += direction * magnetSpeed * Time.deltaTime;

            // Fade alpha
            float distance = Vector3.Distance(transform.position, coin.transform.position);
            float alpha = Mathf.Clamp01(distance / magnetRange);
            Color newColor = originalColor;
            newColor.a = alpha;
            coinRenderer.material.color = newColor;

            yield return null;
        }

        // Collect coin
        if (coin != null)
        {
            CoinManager.Instance?.AddCoin(1);
            Destroy(coin);
        }
    }

    private void HandleLaserDestruction()
    {
        // Find all obstacles and destroy them if they're in front of the player
        List<GameObject> obstacles = new List<GameObject>();
        obstacles.AddRange(GameObject.FindGameObjectsWithTag("Obstacle1"));
        obstacles.AddRange(GameObject.FindGameObjectsWithTag("Obstacle2"));
        obstacles.AddRange(GameObject.FindGameObjectsWithTag("Obstacle3"));

        foreach (GameObject obstacle in obstacles)
        {
            if (obstacle == null) continue;

            // Check if obstacle is in front of player (positive Z direction in your setup)
            if (obstacle.transform.position.z > transform.position.z)
            {
                // Check if obstacle is within laser width
                float distanceFromCenter = Mathf.Abs(obstacle.transform.position.x - transform.position.x);
                if (distanceFromCenter <= laserWidth / 2f)
                {
                    // Add explosion effect here if you want
                    Destroy(obstacle);
                }
            }
        }

        // Also check other obstacle tags
        string[] obstacleTags = { "Obstacle1", "Obstacle2", "Obstacle3" };
        foreach (string tag in obstacleTags)
        {
            GameObject[] taggedObstacles = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obstacle in taggedObstacles)
            {
                if (obstacle == null) continue;

                if (obstacle.transform.position.z > transform.position.z)
                {
                    float distanceFromCenter = Mathf.Abs(obstacle.transform.position.x - transform.position.x);
                    if (distanceFromCenter <= laserWidth / 2f)
                    {
                        Destroy(obstacle);
                    }
                }
            }
        }
    }

    private GameObject SpawnPowerUp(PowerUp.PowerUpType type)
    {
        GameObject prefab = GetPowerUpPrefab(type);
        Transform spawnPoint = GetSpawnPoint(type);

        if (prefab != null && spawnPoint != null)
        {
            GameObject instance = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            SetActivePowerUpInstance(type, instance);
            return instance;
        }

        return null;
    }

    private void SpawnLaser()
    {
        if (activeBiggie != null)
        {
            // Find laser child object in Biggie prefab or create laser effect
            Transform laserChild = activeBiggie.transform.Find("Laser");
            if (laserChild != null)
            {
                activeLaser = laserChild.gameObject;
                activeLaser.SetActive(true);
            }
        }
    }

    private void DestroyLaser()
    {
        if (activeLaser != null)
        {
            activeLaser.SetActive(false);
        }
    }

    private void SpawnShieldEffect()
    {
        if (shieldEffectPrefab != null)
        {
            // Spawn shield effect as child of player
            Vector3 spawnPosition = transform.position + shieldEffectOffset;
            activeShieldEffect = Instantiate(shieldEffectPrefab, spawnPosition, transform.rotation);
            activeShieldEffect.transform.SetParent(transform);

            Debug.Log("Shield effect spawned around player");
        }
        else
        {
            Debug.LogWarning("Shield effect prefab not assigned!");
        }
    }

    private void DestroyShieldEffect()
    {
        if (activeShieldEffect != null)
        {
            Destroy(activeShieldEffect);
            activeShieldEffect = null;
            Debug.Log("Shield effect destroyed");
        }
    }

    private GameObject GetPowerUpPrefab(PowerUp.PowerUpType type)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.EagleStrategem: return biggiePrefab;
            case PowerUp.PowerUpType.Magnet: return oguPrefab;
            case PowerUp.PowerUpType.Shield: return shieldPrefab;
            case PowerUp.PowerUpType.SpeedBoost: return speedPrefab;
            default: return null;
        }
    }

    private Transform GetSpawnPoint(PowerUp.PowerUpType type)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.EagleStrategem: return biggieSpawnPoint;
            case PowerUp.PowerUpType.Magnet: return oguSpawnPoint;
            case PowerUp.PowerUpType.Shield: return shieldSpawnPoint;
            case PowerUp.PowerUpType.SpeedBoost: return speedSpawnPoint;
            default: return transform; // Fallback to player position
        }
    }

    private void SetActivePowerUpInstance(PowerUp.PowerUpType type, GameObject instance)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.EagleStrategem: activeBiggie = instance; break;
            case PowerUp.PowerUpType.Magnet: activeOgu = instance; break;
            case PowerUp.PowerUpType.Shield: activeShield = instance; break;
            case PowerUp.PowerUpType.SpeedBoost: activeSpeed = instance; break;
        }
    }

    private GameObject GetActivePowerUpInstance(PowerUp.PowerUpType type)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.EagleStrategem: return activeBiggie;
            case PowerUp.PowerUpType.Magnet: return activeOgu;
            case PowerUp.PowerUpType.Shield: return activeShield;
            case PowerUp.PowerUpType.SpeedBoost: return activeSpeed;
            default: return null;
        }
    }

    private void ClearActivePowerUpInstance(PowerUp.PowerUpType type)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.EagleStrategem: activeBiggie = null; break;
            case PowerUp.PowerUpType.Magnet: activeOgu = null; break;
            case PowerUp.PowerUpType.Shield: activeShield = null; break;
            case PowerUp.PowerUpType.SpeedBoost: activeSpeed = null; break;
        }
    }

    private float GetAnimationLength(Animator animator, string animationName)
    {
        if (animator == null) return 1f;

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
                return clip.length;
        }
        return 1f; // Default duration
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Obstacle1") ||
            collision.gameObject.CompareTag("Obstacle2") ||
            collision.gameObject.CompareTag("Obstacle3"))

        {
            if (!isShieldActive)
            {
                int damage = GetObstacleDamage(collision.gameObject.tag);
                currentHealth -= damage;

                AudioManager.Instance?.PlayRandomSound2D(HitAudios, 1f);
                AudioManager.Instance?.PlayRandomSound2D(RockAudios, 1f);

                UpdateHealthUI();
                Debug.Log("Player hit! Health: " + currentHealth);

                if (currentHealth <= 0)
                    Die();
            }
            else
            {
                Debug.Log("Shield protected the player!");
            }

            Destroy(collision.gameObject);
        }
    }

    private int GetObstacleDamage(string tag)
    {
        switch (tag)
        {
            case "Obstacle1": return 1;
            case "Obstacle2": return 2;
            case "Obstacle3": return 3;
            default: return 1;
        }
    }

    private bool hasDied = false;

    private void Die()
    {
        if (hasDied) return;
        hasDied = true;

        Debug.Log("Player Died!");

        // Stop forward movement
        MoveToZ.globalSpeed = 0;

        // Disable player movement
        var controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;

        // Pause all particle systems (so they freeze instead of disappearing)
        foreach (var ps in FindObjectsByType<ParticleSystem>(FindObjectsSortMode.None))
        {
            ps.Pause();
        }

        // Disable all animators
        foreach (var a in FindObjectsByType<Animator>(FindObjectsSortMode.None))
        {
            a.enabled = false;
        }

        // Disable any other game logic if needed...

        // Show end screen
        FindFirstObjectByType<EndScreenManager>()?.ShowEndScreen(distanceCounter, CoinManager.Instance?.GetCoinCount() ?? 0);
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

    // Methods for upgrade system
    public void UpgradePowerUpDuration(PowerUp.PowerUpType type, float additionalDuration)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.EagleStrategem:
                biggieDuration += additionalDuration;
                break;
            case PowerUp.PowerUpType.Magnet:
                oguDuration += additionalDuration;
                break;
            case PowerUp.PowerUpType.Shield:
                shieldDuration += additionalDuration;
                break;
            case PowerUp.PowerUpType.SpeedBoost:
                speedDuration += additionalDuration;
                break;
        }
    }

    public void UpgradeBaseSpeed(float speedIncrease)
    {
        baseSpeedMultiplier += speedIncrease;

        // Update current speed if no speed powerup is active
        if (!isSpeedActive)
        {
            MoveToZ.globalSpeed = baseSpeed * baseSpeedMultiplier;
        }
    }

    public void FullHeal()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Public getters for upgrade system
    public float GetBiggieDuration() => biggieDuration;
    public float GetOguDuration() => oguDuration;
    public float GetShieldDuration() => shieldDuration;
    public float GetSpeedDuration() => speedDuration;
    public float GetBaseSpeedMultiplier() => baseSpeedMultiplier;
}