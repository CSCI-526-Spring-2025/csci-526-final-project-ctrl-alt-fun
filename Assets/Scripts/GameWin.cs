using UnityEngine;
using System.Collections.Generic;

public class GameWin : MonoBehaviour
{
    // 用于记录当前在终点区域内的玩家
    private static HashSet<GameObject> playersInGoal = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 添加进入终点区域的玩家
            if (playersInGoal.Add(other.gameObject))
            {
                Debug.Log("Player entered the goal area. Count: " + playersInGoal.Count);
            }

            // 检查是否两个玩家都在终点区域
            if (playersInGoal.Count >= 2)
            {
                Debug.Log("Both players are in the goal area! You Win!");

                Vector3 position = other.transform.position;
                string sessionId = GameManager.Instance.sessionId;
                string levelId = GameManager.Instance.levelId;
                if (AnalyticsManager.instance != null) {
                    AnalyticsManager.instance.AddAnalyticsEvent(
                        sessionId: sessionId, 
                        eventType: "Win", 
                        levelId: levelId, 
                        timestamp: System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), 
                        eventSequence: -1,
                        viewBeforeEvent: "N/A",
                        reason: "N/A",
                        position: position
                    );
                }

                if (GameOverManager.instance != null)
                {
                    GameOverManager.instance.ShowGameOver(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 玩家离开终点区域后，从集合中移除
            if (playersInGoal.Remove(other.gameObject))
            {
                Debug.Log("Player left the goal area. Count: " + playersInGoal.Count);
            }
        }
    }
}
