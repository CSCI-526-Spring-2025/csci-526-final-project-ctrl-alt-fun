using UnityEngine;

public class TeleportBlockScript : MonoBehaviour
{
    // 在 Inspector 中将 TeleportTarget（可移动目标块）拖拽到这里
    public Transform teleportTarget;

    // 当红色方块进入 TeleportBlock 的触发器时，将其传送到 teleportTarget 的位置
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && teleportTarget != null)
        {
            // 将红色方块位置设置为可移动目标块的位置
            other.transform.position = teleportTarget.position;
            // 如果担心卡进地面，可加一点向上偏移:
            // other.transform.position += Vector3.up * 0.5f;

            Debug.Log("红色方块已被传送到 TeleportTarget 所在位置！");
        }
    }
}
