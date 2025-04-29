using UnityEngine;
using System.Collections.Generic;

public class GameWin : MonoBehaviour
{
    // 记录在终点区域内的玩家
    public static HashSet<GameObject> playersInGoal = new HashSet<GameObject>();

    // 当前关卡需要的玩家数量（自动检测）
    private int requiredPlayers = 1;

    private void Start()
    {
        // 自动检测场景中的玩家数量
        DetectPlayerCount();
    }

    // 自动检测场景中的玩家数量
    private void DetectPlayerCount()
    {
        List<GameObject> activePlayers = new List<GameObject>();

        // 查找 "Player" 标签的角色
        GameObject[] topDownPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in topDownPlayers)
        {
            if (player.activeInHierarchy)
            {
                activePlayers.Add(player);
            }
        }

        // 查找 "Player3D" 标签的角色
        GameObject[] platformerPlayers = GameObject.FindGameObjectsWithTag("Player3d");
        foreach (var player in platformerPlayers)
        {
            if (player.activeInHierarchy)
            {
                activePlayers.Add(player);
            }
        }

        requiredPlayers = activePlayers.Count;


        if (GameManager.Instance != null)
        {
            GameManager.Instance.totalPlayers = requiredPlayers;
            GameManager.Instance.UpdateGoalProgress(playersInGoal.Count);
        }


        // 如果没有找到玩家，默认为1（单玩家模式）
        if (requiredPlayers == 0)
        {
            requiredPlayers = 1;
            Debug.LogWarning("未找到玩家对象，默认按单玩家模式运行");
        }
        else
        {
            Debug.Log($"检测到当前关卡需要 {requiredPlayers} 名玩家通关");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player3d"))
        {
            // 添加进入终点区域的玩家
            if (playersInGoal.Add(other.gameObject))
            {
                Debug.Log($"玩家进入终点区域，当前进度: {playersInGoal.Count}/{requiredPlayers}");
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateGoalProgress(playersInGoal.Count);
            }

            // 检查是否满足通关条件
            CheckWinCondition(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player3d"))
        {
            // 玩家离开终点区域后，从集合中移除
            if (playersInGoal.Remove(other.gameObject))
            {
                Debug.Log($"玩家离开终点区域，当前进度: {playersInGoal.Count}/{requiredPlayers}");
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateGoalProgress(playersInGoal.Count);
            }
        }
    }

    // 检查胜利条件
    private void CheckWinCondition(Collider lastEnteredPlayer)
    {
        if (playersInGoal.Count >= requiredPlayers)
        {
            Debug.Log($"所有玩家({requiredPlayers}名)都到达终点！胜利！");
            Time.timeScale = 0f;

            // 记录分析数据
            RecordWinAnalytics(lastEnteredPlayer.transform.position);

            // 显示胜利UI
            ShowWinUI();
        }
    }

    // 记录胜利分析数据
    private void RecordWinAnalytics(Vector3 position)
    {
        if (GameManager.Instance != null && AnalyticsManager.instance != null)
        {
            AnalyticsManager.instance.AddAnalyticsEvent(
                sessionId: GameManager.Instance.sessionId,
                eventType: "Win",
                levelId: GameManager.Instance.levelId,
                timestamp: System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                eventSequence: -1,
                viewBeforeEvent: "N/A",
                reason: "N/A",
                position: position
            );
        }
    }

    // 显示胜利UI
    private void ShowWinUI()
    {
        if (GameOverManager.instance != null)
        {
            GameOverManager.instance.ShowGameOver(true);
        }
        else
        {
            Debug.LogWarning("未找到GameOverManager实例，无法显示胜利UI");
        }
    }
}