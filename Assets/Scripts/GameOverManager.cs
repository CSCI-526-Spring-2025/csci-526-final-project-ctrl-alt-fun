using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public GameObject nextLevelButton;
    public GameObject replayButton;

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
            GameWin.playersInGoal.Clear();
            
            if (isWin)
            {
                gameOverText.text = "You Win!";
                gameOverText.color = Color.green;
                if (nextLevelButton != null) nextLevelButton.SetActive(true);
                if (replayButton != null) replayButton.SetActive(false);
            }
            else
            {
                gameOverText.text = "You Lose!";
                gameOverText.color = Color.red;
                if (nextLevelButton != null) nextLevelButton.SetActive(false);
                if (replayButton != null) replayButton.SetActive(true);
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
        Time.timeScale = 1f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        SceneManager.LoadScene("MainMenu");
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        string nextSceneName = GetNextLevelName(currentSceneName);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("No next level found. Returning to Main Menu.");
            SceneManager.LoadScene("MainMenu");
        }
    }

    private string GetNextLevelName(string currentLevel)
    {
        Match match = Regex.Match(currentLevel, @"Level(\d+)");
        if (match.Success)
        {
            int currentLevelNumber = int.Parse(match.Groups[1].Value);
            string nextLevel = $"Level{currentLevelNumber + 1}";

            if (SceneExists(nextLevel))
            {
                return nextLevel;
            }
        }
        return null;
    }

    private bool SceneExists(string sceneName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}
