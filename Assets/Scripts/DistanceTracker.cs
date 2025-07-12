using UnityEngine;
using UnityEngine.UI;

public class DistanceTracker : MonoBehaviour
{
    public static DistanceTracker Instance;

    [Header("Tracking Settings")]
    public float speed = 5f; // How fast the "distance" should increase per second
    public bool isTracking = true;

    private float traveledDistance = 0f;

    [Header("UI Display")]
    public Text distanceText; // Assign this in the Inspector
    public string unit = "m";
    public int roundTo = 0; // 0 = integer, 1 = 0.0 format

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!isTracking)
            return;

        // Increase distance
        traveledDistance += speed * Time.deltaTime;

        // Update UI if assigned
        if (distanceText != null)
        {
            string displayValue = traveledDistance.ToString($"F{roundTo}") + unit;
            distanceText.text = displayValue;
        }

        // TODO: Stop tracking when health == 0
        // if (PlayerHealth.Instance.currentHealth <= 0)
        // {
        //     StopTracking();
        //  or in another code: DistanceTracker.Instance.StopTracking();
        // }
    }

    public void StopTracking()
    {
        isTracking = false;

        Debug.Log($"Final Distance: {traveledDistance:F1}");
        PlayerPrefs.SetFloat("LastRunDistance", traveledDistance);
    }

    public float GetDistance()
    {
        return traveledDistance;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
