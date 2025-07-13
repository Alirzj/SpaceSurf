using UnityEngine;
using System.Runtime.InteropServices;

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

    [Header("WebGL Gyro Settings")]
    public float webGLTiltSensitivity = 0.08f; // Increased from 0.02f
    public float webGLTiltOffset = 0f; // To calibrate neutral position
    public bool useHorizontalOrientation = true; // Use beta (front-back) for landscape games
    public float webGLSmoothingFactor = 0.5f; // Reduced from 0.8f for more responsiveness
    public float webGLMaxTiltAngle = 45f; // Clamp input to this range
    public bool useWebGLFiltering = true; // Enable additional filtering for WebGL

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

    // WebGL gyro variables
    private float webGLTilt = 0f;
    private float webGLTiltSmoothed = 0f; // Smoothed tilt value
    private float webGLTiltRaw = 0f; // Raw tilt value for debugging
    private bool gyroPermissionGranted = false;
    private bool gyroSupported = false;

    // UI References
    private ArrowControlsUI arrowUI;

    // WebGL JavaScript interface
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RequestDeviceOrientationPermission();
    
    [DllImport("__Internal")]
    private static extern bool IsDeviceOrientationSupported();
    
    [DllImport("__Internal")]
    private static extern float GetDeviceOrientationBeta();
    
    [DllImport("__Internal")]
    private static extern float GetDeviceOrientationGamma();
    
    [DllImport("__Internal")]
    private static extern float GetDeviceOrientationAlpha();
    
    [DllImport("__Internal")]
    private static extern bool HasDeviceOrientationPermission();
#endif

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

        // Initialize WebGL gyro support
        InitializeWebGLGyro();

        // Initialize control type
        SetControlType(currentControlType);
    }

    void InitializeWebGLGyro()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        gyroSupported = IsDeviceOrientationSupported();
        if (gyroSupported)
        {
            Debug.Log("Device orientation supported in WebGL");
            // Request permission (required for iOS 13+ Safari)
            RequestDeviceOrientationPermission();
        }
        else
        {
            Debug.Log("Device orientation not supported in WebGL");
        }
#endif
    }

    void Update()
    {
        // Update WebGL gyro permission status
#if UNITY_WEBGL && !UNITY_EDITOR
        if (gyroSupported && !gyroPermissionGranted)
        {
            gyroPermissionGranted = HasDeviceOrientationPermission();
        }
#endif

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
        float tilt = 0f;

#if UNITY_WEBGL && !UNITY_EDITOR
        if (gyroSupported && gyroPermissionGranted)
        {
            // Use different axes based on orientation
            float tiltValue;
            if (useHorizontalOrientation)
            {
                // For landscape/horizontal games, use beta (front-back tilt)
                // This corresponds to tilting the device left/right when held horizontally
                tiltValue = GetDeviceOrientationBeta();
            }
            else
            {
                // For portrait games, use gamma (left-right tilt)
                tiltValue = GetDeviceOrientationGamma();
            }
            
            // Store raw value for debugging
            webGLTiltRaw = tiltValue;
            
            // Apply offset and clamp to max angle
            tiltValue = tiltValue + webGLTiltOffset;
            tiltValue = Mathf.Clamp(tiltValue, -webGLMaxTiltAngle, webGLMaxTiltAngle);
            
            // Apply sensitivity
            float processedTilt = tiltValue * webGLTiltSensitivity;
            
            // Apply smoothing if enabled
            if (useWebGLFiltering)
            {
                webGLTiltSmoothed = Mathf.Lerp(webGLTiltSmoothed, processedTilt, 1f - webGLSmoothingFactor);
                webGLTilt = webGLTiltSmoothed;
            }
            else
            {
                webGLTilt = processedTilt;
            }
            
            tilt = webGLTilt;
        }
        else
        {
            // Fallback to keyboard for testing in WebGL
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                tilt = -1f;
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                tilt = 1f;
        }
#else
        // Use Unity's standard input system for editor/mobile builds
        tilt = Input.acceleration.x * tiltSensitivity;
#endif

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
        }
    }

    public void OnRightArrowPressed()
    {
        if (currentControlType == ControlType.Arrows)
        {
            rightPressed = true;
        }
    }

    public void OnArrowReleased()
    {
        if (currentControlType == ControlType.Arrows)
        {
            leftPressed = false;
            rightPressed = false;
        }
    }

    // Public method to calibrate gyro (call from UI button)
    public void CalibrateGyro()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (gyroSupported && gyroPermissionGranted)
        {
            if (useHorizontalOrientation)
            {
                webGLTiltOffset = -GetDeviceOrientationBeta();
            }
            else
            {
                webGLTiltOffset = -GetDeviceOrientationGamma();
            }
            Debug.Log($"Gyro calibrated. Offset: {webGLTiltOffset}");
        }
#endif
    }

    // Method to toggle between horizontal and vertical orientation modes
    public void ToggleOrientation()
    {
        useHorizontalOrientation = !useHorizontalOrientation;
        Debug.Log($"Orientation mode: {(useHorizontalOrientation ? "Horizontal (Beta)" : "Vertical (Gamma)")}");
        // Recalibrate when switching orientation
        CalibrateGyro();
    }

//    void OnGUI()
//    {
//        if (!showDebugInfo) return;

//        GUI.Box(new Rect(10, 10, 300, 200), "Debug Info");
//        GUI.Label(new Rect(20, 35, 280, 20), $"Current Control: {currentControlType}");

//        if (currentControlType == ControlType.Gyro)
//        {
//#if UNITY_WEBGL && !UNITY_EDITOR
//            GUI.Label(new Rect(20, 55, 280, 20), $"WebGL Gyro: {gyroSupported}");
//            GUI.Label(new Rect(20, 75, 280, 20), $"Permission: {gyroPermissionGranted}");
//            GUI.Label(new Rect(20, 95, 280, 20), $"Orientation: {(useHorizontalOrientation ? "Horizontal" : "Vertical")}");
//            GUI.Label(new Rect(20, 115, 280, 20), $"Raw Tilt: {webGLTiltRaw:F2}");
//            GUI.Label(new Rect(20, 135, 280, 20), $"Processed Tilt: {webGLTilt:F2}");
//            GUI.Label(new Rect(20, 155, 280, 20), $"Fast Mode: {inFastMode}");
//#else
//            GUI.Label(new Rect(20, 55, 280, 20), $"Tilt: {Input.acceleration.x:F2}");
//            GUI.Label(new Rect(20, 75, 280, 20), $"Fast Mode: {inFastMode}");
//#endif
//        }
//        else
//        {
//            GUI.Label(new Rect(20, 55, 280, 20), $"Arrow Input: {arrowInput:F2}");
//            GUI.Label(new Rect(20, 75, 280, 20), "Touch arrow buttons to move");
//        }

//        GUI.Label(new Rect(20, 175, 280, 20), $"Position: {transform.position.x:F2}");
//    }
}