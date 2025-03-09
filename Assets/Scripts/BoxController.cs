using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
   

    // 当前是否为 TopDown 模式
    private Rigidbody rb;
    private Collider boxCollider;
    private bool isPickedUp = false;
    private Transform playerTransform;
    private Vector3 Zoffset = new Vector3(0, 0, 1);
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<Collider>();

      
    }
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
            transform.position = playerTransform.position - Zoffset;
        }
    }

    // 更新箱子的状态
    void UpdateBoxState()
    {
        if (GameManager.Instance.isTopDownView)
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
        if (GameManager.Instance.isTopDownView)
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
        if (GameManager.Instance.isTopDownView && isPickedUp)
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
            Vector3[] directions = new Vector3[]
            {
            Vector3.right,  // 右
            Vector3.left,   // 左
            Vector3.up,     // 上
            Vector3.down    // 下
            };

            foreach (Vector3 dir in directions)
            {
                Vector3 potentialPosition = transform.position + dir + Zoffset;
                Collider[] hitColliders = Physics.OverlapBox(potentialPosition, Vector3.one * 0.4f, Quaternion.identity);

                // 如果该位置没有其他物体，则放置箱子
                if (hitColliders.Length == 0)
                {
                    transform.position = potentialPosition;
                    boxCollider.isTrigger = false;
                    Debug.Log("箱子放置在: " + potentialPosition);
                    return; // 放置成功，退出函数
                }
            }

            // 如果四个方向都被占用，箱子保持原地
            Debug.Log("无法放置箱子，所有方向都被占据！");
            
            boxCollider.isTrigger = false;
        }
    }
}