using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public List<TutorialStep> steps = new List<TutorialStep>();

    private int currentIndex = 0;
    private HashSet<string> completedConditions = new HashSet<string>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        TryShowNextStep(); // 自动从第 0 步开始
    }

    void TryShowNextStep()
    {
        while (currentIndex < steps.Count)
        {
            var step = steps[currentIndex];

            // 如果已经完成，跳过这步
            if (completedConditions.Contains(step.condition))
            {
                currentIndex++;
                continue;
            }

            // 否则，显示这步提示
            TutorialUI.Instance.ShowMessage(step.message, step.autoHideAfter);
            break;
        }
    }

    public void MarkCondition(string condition)
    {
        if (!completedConditions.Contains(condition))
        {
            completedConditions.Add(condition);

            // 如果这就是当前要完成的步骤，推进
            if (steps[currentIndex].condition == condition)
            {
                TutorialUI.Instance.Hide();
                currentIndex++;
                TryShowNextStep(); // 自动进入下一步
            }
        }
    }
}
