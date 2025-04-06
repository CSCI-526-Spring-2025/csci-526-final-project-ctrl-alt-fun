using UnityEngine;

public class SwitchController : MonoBehaviour
{
    // Reference to the door that should open when this switch is activated.
    public GameObject door;
    
    // Flag to determine if the switch is currently activated.
    private bool isSwitchActivated = false;

    // Called when an object enters the switch's trigger collider.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player and the switch is not already activated.
        if (!isSwitchActivated && other.CompareTag("Player"))
        {
            isSwitchActivated = true;
            Debug.Log("Switch activated!");

            // Try to get the DoorController component from the assigned door.
            DoorController doorCtrl = door.GetComponent<DoorController>();
            if (doorCtrl != null)
            {
                doorCtrl.OpenDoor();
            }
            else
            {
                Debug.LogWarning("DoorController component not found on door object.");
            }
        }
    }

    // Called when an object exits the switch's trigger collider.
    private void OnTriggerExit(Collider other)
    {
        // Check if the collider belongs to the player and the switch is currently activated.
        if (isSwitchActivated && other.CompareTag("Player"))
        {
            isSwitchActivated = false;
            Debug.Log("Switch deactivated!");

            // Try to get the DoorController component from the assigned door.
            DoorController doorCtrl = door.GetComponent<DoorController>();
            if (doorCtrl != null)
            {
                doorCtrl.CloseDoor();
            }
            else
            {
                Debug.LogWarning("DoorController component not found on door object.");
            }
        }
    }
}