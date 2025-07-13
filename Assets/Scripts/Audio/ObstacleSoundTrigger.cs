using UnityEngine;

public class ObstacleSoundTrigger : MonoBehaviour
{
    public AudioClip[] audioClips;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Trigger" || other.CompareTag("Trigger"))
        {
            AudioManager.Instance?.PlayRandomSound2D(audioClips, 1f);
        }
    }
}
