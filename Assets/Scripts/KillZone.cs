using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player has collided with KillZone! Game Over.");

            Vector3 position = collision.transform.position;
            string reason = "KillZone";

            if (AnalyticsManager.instance != null)
            {
                AnalyticsManager.instance.AddLossEvent(reason, position);
            }

            if (GameOverManager.instance != null)
            {
                GameOverManager.instance.ShowGameOver(false);
            }
        }
    }
}
