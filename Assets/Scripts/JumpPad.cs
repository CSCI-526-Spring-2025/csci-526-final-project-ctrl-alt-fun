using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Tooltip("The upward force applied when the player enters the jump pad.")]
    public float jumpForce = 15f;

    [Tooltip("The tag used to identify the player.")]
    public string playerTag = "Player";

    // Called when another collider enters the trigger area
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player
        if (other.CompareTag(playerTag))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Optionally reset the player's vertical velocity for a consistent jump
                Vector3 currentVelocity = rb.velocity;
                currentVelocity.y = 0;
                rb.velocity = currentVelocity;

                // Apply an upward impulse to the player
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}
