using UnityEngine;

public class TopDownTutorialReporter : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow))
        {
            TutorialManager.Instance.MarkCondition("Moved");
        }

        if (Input.GetKey(KeyCode.Space) && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow)))
        {
            TutorialManager.Instance.MarkCondition("Changed");
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            TutorialManager.Instance.MarkCondition("Picked");
        }
    }
}