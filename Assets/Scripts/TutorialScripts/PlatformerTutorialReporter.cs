using UnityEngine;

public class PlatformerTutorialReporter : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TutorialManager.Instance.MarkCondition("Jumped");
        }

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
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