using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, EagleStrategem, Magnet, Shield }
    public PowerUpType powerUpType;
    public float duration = 5f;
    public AudioSource audio;

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
                // Create audio object manually
                GameObject audioObj = new GameObject("OneShotAudio");
                audioObj.transform.position = transform.position;

                AudioSource source = audioObj.AddComponent<AudioSource>();
                source.clip = audio.clip; // Use your assigned AudioSource's clip
                source.Play();

                Destroy(audioObj, source.clip.length);

                Debug.Log("Bam Power Up Collected!");
                player.ActivatePowerUp(powerUpType, duration);
                Destroy(gameObject); // Now safe to destroy this
            }
        }
    }

}
