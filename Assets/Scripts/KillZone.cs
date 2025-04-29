using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Player3d"))
        {
            Debug.Log("Player has collided with KillZone! Game Over.");

            // Record an analytics session
            Vector3 position = collision.transform.position;
            string reason = "KillZone";
            if (AnalyticsManager.instance != null)
            {
                AnalyticsManager.instance.AddLossEvent(reason, position);
            }
            // End recording

            if (GameOverManager.instance != null)
            {
                GameOverManager.instance.ShowGameOver(false);
            }
        }
    }
}
