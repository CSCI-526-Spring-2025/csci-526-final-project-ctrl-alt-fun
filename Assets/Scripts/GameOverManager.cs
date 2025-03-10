using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        Debug.Log("GameOverPanel: " + gameOverPanel);
    }

    public void ShowGameOver(bool isWin)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            
            if (isWin)
            {
                gameOverText.text = "All right! You Win!";
                gameOverText.color = Color.green;
            }
            else
            {
                gameOverText.text = "Oh shit! You Lose!";
                gameOverText.color = Color.red;
            }

            // End an analytics session
            string sessionId = GameManager.Instance.sessionId;
            string levelId = GameManager.Instance.levelId;
            string eventType = isWin ? "Win" : "Lose";
            // Debug.Log("Ending analytics session: " + sessionId);
            if (AnalyticsManager.instance != null) {
                AnalyticsManager.instance.AddAnalyticsEvent(
                    sessionId: sessionId, 
                    eventType: eventType, 
                    levelId: levelId, 
                    timestamp: System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 
                    eventSequence: -1,
                    viewBeforeEvent: "N/A"
                );
            }
        }
    }

    public void ReplayLevel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}