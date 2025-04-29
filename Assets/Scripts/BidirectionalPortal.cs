using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BidirectionalPortal : MonoBehaviour
{

    [Tooltip("The paired portal (another portal that this one connects to)")]
    public BidirectionalPortal linkedPortal; // 关联的传送门

    [Tooltip("Cooldown time to prevent immediate re-teleporting")]
    public float teleportCooldown = 0.5f; // 传送冷却时间

    [Tooltip("Tags that can be teleported (e.g., Player, Enemy)")]
    public List<string> targetTags ; // 可传送的对象列表

    private bool isTeleporting = false; // 是否在冷却状态
    private Vector3 portalDirection; // 计算出的传送门方向

    private void Start()
    {
        // 获取第一层父物体（即 `portal1` / `portal2`）
        Transform parentTransform = transform.parent;
        if (parentTransform != null)
        {
            // 计算方向：realPortal - 父物体坐标
            portalDirection = (transform.position - parentTransform.position).normalized;
        }
        else
        {
            Debug.LogError("TeleportPortal must have a parent object!");
            portalDirection = transform.forward; // 兜底方案，防止出错
        }
    }



    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.WakeUp(); // 强制唤醒刚体，防止它被物理系统冻结
            Debug.Log("物体进入传送门（已唤醒），rb.velocity = " + rb.velocity);
        }
        // 1. 过滤掉不符合 tag 的物体
        if (!targetTags.Contains(other.tag)) return;
        // 2. 确保有有效的传送门
        if (linkedPortal == null || isTeleporting) return;
        if (GameManager.Instance != null && GameManager.Instance.isTopDownView) return; 
        // 3. 确保玩家接触的是 "正确的方向"（浅蓝色长表面）
        if (!IsCorrectSurface(other)) return;

        //Rigidbody rb = other.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            Vector3 incomingVelocity = rb.velocity;

            StartCoroutine(TeleportWithVelocity(other, incomingVelocity));

        }
        else
        {
            StartCoroutine(TeleportWithVelocity(other, Vector3.zero)); // 没有刚体的情况
        }
    }

    private bool IsCorrectSurface(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return false; // 如果物体没有刚体，则不允许传送

        Vector3 incomingVelocity = rb.velocity.normalized; // 归一化速度方向
        float angle = Vector3.Angle(incomingVelocity, portalDirection); // 计算角度

        // 只有角度在 90° ~ 180° 之间，才允许传送
        return (angle >= 90f && angle <= 270f);
    }

    private IEnumerator TeleportWithVelocity(Collider objectToTeleport, Vector3 incomingVelocity)
    {
        isTeleporting = true; // 开启冷却
        linkedPortal.isTeleporting = true; // 另一端也进入冷却

        // 直接传送到 `linkedPortal` 的位置
        Vector3  offset = linkedPortal.portalDirection * 0.5f;
        objectToTeleport.transform.position = linkedPortal.transform.position + offset ;

        // 用 `linkedPortal` 计算出的方向替代 `transform.forward`
        Vector3 newVelocity = linkedPortal.portalDirection * incomingVelocity.magnitude ;
        Rigidbody rb = objectToTeleport.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = newVelocity; // 赋予新方向的速度
        }

        yield return new WaitForSeconds(teleportCooldown); // 冷却时间

        isTeleporting = false;
        linkedPortal.isTeleporting = false;
    }
}