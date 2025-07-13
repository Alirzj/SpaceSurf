using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndScreenManager : MonoBehaviour
{
    public GameObject endScreenCanvas;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI coinsText;

    private float finalScore;
    private int totalCoins;

    public void ShowEndScreen(float score, int coins)
    {
        finalScore = score;
        totalCoins = coins;

        float highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        if (score > highScore)
        {
            PlayerPrefs.SetFloat("HighScore", score);
            highScore = score;
        }

        scoreText.text = $"Score: {score:F1}";
        highScoreText.text = $"High Score: {highScore:F1}";
        coinsText.text = $"Coins: {coins}";

        endScreenCanvas.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Main Menu"); // Change this to your actual main menu scene name
    }
}
