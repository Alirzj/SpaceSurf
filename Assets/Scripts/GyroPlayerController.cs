using UnityEngine;

public enum ControlType
{
    Gyro,
    Arrows
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float tiltSensitivity = 1.5f;
    public float returnSpeed = 2f;
    public float fastReturnSpeed = 8f;
    public float maxTiltAngle = 25f;
    public float maxLiftAmount = 0.5f;
    public float liftMultiplier = 1.5f;
    public float edgeStartDistance = 2f;
    public float smoothing = 5f;
    public float deadZone = 0.05f;
    public float borderOffset = 0f;

    [Header("Gyro Settings")]
    public float tiltChangeThreshold = 0.3f;
    public float fastModeTime = 0.3f;

    [Header("Arrow Control Settings")]
    public float arrowMoveSpeed = 6f;
    public float arrowSmoothing = 8f;

    [Header("Control Type")]
    public ControlType currentControlType = ControlType.Gyro;

    [Header("Debug")]
    public bool showDebugInfo = true;

    private float minX, maxX, playerHalfWidth;
    private float baseY;
    private Vector3 targetVisualPos;
    private Quaternion targetVisualRot;
    private float centerX;
    private float targetX;

    // Gyro-specific variables
    private float lastTilt = 0f;
    private float fastModeTimer = 0f;
    private bool inFastMode = false;

    // Arrow control variables
    private float arrowInput = 0f;
    private bool leftPressed = false;
    private bool rightPressed = false;

    // UI References
    private ArrowControlsUI arrowUI;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            playerHalfWidth = sr.bounds.extents.x;
        else
            playerHalfWidth = GetComponent<Collider>().bounds.extents.x;

        float depth = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
        Vector3 leftEdge = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2f, depth));
        Vector3 rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, depth));

        minX = leftEdge.x + playerHalfWidth + borderOffset;
        maxX = rightEdge.x - playerHalfWidth - borderOffset;
        centerX = (minX + maxX) / 2f;
        baseY = transform.position.y;

        targetX = centerX;

        // Find or create arrow UI
        arrowUI = FindFirstObjectByType<ArrowControlsUI>();
        if (arrowUI == null)
        {
            Debug.LogWarning("ArrowControlsUI not found. Arrow controls will not be visible.");
        }

        // Initialize control type
        SetControlType(currentControlType);
    }

    void Update()
    {
        // Handle input based on current control type
        switch (currentControlType)
        {
            case ControlType.Gyro:
                HandleGyroInput();
                break;
            case ControlType.Arrows:
                HandleArrowInput();
                break;
        }

        // Apply movement and visual effects
        ApplyMovement();
        ApplyVisualEffects();
    }

    void HandleGyroInput()
    {
        float tilt = Input.acceleration.x * tiltSensitivity;

        // Calculate tilt change rate
        float tiltChange = Mathf.Abs(tilt - lastTilt);

        // Check if we should enter fast mode
        if (tiltChange > tiltChangeThreshold)
        {
            inFastMode = true;
            fastModeTimer = fastModeTime;
        }

        // Update fast mode timer
        if (inFastMode)
        {
            fastModeTimer -= Time.deltaTime;
            if (fastModeTimer <= 0f)
            {
                inFastMode = false;
            }
        }

        // Store current tilt for next frame
        lastTilt = tilt;

        // Convert tilt to target position
        float tiltRange = (maxX - minX) / 2f;
        targetX = centerX + (tilt * tiltRange);
        targetX = Mathf.Clamp(targetX, minX, maxX);
    }

    void HandleArrowInput()
    {
        // Calculate arrow input based on button states
        arrowInput = 0f;

        if (leftPressed)
            arrowInput -= 1f;
        if (rightPressed)
            arrowInput += 1f;

        // Also check for keyboard input for testing
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            arrowInput -= 1f;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            arrowInput += 1f;

        // Apply deadzone
        if (Mathf.Abs(arrowInput) < deadZone)
            arrowInput = 0f;

        // Calculate target position based on input
        if (Mathf.Abs(arrowInput) > 0.01f)
        {
            targetX += arrowInput * arrowMoveSpeed * Time.deltaTime;
            targetX = Mathf.Clamp(targetX, minX, maxX);
        }
    }

    void ApplyMovement()
    {
        float currentReturnSpeed = (currentControlType == ControlType.Gyro && inFastMode) ? fastReturnSpeed :
                                   (currentControlType == ControlType.Arrows) ? arrowSmoothing : returnSpeed;

        // Smoothly move ship toward target position
        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.Lerp(transform.position.x, targetX, currentReturnSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    void ApplyVisualEffects()
    {
        // Edge effect for tilt/lift
        float distanceToLeftEdge = Mathf.Abs(transform.position.x - minX);
        float distanceToRightEdge = Mathf.Abs(transform.position.x - maxX);
        float distanceToEdge = Mathf.Min(distanceToLeftEdge, distanceToRightEdge);
        float edgeFactor = 1f - Mathf.Clamp01(distanceToEdge / edgeStartDistance);
        float side = transform.position.x < centerX ? -1f : 1f;
        float tiltAngle = side * edgeFactor * maxTiltAngle;
        float lift = edgeFactor * maxLiftAmount * liftMultiplier;

        targetVisualRot = Quaternion.Euler(0f, 0f, tiltAngle);
        targetVisualPos = new Vector3(transform.position.x, baseY + lift, transform.position.z);

        // Use faster smoothing for visual elements during fast mode (gyro only)
        float currentSmoothing = (currentControlType == ControlType.Gyro && inFastMode) ? smoothing * 2f : smoothing;

        transform.rotation = Quaternion.Lerp(transform.rotation, targetVisualRot, currentSmoothing * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, targetVisualPos, currentSmoothing * Time.deltaTime);
    }

    public void ToggleControlType()
    {
        ControlType newType = currentControlType == ControlType.Gyro ? ControlType.Arrows : ControlType.Gyro;
        SetControlType(newType);
    }

    public void SetControlType(ControlType newType)
    {
        currentControlType = newType;

        // Reset state when switching
        arrowInput = 0f;
        leftPressed = false;
        rightPressed = false;
        lastTilt = 0f;
        inFastMode = false;
        fastModeTimer = 0f;

        // Update UI visibility
        if (arrowUI != null)
        {
            arrowUI.SetVisible(currentControlType == ControlType.Arrows);
        }

        Debug.Log($"Control type changed to: {currentControlType}");
    }

    // Called by UI buttons
    public void OnLeftArrowPressed()
    {
        if (currentControlType == ControlType.Arrows)
        {
            leftPressed = true;
            Debug.Log("Left arrow pressed");
        }
    }

    public void OnRightArrowPressed()
    {
        if (currentControlType == ControlType.Arrows)
        {
            rightPressed = true;
            Debug.Log("Right arrow pressed");
        }
    }

    public void OnArrowReleased()
    {
        if (currentControlType == ControlType.Arrows)
        {
            leftPressed = false;
            rightPressed = false;
            Debug.Log("Arrow released");
        }
    }

    void OnGUI()
    {
        if (!showDebugInfo) return;

        GUI.Box(new Rect(10, 10, 300, 100), "Debug Info");
        GUI.Label(new Rect(20, 35, 280, 20), $"Current Control: {currentControlType}");

        if (currentControlType == ControlType.Gyro)
        {
            GUI.Label(new Rect(20, 55, 280, 20), $"Tilt: {Input.acceleration.x:F2}");
            GUI.Label(new Rect(20, 75, 280, 20), $"Fast Mode: {inFastMode}");
        }
        else
        {
            GUI.Label(new Rect(20, 55, 280, 20), $"Arrow Input: {arrowInput:F2}");
            GUI.Label(new Rect(20, 75, 280, 20), "Touch arrow buttons to move");
        }

        GUI.Label(new Rect(20, 95, 280, 20), $"Position: {transform.position.x:F2}");
    }
}