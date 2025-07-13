using UnityEngine;

public class AnimationStop : MonoBehaviour
{
    public PowerUp.PowerUpType type; // Set this in the prefab or during spawn

    public void DestroySelf()
    {
        // Deactivate the powerup effect before destroying
        PlayerPowerUpController player = FindFirstObjectByType<PlayerPowerUpController>();
        if (player != null)
        {
            player.DeactivatePowerUpEffect(type);
        }

        Destroy(gameObject);
    }
}
