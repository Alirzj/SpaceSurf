using UnityEngine;

public class MoveToZ : MonoBehaviour
{
    public static float globalSpeed = 40f; // This will be controlled from the powerup system
    private float targetZ = -10f;

    void Update()
    {
        Vector3 currentPosition = transform.position;
        float newZ = Mathf.MoveTowards(currentPosition.z, targetZ, globalSpeed * Time.deltaTime);
        transform.position = new Vector3(currentPosition.x, currentPosition.y, newZ);

        if (transform.position.z <= targetZ)
        {
            Destroy(gameObject);
        }
    }
}
