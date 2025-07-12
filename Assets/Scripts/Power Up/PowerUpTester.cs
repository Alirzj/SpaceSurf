using UnityEngine;

public class PowerUpTester : MonoBehaviour
{
    public PlayerPowerUpController playerPowerUpController;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Testing Biggie (EagleStrategem)");
            playerPowerUpController.ActivatePowerUp(PowerUp.PowerUpType.EagleStrategem);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Testing Magnet (Ogu)");
            playerPowerUpController.ActivatePowerUp(PowerUp.PowerUpType.Magnet);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Testing Shield");
            playerPowerUpController.ActivatePowerUp(PowerUp.PowerUpType.Shield);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("Testing Speed Boost");
            playerPowerUpController.ActivatePowerUp(PowerUp.PowerUpType.SpeedBoost);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Current Speed: " + MoveToZ.globalSpeed);
        }
    }
}
