using UnityEngine;

public class PlayerTutorialReporter : MonoBehaviour
{
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            TutorialManager.Instance.MarkCondition("Shifted");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TutorialManager.Instance.MarkCondition("Paused");
        }
    }
}