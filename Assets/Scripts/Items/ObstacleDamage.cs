using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    public int damageAmount = 1;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // TODO: Add your health script reference here
            // Example:
            // other.GetComponent<PlayerHealth>().TakeDamage(damageAmount);

            Debug.Log($"Player hit! Should take {damageAmount} damage.");

            // Optional: Destroy the obstacle after hitting player
            Destroy(gameObject);
        }
    }
}