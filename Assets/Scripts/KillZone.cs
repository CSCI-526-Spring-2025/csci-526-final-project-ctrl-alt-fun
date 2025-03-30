using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has fallen! Game Over.");

            // End an analytics session
            Vector3 position = other.transform.position;
            string reason = "KillZone";
            if (AnalyticsManager.instance != null) {
                AnalyticsManager.instance.AddLossEvent(reason, position);
            }

            if (GameOverManager.instance != null)
            {
                GameOverManager.instance.ShowGameOver(false);
            }
        }
    }
}