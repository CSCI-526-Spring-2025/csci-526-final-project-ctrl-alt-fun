using UnityEngine;

public class TopDownTutorialReporter : MonoBehaviour
{
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     TutorialManager.Instance.MarkCondition("Jumped");
        // }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            TutorialManager.Instance.MarkCondition("Moved");
        }

        // if (Input.GetKeyDown(KeyCode.LeftShift))
        // {
        //     TutorialManager.Instance.MarkCondition("Switched");
        // }

        if (Input.GetKeyDown(KeyCode.X))
        {
            TutorialManager.Instance.MarkCondition("Picked");
        }
    }
}