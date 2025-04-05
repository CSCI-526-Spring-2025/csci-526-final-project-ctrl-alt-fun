[System.Serializable]
public class TutorialStep
{
    public string id;
    public string message;
    public string condition;
    public float autoHideAfter = 0f; // optional
}