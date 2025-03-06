using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has fallen! Game Over.");

            if (GameOverManager.instance != null)
            {
                GameOverManager.instance.ShowGameOver(false);
            }
        }
    }
}