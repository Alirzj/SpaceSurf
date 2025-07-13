using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    [Header("Camera Shake Settings")]
    public CinemachineVirtualCamera virtualCamera;
    public float defaultIntensity = 2f;
    public float defaultDuration = 0.15f;

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;
    private float originalAmplitude; // Store the original amplitude

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise != null)
        {
            // Store the original amplitude value
            originalAmplitude = noise.m_AmplitudeGain;
        }
    }

    public void ShakeCamera()
    {
        ShakeCamera(defaultIntensity, defaultDuration);
    }

    public void ShakeCamera(float intensity, float duration)
    {
        if (noise == null) return;

        noise.m_AmplitudeGain = intensity;
        shakeTimer = duration;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f && noise != null)
            {
                // Restore to original amplitude instead of 0
                noise.m_AmplitudeGain = originalAmplitude;
            }
        }
    }
}