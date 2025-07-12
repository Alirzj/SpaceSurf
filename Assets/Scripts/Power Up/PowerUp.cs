using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, DamageBoost }
    public PowerUpType powerUpType;
    public float duration = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPowerUpController player = other.GetComponent<PlayerPowerUpController>();
            if (player != null)
            {
                Debug.Log("Bam Power Up Collected!");
                player.ActivatePowerUp(powerUpType, duration);
                Destroy(gameObject); // Remove power-up from scene
            }
        }
    }
}
