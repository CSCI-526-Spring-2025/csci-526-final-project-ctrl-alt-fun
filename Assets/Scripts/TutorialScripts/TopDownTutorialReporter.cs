using UnityEngine;

public class TopDownTutorialReporter : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow))
        {
            TutorialManager.Instance.MarkCondition("Moved");
        }


        if (Input.GetKeyDown(KeyCode.X))
        {
            TutorialManager.Instance.MarkCondition("Picked");
        }
    }
}