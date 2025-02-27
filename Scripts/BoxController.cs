using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    public GameManager gameManager;
     // 当前是否为 TopDown 模式
    private Rigidbody rb;
    private Collider boxCollider;
    private bool isPickedUp = false;
    private Transform playerTransform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();
        UpdateBoxState();
    }

    void Update()
    {
        // 根据模式切换状态
        UpdateBoxState();

        // 如果被拾取，保持与角色位置重合
        if (isPickedUp && playerTransform != null)
        {
            // 直接使用世界坐标，使箱子和角色重合
            transform.position = playerTransform.position;
        }
    }

    // 更新箱子的状态
    void UpdateBoxState()
    {
        if (gameManager.isTopDownView)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            boxCollider.isTrigger = isPickedUp; // 被拾取后忽略碰撞
        }
        else
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            boxCollider.isTrigger = false; // Platform 模式下恢复碰撞
            rb.useGravity = false;
            ReleaseBox();
        }
    }

    // 拾取箱子
    public void PickUpBox(Transform player)
    {
        if (gameManager.isTopDownView)
        {
            isPickedUp = true;
            playerTransform = player;
            transform.position = player.position; // 立即更新位置
            boxCollider.isTrigger = true; // 忽略碰撞
        }
    }

    // 放置箱子
    public void PlaceBox(Vector3 placePosition)
    {
        if (gameManager.isTopDownView && isPickedUp)
        {
            isPickedUp = false;
            playerTransform = null;
            transform.position = placePosition;
            boxCollider.isTrigger = false; // 恢复碰撞
        }
    }

    // 释放箱子（切换到 Platform 模式时调用）
    public void ReleaseBox()
    {
        if (isPickedUp)
        {
            isPickedUp = false;
            playerTransform = null;
            boxCollider.isTrigger = false; // 恢复碰撞
        }
    }
}