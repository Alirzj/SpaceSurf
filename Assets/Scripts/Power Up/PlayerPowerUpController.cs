using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUpController : MonoBehaviour
{
    public float baseSpeed = 5f;
    public float speedBoostMultiplier = 2f;
    public float baseDamage = 10f;
    public float damageBoostMultiplier = 2f;

    private float currentSpeed;
    private float currentDamage;

    private Dictionary<PowerUp.PowerUpType, Coroutine> activePowerUps = new Dictionary<PowerUp.PowerUpType, Coroutine>();

    private void Start()
    {
        ResetStats();
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
                currentSpeed = activate ? baseSpeed * speedBoostMultiplier : baseSpeed;
                break;
            case PowerUp.PowerUpType.DamageBoost:
                currentDamage = activate ? baseDamage * damageBoostMultiplier : baseDamage;
                break;
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
