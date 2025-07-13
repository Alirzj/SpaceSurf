using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroSwiper : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public int totalPages = 7;
    public float swipeThreshold = 0.2f;
    public float snapSpeed = 10f;
    public string nextSceneName = "MainScene";

    private int currentPage = 0;
    private Vector2[] pagePositions;
    private bool isDragging = false;

    void Start()
    {
        pagePositions = new Vector2[totalPages];
        float step = 1f / (totalPages - 1);
        for (int i = 0; i < totalPages; i++)
        {
            pagePositions[i] = new Vector2(i * step, 0);
        }
    }

    void Update()
    {
        if (!isDragging)
        {
            scrollRect.normalizedPosition = Vector2.Lerp(scrollRect.normalizedPosition, pagePositions[currentPage], Time.unscaledDeltaTime * snapSpeed);
        }
    }

    public void OnBeginDrag()
    {
        isDragging = true;
    }

    public void OnEndDrag()
    {
        isDragging = false;

        float pos = scrollRect.horizontalNormalizedPosition;
        float step = 1f / (totalPages - 1);

        int nearest = Mathf.RoundToInt(pos / step);
        currentPage = Mathf.Clamp(nearest, 0, totalPages - 1);
    }

    public void SkipIntro()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
