using UnityEngine;

public class GyroPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float tiltSensitivity = 1.5f;

    private float maxXPosition;
    private float playerHalfWidth;

    private void Start()
    {

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            playerHalfWidth = sr.bounds.extents.x;
        }
        else
        {
            playerHalfWidth = GetComponent<Collider2D>().bounds.extents.x;
        }

        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;

        maxXPosition = screenHalfWidth - playerHalfWidth;
    }

    private void Update()
    {
        float tilt = Input.acceleration.x * tiltSensitivity;

        Vector3 newPosition = transform.position + new Vector3(tilt * moveSpeed * Time.deltaTime, 0, 0);

        newPosition.x = Mathf.Clamp(newPosition.x, -maxXPosition, maxXPosition);

        transform.position = newPosition;
    }
}
