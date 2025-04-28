using UnityEngine;

public class SwitchDisappear : MonoBehaviour
{
    public GameObject door;

    private bool isSwitchActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        // 用名字而不是 Tag
        if (!isSwitchActivated && other.name == "Platformer")
        {
            isSwitchActivated = true;
            Debug.Log("Switch activated by " + other.name);

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
