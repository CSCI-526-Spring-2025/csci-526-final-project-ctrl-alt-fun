using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;

public class LevelButton : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI levelText;
    private string levelName;

    void Start()
    {
        button = GetComponent<Button>();
        levelText = GetComponentInChildren<TextMeshProUGUI>();

        if (levelText == null)
        {
            Debug.LogError("LevelButton: TextMeshProUGUI component not found.");
            return;
        }

        Debug.Log($"Button text: {levelText.text}");

        string levelNumber = Regex.Match(levelText.text, @"\d+").Value;

        if (!string.IsNullOrEmpty(levelNumber))
        {
            levelName = "Level" + levelNumber;
            Debug.Log($"Parsed level name: {levelName}");
        }
        else
        {
            Debug.LogError("LevelButton: Unable to extract level number.");
        }

        if (button != null)
        {
            button.onClick.AddListener(LoadLevel);
        }
    }

    void LoadLevel()
    {
        if (!string.IsNullOrEmpty(levelName))
        {
            Debug.Log($"Loading scene: {levelName}");
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogError("LevelButton: Level name is empty. Cannot load scene.");
        }
    }
}
