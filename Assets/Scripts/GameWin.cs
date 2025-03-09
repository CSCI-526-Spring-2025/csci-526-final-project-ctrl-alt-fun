using UnityEngine;
using System.Collections.Generic;

public class GameWin : MonoBehaviour
{
    // 用于记录已经到达终点的玩家
    private static HashSet<GameObject> playersFinished = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 如果该玩家之前还没到达终点，则添加到集合中
            if (!playersFinished.Contains(other.gameObject))
            {
                playersFinished.Add(other.gameObject);
                Debug.Log("A player has reached the goal! Total finished: " + playersFinished.Count);
            }

            // 检查是否两个玩家都到达终点
            if (playersFinished.Count >= 2)
            {
                Debug.Log("Both players have reached the goal! You Win!");
                if (GameOverManager.instance != null)
                {
                    GameOverManager.instance.ShowGameOver(true);
                }
            }
        }
    }
}
