using UnityEngine;
using TMPro;

public class TutorialUI : MonoBehaviour
{
    public static TutorialUI Instance;

    public GameObject panel;
    public TextMeshProUGUI tutorialText;

    void Awake()
    {
        Instance = this;
        Hide();
    }

    public void ShowMessage(string msg, float duration = 0f)
    {
        panel.SetActive(true);
        tutorialText.text = msg;

        CancelInvoke();
        if (duration > 0f)
        {
            Invoke("Hide", duration);
        }
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}