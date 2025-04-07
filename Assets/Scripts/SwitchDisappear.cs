using UnityEngine;

public class SwitchDisappear : MonoBehaviour
{
    public GameObject door;
        
    private bool isSwitchActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!isSwitchActivated && other.CompareTag("Player"))
        {
            isSwitchActivated = true;
            Debug.Log("Switch activated!");

            DoorController doorCtrl = door.GetComponent<DoorController>();
            if (doorCtrl != null)
            {
                doorCtrl.OpenDoor();
            }
            else
            {
                Debug.LogWarning("DoorController component not found on door object.");
            }

            gameObject.SetActive(false); 
        }
    }
}
