using UnityEngine;
using System.Collections;

public class TeleportPortal : MonoBehaviour
{
    [Tooltip("Set to true if this portal is the entry portal (Portal A); set to false for the exit portal (Portal B)")]
    public bool isEntryPortal = true;

    [Tooltip("The exit portal's Transform (only used for entry portals)")]
    public Transform exitPortal;

    [Tooltip("Teleport cooldown time to prevent immediate re-teleporting")]
    public float teleportCooldown = 1f;

    // Flag to indicate whether teleportation is on cooldown
    private bool isTeleporting = false;

    // Called when another object enters the trigger area
    private void OnTriggerEnter(Collider other)
    {
        // If this is not an entry portal, do not teleport
        if (!isEntryPortal)
            return;

        // Check if the colliding object is the player and if teleport is not on cooldown
        if (!isTeleporting && other.CompareTag("Player"))
        {
            Teleport(other.gameObject);
        }
    }

    // Teleport the player to the exit portal's position
    private void Teleport(GameObject player)
    {
        if (exitPortal != null)
        {
            // Move the player to the exit portal's position
            player.transform.position = exitPortal.position;
            // Optionally, set the player's rotation to match the exit portal's rotation:
            // player.transform.rotation = exitPortal.rotation;

            // Start the teleport cooldown coroutine
            StartCoroutine(TeleportCooldownRoutine());
        }
        else
        {
            Debug.LogWarning("Exit portal is not assigned!");
        }
    }

    // Cooldown coroutine to prevent immediate re-teleporting
    private IEnumerator TeleportCooldownRoutine()
    {
        isTeleporting = true;
        yield return new WaitForSeconds(teleportCooldown);
        isTeleporting = false;
    }
}
