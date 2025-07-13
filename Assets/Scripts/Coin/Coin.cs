using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public AudioClip[] coinClips; // Assign 2-3 coin audio clips in inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CoinManager.Instance?.AddCoin(value);

            // Use AudioManager to play one random coin clip
            if (coinClips != null && coinClips.Length > 0)
            {
                AudioManager.Instance?.PlayRandomSound2D(coinClips , 1f);
            }

            Destroy(gameObject);
        }
    }
}

