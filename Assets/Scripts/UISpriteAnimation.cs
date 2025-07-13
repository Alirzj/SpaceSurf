using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour
{
    [Header("Auto Load Settings")]
    public string resourcePath = "SpriteSheets/MySpriteSheet"; // Path under Resources (no .png)

    [Header("Animation Settings")]
    public float frameRate = 10f; // Frames per second

    private Sprite[] frames;
    private Image imageComponent;
    private int currentFrame;
    private float timer;

    void OnEnable()
    {
        imageComponent = GetComponent<Image>();
        currentFrame = 0;
        timer = 0f;

        // Auto-load all sliced sprites from Resources
        frames = Resources.LoadAll<Sprite>(resourcePath);

        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning("No sprites found at: Resources/" + resourcePath);
            enabled = false;
            return;
        }

        // Optional: Sort by name if Unity loads them out of order
        System.Array.Sort(frames, (a, b) => a.name.CompareTo(b.name));

        imageComponent.sprite = frames[0];
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;

        timer += Time.unscaledDeltaTime;

        if (timer >= 1f / frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Length;
            imageComponent.sprite = frames[currentFrame];
            timer = 0f;
        }
    }
}
