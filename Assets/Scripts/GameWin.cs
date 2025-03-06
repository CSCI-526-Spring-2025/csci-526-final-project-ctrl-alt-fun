using UnityEngine;

public class GameWin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has reached the goal! You Win!");

            if (GameOverManager.instance != null)
            {
                GameOverManager.instance.ShowGameOver(true);
            }
        }
    }
}