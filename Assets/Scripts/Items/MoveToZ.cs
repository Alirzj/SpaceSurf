using UnityEngine;

public class MoveToZ : MonoBehaviour
{
    public float moveSpeed = 5f; // customizable speed

    private float targetZ = -10f;

    void Update()
    {
        Vector3 currentPosition = transform.position;

        // Gradually move Z toward -10
        float newZ = Mathf.MoveTowards(currentPosition.z, targetZ, moveSpeed * Time.deltaTime);
        transform.position = new Vector3(currentPosition.x, currentPosition.y, newZ);

        // Destroy when it reaches or passes the target
        if (transform.position.z <= targetZ)
        {
            Destroy(gameObject);
        }
    }
}
