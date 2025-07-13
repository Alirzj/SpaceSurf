using System.Collections;
using UnityEngine;

public class CardRotator : MonoBehaviour
{
    [SerializeField] private float maxRotationX = 30.0f;
    [SerializeField] private float maxRotationY = 30.0f;
    [SerializeField] private float rotationSpeed = 90.0f;
    [SerializeField] private float snapBackDuration = 0.1f;

    private bool isRotating = false;
    private Coroutine snapBackRoutine = null;
    private Vector3 rotationStartPos;

    private void Update()
    {
        Vector3 inputPos = Vector3.zero;
        bool inputBegan = false;
        bool inputEnded = false;
        bool inputHeld = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        // Mouse input
        inputPos = Input.mousePosition;
        inputBegan = Input.GetMouseButtonDown(0);
        inputEnded = Input.GetMouseButtonUp(0);
        inputHeld = Input.GetMouseButton(0);
#else
    // Touch input
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);
        inputPos = touch.position;

        inputBegan = touch.phase == TouchPhase.Began;
        inputEnded = touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled;
        inputHeld = touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
    }
#endif

        inputPos.z = 1.0f;

        if (!isRotating && inputBegan)
        {
            isRotating = true;
            StopSnapBack();
            rotationStartPos = Camera.main.ScreenToWorldPoint(inputPos);
        }

        if (isRotating && inputEnded)
        {
            isRotating = false;
            StopSnapBack();
            snapBackRoutine = StartCoroutine(SnapBack());
        }

        if (isRotating && inputHeld)
        {
            var rotationCurrentPos = Camera.main.ScreenToWorldPoint(inputPos);
            var offset = rotationCurrentPos - rotationStartPos;

            var xRotation = offset.x * -rotationSpeed;
            var yRotation = offset.y * rotationSpeed;

            xRotation = Mathf.Clamp(xRotation, -maxRotationX, maxRotationX);
            yRotation = Mathf.Clamp(yRotation, -maxRotationY, maxRotationY);

            transform.rotation = Quaternion.Euler(yRotation, xRotation, 0);
        }
    }


    private void StopSnapBack()
    {
        if(snapBackRoutine != null)
        {
            StopCoroutine(snapBackRoutine);
            snapBackRoutine = null;
        }
    }

    private IEnumerator SnapBack()
    {
        var startRotation = transform.rotation;
        var endRotation = Quaternion.identity;

        for(float t = 0.0f; t < snapBackDuration; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t / snapBackDuration);
            yield return null;
        }

        transform.rotation = endRotation;
    }
}
