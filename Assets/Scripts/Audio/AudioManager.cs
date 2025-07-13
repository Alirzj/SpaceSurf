using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Settings")]
    public float defaultVolume = 1f;

    [Header("Audio Routing")]
    public AudioMixerGroup mixerGroup; // Assign the "All" group here in Inspector

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Plays a single 2D sound.
    /// </summary>
    public void PlaySound2D(AudioClip clip, float volume = -1f)
    {
        if (clip == null) return;

        GameObject temp = new GameObject("OneShotSound");
        AudioSource source = temp.AddComponent<AudioSource>();
        source.clip = clip;
        source.outputAudioMixerGroup = mixerGroup;
        source.volume = (volume >= 0) ? volume : defaultVolume;
        source.Play();

        Destroy(temp, clip.length);
    }

    /// <summary>
    /// Plays a random 2D sound from a given array.
    /// </summary>
    public void PlayRandomSound2D(AudioClip[] clips, float volume = -1f)
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        PlaySound2D(clips[index], volume);
    }
}
