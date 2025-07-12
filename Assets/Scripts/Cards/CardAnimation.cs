using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CardAnimation : MonoBehaviour
{
    public RectTransform targetTransform; // target position and rotation
    public float moveDuration = 0.5f;

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void AnimateIn()
    {
        StartCoroutine(AnimateToTarget());
    }

    IEnumerator AnimateToTarget()
    {
        Vector3 startPos = rectTransform.anchoredPosition;
        Quaternion startRot = rectTransform.rotation;

        Vector3 endPos = targetTransform.anchoredPosition;
        Quaternion endRot = targetTransform.rotation;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / moveDuration;
            rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
            rectTransform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }

        rectTransform.anchoredPosition = endPos;
        rectTransform.rotation = endRot;
    }
}
