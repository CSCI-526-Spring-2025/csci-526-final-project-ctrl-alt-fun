using UnityEngine;
using UnityEngine.SceneManagement; // 用于重新加载场景

public class MovingCylinder : MonoBehaviour
{
    // 记录初始位置
    private Vector3 startPos;

    // 横向移动的速度
    public float speed = 2f;

    // 总的移动范围（左右两个边界之间的距离）
    public float range = 5f;

    public string targetTag = "Player";

    void Start()
    {
        // 保存物体的初始位置
        startPos = transform.position;
    }

    void Update()
    {
        // 使用 Mathf.PingPong 计算出一个在 0 到 range 之间变化的值
        // 然后将其平移到以 startPos.x 为中心的区间：startPos.x - range/2 ~ startPos.x + range/2
        float newX = startPos.x - range / 2f + Mathf.PingPong(Time.time * speed, range);
        transform.position = new Vector3(newX, startPos.y, startPos.z);
    }

    // 如果使用 Trigger 方式，则确保 Cylinder 的 Collider 勾选了 Is Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // End an analytics session
            Vector3 position = other.transform.position;
            string reason = "MovingCylinder";
            if (AnalyticsManager.instance != null) {
                AnalyticsManager.instance.AddLossEvent(reason, position);
            }
            EndGame();
        }
    }


    // 结束游戏的函数
    void EndGame()
    {
        Debug.Log("Game Over! 撞到了移动的柱状物。");
        if (GameOverManager.instance != null)
        {
            GameOverManager.instance.ShowGameOver(false);
        }
        // 示例：重新加载当前场景
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // 若需要真正退出游戏，可以调用 Application.Quit()，但在 Editor 中不会退出
        // Application.Quit();
    }
}
